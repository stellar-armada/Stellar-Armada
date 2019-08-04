using UnityEngine;
using UnityEngine.Serialization;

namespace SpaceCommander.Weapons
{

    public class WeaponPrefabManager : MonoBehaviour
    {
        // Singleton instance
        public static WeaponPrefabManager instance;

        [Header("Vulcan")] public Transform vulcanProjectile; // Projectile prefab
        public Transform vulcanMuzzle; // Muzzle flash prefab  
        public Transform vulcanImpact; // Impact prefab
        
        [Header("Sniper")] public Transform sniperBeam;
        public Transform sniperMuzzle;
        public Transform sniperImpact;

        [Header("Shotgun")] public Transform shotGunProjectile;
        public Transform shotGunMuzzle;
        public Transform shotGunImpact;

        [FormerlySerializedAs("lightningGunBeam")] [Header("Lightning gun")] public Transform guardianBeam;

        [Header("Laser impulse")] public Transform laserImpulseProjectile;
        public Transform laserImpulseMuzzle;
        public Transform laserImpulseImpact;

        private void Awake()
        {
            // Initialize singleton  
            instance = this;
        }
    }
}