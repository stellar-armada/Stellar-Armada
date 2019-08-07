using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace SpaceCommander.Ships
{
    public class TestDamageableHull : MonoBehaviour, IDamageable
    {
        private IEntity owningEntity;

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
            Debug.Log("Hull repaired to " + currentHull);
        }


        public IEntity GetOwningEntity()
        {
            return owningEntity;
        }

        public void SetOwningEntity(IEntity playerEntity)
        {
            owningEntity = playerEntity;
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

