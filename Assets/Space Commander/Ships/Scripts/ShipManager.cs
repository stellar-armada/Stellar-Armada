using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

namespace SpaceCommander.Ships
{
    public class ShipManager : NetworkBehaviour
    {
        public static Dictionary<uint, Ship> Ships = new Dictionary<uint,Ship>();

        public static ShipManager instance;
        
        static Ship s;

        void Awake()
        {
            instance = this;
        }
        
        public void RegisterShip(Ship ship)
        {
            Ships.Add(ship.netId, ship);
        }

        public void UnregisterShip(Ship ship)
        {
            Ships.Remove(ship.netId);
        }

        public static List<Ship> GetShips() => Ships.Values.ToList();


        public static Ship GetShipByNetId(uint netID)
        {
            bool success = Ships.TryGetValue(netID, out s);
            if (success) return s;
            Debug.LogError("Couldn't find ship");
            return null;
        }
    }
}