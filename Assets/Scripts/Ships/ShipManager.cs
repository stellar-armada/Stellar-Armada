using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Ships
{
    public class ShipManager : NetworkBehaviour
    {
        public static Dictionary<uint, Ship> Entities = new Dictionary<uint, Ship>();

        public static ShipManager instance;
        
        static Ship n;

        void Awake()
        {
            instance = this;
        }
        
        public void RegisterEntity(Ship ship)
        {
            Entities.Add(ship.GetEntityId(), ship);
        }

        public void UnregisterEntity(Ship ship)
        {
            Entities.Remove(ship.GetEntityId());
        }

        public static List<Ship> GetEntities() => Entities.Values.ToList();


        public static Ship GetEntityById(uint Id)
        {
            bool success = Entities.TryGetValue(Id, out n);
            if (success) return n;
            Debug.LogError("Couldn't find entity in dictionary");
            return null;
        }
    }
}