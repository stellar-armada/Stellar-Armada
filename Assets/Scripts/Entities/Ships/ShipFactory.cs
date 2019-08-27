using Mirror;
using StellarArmada.Levels;
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
    }
}