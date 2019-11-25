using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Ships
{
    // Interface for any object that has health and can take damage (i.e. shields, hull, destructable environment)
    public interface IDamageable
    {
        Ship GetOwningEntity();
        GameObject GetGameObject();
        void TakeDamage(float damage, Vector3 point, Damager damager);
    }
}
