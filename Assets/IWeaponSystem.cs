using SpaceCommander.Game;
using UnityEngine;

namespace SpaceCommander.Weapons
{
    public interface IWeaponSystem
    {
        IDamager GetDamager();
        void SetDamager(IDamager damager);

        void Impact(WeaponType type, Vector3 point);
    }
}