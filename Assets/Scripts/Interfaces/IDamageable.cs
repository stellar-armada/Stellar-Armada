using UnityEngine;

namespace SpaceCommander
{
    public interface IDamageable
    {
        IEntity GetOwningEntity();
        GameObject GetGameObject();
        void TakeDamage(float damage, Vector3 point, Damager damager);
    }
}
