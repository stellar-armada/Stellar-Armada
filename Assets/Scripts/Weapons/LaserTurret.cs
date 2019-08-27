using UnityEngine;
using StellarArmada.Pooling;

#pragma warning disable 0649
namespace StellarArmada.Weapons
{
    public class LaserTurret : Turret
    {
        public override void Impact(Vector3 point)
        {
            PoolManager.Pools["GeneratedPool"].Spawn(weaponPrefabManager.GetWeaponPrefab(WeaponType.LaserImpulse).impact, point, Quaternion.identity, null);
            weaponAudioController.PlayHitAtPosition(WeaponType.LaserImpulse, point);
        }
        protected override void StartFiring()
        {
            timerID = timeManager.AddTimer(fireRate, Fire);
            Fire();
        }

        void Fire()
        {
            Fire(WeaponType.LaserImpulse);
        }

        // Stop firing 
        protected override void StopFiring()
        {
            // Remove firing timer
            if (timerID != -1)
            {
                timeManager.RemoveTimer(timerID);
                timerID = -1;
            }
        }
    }
}