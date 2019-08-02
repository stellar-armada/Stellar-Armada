using UnityEngine;

namespace SpaceCommander
{
    public interface IDamageable
    {
        IPlayer GetPlayer();
        void SetPlayer(IPlayer player);
        GameObject GetGameObject();
        void CmdTakeDamage(float damage);
    }
}
