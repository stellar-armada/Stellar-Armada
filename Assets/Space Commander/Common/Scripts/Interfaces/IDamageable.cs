using UnityEngine;

namespace SpaceCommander
{
    public interface IDamageable
    {
        IPlayerEntity GetOwningEntity();
        void SetOwningEntity(IPlayerEntity playerEntity);
        GameObject GetGameObject();
        void TakeDamage(float damage);
    }
}
