using System;
using UnityEngine;

#pragma warning disable 0649
namespace SpaceCommander.Weapons
{
    [Serializable]
    public class EnumeratedWeaponPrefab
    {
        public Transform projectile;
        public Transform muzzle;
        public Transform impact;
    }
    public class WeaponPrefabManager : MonoBehaviour
    {
        [SerializeField] private WeaponPrefabDictionary weaponPrefabs = new WeaponPrefabDictionary();

        [Header("Guardian Beam")]
        public Transform guardianBeam;
        
        public EnumeratedWeaponPrefab GetWeaponPrefab(WeaponType type)
        {
            return weaponPrefabs[type];
        }
        
        // Singleton instance
        public static WeaponPrefabManager instance;
        
        private void Awake()
        {
            // Initialize singleton  
            instance = this;
        }
    }
}