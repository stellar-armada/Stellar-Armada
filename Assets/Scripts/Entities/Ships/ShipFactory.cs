using System.Collections.Generic;
using System.Linq;
using Mirror;
using StellarArmada.Levels;
using StellarArmada.Match;
using StellarArmada.Teams;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Entities.Ships
{
    public class ShipFactory : EntityFactory
    {
        public static ShipFactory instance;

        [SerializeField] ShipDictionary ships = new ShipDictionary();

        void Awake()
        {
            instance = this;
        }

// Ship Factory
        [Command]
        public void CmdCreateShipForPlayer(uint playerID, ShipType shipType, Vector3 position, Quaternion rotation)
        {
            GameObject shipPrefab;

            bool success = ships.TryGetValue(shipType, out shipPrefab);
            if (!success)
            {
                Debug.LogError("Failed to create ship - was not found in dictionary");
                return;
            }

            GameObject shipGameObject = Instantiate(shipPrefab, position, rotation, LevelRoot.instance.transform);
            shipGameObject.transform.localScale = Vector3.one;
            NetworkServer.Spawn(shipGameObject);

            Ship s = shipGameObject.GetComponent<Ship>();

            s.CmdSetCaptain(playerID);
        }

        public Ship CreateShipForTeam(uint teamId, int groupId, ShipType shipType)
        {
            if (!isServer) return null;

            GameObject shipPrefab;

            bool success = ships.TryGetValue(shipType, out shipPrefab);
            if (!success)
            {
                Debug.LogError("Failed to create ship - was not found in dictionary");
                return null;
            }

            GameObject shipGameObject = Instantiate(shipPrefab);

            NetworkServer.Spawn(shipGameObject);

            Ship s = shipGameObject.GetComponent<Ship>();

            s.SetEntityId(entityIncrement++);
            s.CmdSetTeam(teamId);

            // Get group from group ID and add
            TeamManager.instance.GetTeamByID(teamId).groups[groupId].Add(s);
            return s;
        }
        
        public void CreateShipsForTeam(uint teamId)
        {
            // Create ships
            Scenario currentScenario = MatchScenarioManager.instance.GetCurrentScenario();
            // Iterate through battle groups
            for (int g = 0; g < currentScenario.teamInfo[teamId].fleetBattleGroups.Count; g++)
            {
                //Store ships in a list for a second
                List<Ship> ships = new List<Ship>();

                foreach (var key in currentScenario.teamInfo[teamId].fleetBattleGroups[g])
                {
                    for (int numShips = 0; numShips < key.Value; numShips++)
                    {
                        // For each ship, instantiate for current team
                        Ship s = ShipFactory.instance.CreateShipForTeam(teamId, g, key.Key);
                        ships.Add(s);
                    }
                }

                // get positions back for list of ships from formation manager
                Dictionary<Ship, Vector3> shipPositions = ShipFormationManager.instance.GetFormationPositionWarp(ships);

                // Get list of warp vectors in level
                var warpPoints = Level.currentLevel.warpPoints;

                Transform wp = warpPoints.Single(w => (w.teamIndex == teamId && w.groupNumber == g)).transform;

                foreach (Ship s in ships)
                {
                    Vector3 pos = wp.localPosition;
                    Quaternion rot = wp.rotation;

                    Matrix4x4 parentMatrix = Matrix4x4.TRS(Vector3.zero, rot, Vector3.one).inverse;

                    Vector3 newPos = parentMatrix.MultiplyPoint3x4(shipPositions[s]);

                    s.shipWarp.InitWarp(pos + newPos, rot);
                }
            }
        }
    }
}