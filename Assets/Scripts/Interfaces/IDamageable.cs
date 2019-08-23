using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada
{
    public interface IDamageable
    {
        NetworkEntity GetOwningEntity();
        GameObject GetGameObject();
        void TakeDamage(float damage, Vector3 point, Damager damager);
    }
}
