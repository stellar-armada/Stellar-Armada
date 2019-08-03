using UnityEngine;

namespace SpaceCommander.Weapons
{

    public class WeaponEffectController : MonoBehaviour
    {
        // Singleton instance
        public static WeaponEffectController instance;

        [Header("Vulcan")] public Transform vulcanProjectile; // Projectile prefab
        public Transform vulcanMuzzle; // Muzzle flash prefab  
        public Transform vulcanImpact; // Impact prefab

        public float VulcanFireRate = 0.07f;

        [Header("Sniper")] public Transform sniperBeam;
        public Transform sniperMuzzle;
        public Transform sniperImpact;

        [Header("Shotgun")] public Transform shotGunProjectile;
        public Transform shotGunMuzzle;
        public Transform shotGunImpact;

        [Header("Lightning gun")] public Transform lightningGunBeam;
        public float lightingGunBeamOffset;

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