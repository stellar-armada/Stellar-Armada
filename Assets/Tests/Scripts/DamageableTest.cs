using StellarArmada.Ships;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Common.Tests
{
    public class DamageableTest : MonoBehaviour
    {
        [SerializeField] TestDamageableHull health;
        [SerializeField] TestDamageableShield shield;

        
        [SerializeField] private bool isEnemy;

        void Awake()
        {
            health.SetOwningEntity(GetComponent<NetworkEntity>());
            shield.SetOwningEntity(GetComponent<NetworkEntity>());
        }

        public void ShipDestroyed()
        {
        }

        public void HullChanged()
        {
        }

        public void ShieldChanged()
        {
        }
    }
}