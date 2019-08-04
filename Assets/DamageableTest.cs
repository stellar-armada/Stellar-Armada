using SpaceCommander.Ships;
using UnityEngine;

namespace SpaceCommander.Common.Tests
{
    public class DamageableTest : MonoBehaviour, IDamageable
    {
        public IPlayerEntity owningEntity;
        
        [SerializeField] TestDamageable health;

        [SerializeField] private bool isEnemy;

        void Awake()
        {
            health.SetOwningEntity(GetComponent<IPlayerEntity>());
        }

        public void ShipDestroyed()
        {
            Debug.Log("Dummy destroyed!");
        }

        public void HullChanged()
        {
            Debug.Log("Hull changed to " + health.hull);
        }

        public void ShieldChanged()
        {
            Debug.Log("Hull changed to " + health.shield);
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

        public void TakeDamage(float damage)
        {
            health.TakeDamage(damage);
        }
    }
}