using UnityEngine;
using SpaceCommander.Pooling;

namespace SpaceCommander.Weapons
{
    public class ShotgunTurret : Turret
    {
        public override void Impact(Vector3 point)
        {
            // Spawn impact prefab at specified position
            PoolManager.Pools["GeneratedPool"].Spawn(WeaponPrefabManager.instance.shotGunImpact, point,
                Quaternion.identity, null);
            // Play impact sound effect
            WeaponAudioController.instance.PlayHitAtPosition(WeaponType.ShotGun, point);
        }

        public override void StartFiring()
        {
            timerID = TimeManager.instance.AddTimer(.3f, Fire);
            Fire();
        }

        void Fire()
        {
            Fire(WeaponType.ShotGun);
        }

        // Stop firing 
        public override void StopFiring()
        {
            // Remove firing timer
            if (timerID != -1)
            {
                TimeManager.instance.RemoveTimer(timerID);
                timerID = -1;
            }
        }
    }
}