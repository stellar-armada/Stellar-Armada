using SpaceCommander.Teams;
using UnityEngine;
namespace SpaceCommander
{
    public class Damager : MonoBehaviour, IDamager
    {
        public IWeaponSystem owningWeaponSystem;
        
        public LayerMask layerMask;

        public IPlayer GetPlayer()
        {
            return owningWeaponSystem.GetPlayer();
        }
        
        public Team GetTeam()
        {
            return owningWeaponSystem.GetPlayer().GetTeam();
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

