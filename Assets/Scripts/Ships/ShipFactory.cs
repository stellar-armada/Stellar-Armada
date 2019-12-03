using System.Collections.Generic;
using System.Linq;
using Mirror;
using StellarArmada.Levels;
using StellarArmada.Match;
using StellarArmada.Teams;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Ships
{
    public class ShipFactory : NetworkBehaviour
    {
        public static ShipFactory instance;

        
        // Share a unique incrementer amongst all entities for dictionary lookups
        public static uint entityIncrement = 0;
        
        [SerializeField] ShipDictionary ships = new ShipDictionary();

        void Awake()
        {
            instance = this;
        }

        [Server]
        public Ship ServerCreateShipForTeam(uint teamId, int groupId, ShipType shipType)
        {

            GameObject shipPrefab;

            bool success = ships.TryGetValue(shipType, out shipPrefab);
            if (!success)
            {
                Debug.LogError("Failed to create ship - was not found in dictionary");
                return null;
            }

            GameObject shipGameObject = Instantiate(shipPrefab);


            Ship s = shipGameObject.GetComponent<Ship>();

            s.SetEntityId(entityIncrement++);
            s.group = groupId;
            
            
            NetworkServer.Spawn(shipGameObject);

            s.ServerSetTeam(teamId);

            // Get group from group ID and add
            TeamManager.instance.GetTeamByID(teamId).groups[groupId].Add(s);
            return s;
        }
        
        [Server]
        public void ServerCreateShipsForTeam(uint teamId)
        {
            Team t = TeamManager.instance.GetTeamByID(teamId);
            
            // Iterate through hardcoded three battle groups
            for (int g = 0; g < 3; g++)
            {
                //Store ships in a list for a second
                List<Ship> ships = new List<Ship>();
                
                // get all from prototypes where t.group == g
                var groupPrototypes = t.prototypes.Where(p => p.group == g);

                foreach (var groupPrototype in groupPrototypes)
                {
                    // For each ship, instantiate for current team
                        Ship s = ServerCreateShipForTeam(teamId, groupPrototype.group, groupPrototype.shipType);
                        ships.Add(s);

                        if (groupPrototype.hasCaptain)
                            s.ServerSetCaptain(groupPrototype.captain);
                }

                // get positions back for list of ships from formation manager
                Dictionary<Ship, Vector3> shipPositions = VrShipShipFormationManager.instance.GetFormationPositionWarp(ships);

                // Get list of warp vectors in level
                var warpPoints = Level.currentLevel.warpPoints;
                
                WarpPoint wp = warpPoints.First(w =>
                {
                    return w.teamIndex == teamId && w.groupNumber == g;
                });

                foreach (Ship s in ships)
                {
                    Vector3 pos = wp.transform.localPosition;
                    Quaternion rot = wp.transform.rotation;

                    Matrix4x4 parentMatrix = Matrix4x4.TRS(Vector3.zero, rot, Vector3.one).inverse;

                    Vector3 newPos = parentMatrix.MultiplyPoint3x4(shipPositions[s]);

                    s.shipWarp.ServerInitWarp(pos + newPos, rot);
                }
            }

            // Tick up the number of ready players
            for (int i = 0; i < t.players.Count; i++)
            {
                MatchStateManager.instance.ReadyPlayer();
            }
        }
    }
}