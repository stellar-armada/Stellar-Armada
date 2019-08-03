using UnityEngine;

namespace SpaceCommander
{
    public interface IDamageable
    {
        IPlayerOwnedEntity GetOwnable();
        void SetOwnable(IPlayerOwnedEntity playerOwnedEntity);
        GameObject GetGameObject();
        void TakeDamage(float damage);
    }
}
