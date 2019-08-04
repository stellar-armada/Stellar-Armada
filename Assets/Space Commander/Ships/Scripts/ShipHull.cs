using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace SpaceCommander.Ships
{
    public class ShipHull : NetworkBehaviour, IDamageable
    {
        private IPlayerEntity owningEntity;

        public float maxHull;

        public UnityEvent HullChanged;

        [SyncVar (hook=nameof(HandleHullChange))] public float currentHull;

        void Awake()
        {
            currentHull = maxHull;
        }
        
        [Command]
        public void CmdRepairHull(float amount)
        {
            currentHull = Mathf.Min(currentHull + amount, maxHull);
        }


        public IPlayerEntity GetOwningEntity()
        {
            return owningEntity;
        }

        public void SetOwningEntity(IPlayerEntity playerEntity)
        {
            owningEntity = playerEntity;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public void TakeDamage(float damage, Vector3 point, Damager damager)
        {
            if (owningEntity.IsAlive() && isServer) CmdTakeDamage(damage);
            // We can spawn burn decals here later
        }

        [Command]
        void CmdTakeDamage(float damage)
        {
            currentHull -= damage;
                if (currentHull <= 0 && owningEntity.IsAlive())
                {
                    owningEntity.CmdDie();
                }
        }
        void HandleHullChange(float h)
        {
            HullChanged.Invoke();
        }
    }  
}

