using UnityEngine;
#pragma warning disable 0649
namespace StellarArmada
{
    // Base class for objects that can cause damage to other objects when hitting them (i.e. projectiles, etc)
    public abstract class Damager : MonoBehaviour
    {
        public WeaponSystem owningWeaponSystem;
        
        public LayerMask layerMask;
        
        [SerializeField] float impactHitSize;


        public void SetOwningWeaponSystem(WeaponSystem weaponSystem)
        {
            owningWeaponSystem = weaponSystem;
        }

        public WeaponSystem GetOwningWeaponSystem()
        {
            return owningWeaponSystem;
        }

        public float GetImpactSize()
        {
            return impactHitSize;
        }
    }
    
}

