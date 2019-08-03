using SpaceCommander.Teams;
using UnityEngine;
namespace SpaceCommander
{
    public class Damager : MonoBehaviour, IDamager
    {
        public IWeaponSystem owningWeaponSystem;
        
        public LayerMask layerMask;

        public IWeaponSystem GetWeaponSystem()
        {
            return owningWeaponSystem;
        }

        public void Damage(IDamageable target)
        {
            target.TakeDamage(owningWeaponSystem.GetDamage());
        }

        public void SetOwningWeaponSystem(IWeaponSystem weaponSystem)
        {
            owningWeaponSystem = weaponSystem;
        }
    }
    
}

