using Mirror;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Entities
{
    public class EntityHull : NetworkBehaviour, IDamageable, ICollidable
    {
        [SerializeField] private NetworkEntity entity;

        public float maxHull;
        
        public delegate void HullChangeEvent(float shieldVal);

        public HullChangeEvent HullChanged;
        
        [SyncVar] public float currentHull;

        void Awake()
        {
            currentHull = maxHull;
        }

        [Command]
        public void CmdRepairHull(float amount)
        {
            currentHull = Mathf.Min(currentHull + amount, maxHull);
        }

        public NetworkEntity GetOwningEntity()
        {
            return entity;
        }


        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public void TakeDamage(float damage, Vector3 point, Damager damager)
        {
            if (!entity.IsAlive())
            {
                return;
            }

            currentHull -= damage;
            if (currentHull <= 0 && entity.IsAlive())
            {
                    entity.CmdDie();
            }

            HullChanged.Invoke(currentHull);
        }

        public IDamageable GetDamageable()
        {
            return this;
        }

    }
}