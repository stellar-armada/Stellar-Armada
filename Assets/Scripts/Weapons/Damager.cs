using UnityEngine;
#pragma warning disable 0649
namespace SpaceCommander
{
    public abstract class Damager : MonoBehaviour
    {
        public IWeaponSystem owningWeaponSystem;
        
        public LayerMask layerMask;
        
        [SerializeField] float impactHitSize;


        public void SetOwningWeaponSystem(IWeaponSystem weaponSystem)
        {
            owningWeaponSystem = weaponSystem;
        }

        public IWeaponSystem GetOwningWeaponSystem()
        {
            return owningWeaponSystem;
        }

        public float GetImpactSize()
        {
            return impactHitSize;
        }
    }
    
}

