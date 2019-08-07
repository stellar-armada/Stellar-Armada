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
        void Impact(Vector3 point);
        void ClearTarget();
        GameObject GetGameObject();
    }
}