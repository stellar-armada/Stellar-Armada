using SpaceCommander.Game;
using UnityEngine;

namespace SpaceCommander
{
    public interface IWeaponSystem
    {
        IWeaponSystemController GetWeaponSystemController();
        void SetWeaponSystemController(IWeaponSystemController weaponSystemController);
        Transform GetTarget();
        void SetTarget(Transform target);
        float GetDamage();
        void StartFiring();
        void StopFiring();
        void Fire();
        void Impact(Vector3 point);
    }
}