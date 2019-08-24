using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

#pragma warning disable 0649
namespace StellarArmada.Ships
{
    public class TestDamageableHull : MonoBehaviour, IDamageable
    {
        private NetworkEntity _owningNetworkEntity;

        public float maxHull;
        public bool isDead;

        public UnityEvent ShipDestroyed;
        public UnityEvent HullChanged;

        [FormerlySerializedAs("hull")] public float currentHull;

        void Awake()
        {
            currentHull = maxHull;

        }
        
        public void CmdRepairHull(float amount)
        {
            currentHull = Mathf.Min(currentHull + amount, maxHull);
        }


        public NetworkEntity GetOwningEntity()
        {
            return _owningNetworkEntity;
        }

        public void SetOwningEntity(NetworkEntity playerNetworkEntity)
        {
            _owningNetworkEntity = playerNetworkEntity;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public void TakeDamage(float damage, Vector3 point, Damager damager)
        {
            if (isDead) return;
            CmdTakeDamage(damage);
            HullChanged.Invoke();
        }

        void CmdTakeDamage(float damage)
        {
            currentHull -= damage;
                if (currentHull <= 0 && !isDead)
                {
                    CmdDie();
                }
        }

        public void CmdDie()
        {
            isDead = true;
            ShipDestroyed.Invoke();
        }
        
        
    }  
}

