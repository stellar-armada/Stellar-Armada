using SpaceCommander.Game;
using UnityEngine;

namespace SpaceCommander
{
    public interface IWeaponSystem
    {
        IPlayer GetPlayer();
        void SetPlayer(IPlayer player);
        float GetDamage();
        void StartFiring();
        void StopFiring();
        void Fire();
        void Impact(Vector3 point);
    }
}