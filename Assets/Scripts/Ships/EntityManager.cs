using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Ships
{
    public class EntityManager : NetworkBehaviour
    {
        public static Dictionary<uint, NetworkEntity> Entities = new Dictionary<uint, NetworkEntity>();

        public static EntityManager instance;
        
        static NetworkEntity n;

        void Awake()
        {
            instance = this;
        }
        
        public void RegisterEntity(NetworkEntity entity)
        {
            Entities.Add(entity.GetEntityId(), entity);
        }

        public void UnregisterEntity(NetworkEntity entity)
        {
            Entities.Remove(entity.GetEntityId());
        }

        public static List<NetworkEntity> GetEntities() => Entities.Values.ToList();


        public static NetworkEntity GetEntityById(uint Id)
        {
            bool success = Entities.TryGetValue(Id, out n);
            if (success) return n;
            Debug.LogError("Couldn't find entity in dictionary");
            return null;
        }
    }
}