using SpaceCommander.Ships;
using UnityEngine;

#pragma warning disable 0649
namespace SpaceCommander.Common.Tests
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
            Debug.Log("Dummy destroyed!");
        }

        public void HullChanged()
        {
            Debug.Log("Hull changed to " + health.currentHull);
        }

        public void ShieldChanged()
        {
            Debug.Log("Hull changed to " + shield.currentShield);
        }
    }
}