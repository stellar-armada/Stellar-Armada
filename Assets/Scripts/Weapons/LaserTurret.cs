using UnityEngine;
using SpaceCommander.Pooling;

#pragma warning disable 0649
namespace SpaceCommander.Weapons
{
    public class LaserTurret : Turret
    {
        public override void Impact(Vector3 point)
        {
            PoolManager.Pools["GeneratedPool"].Spawn(WeaponPrefabManager.instance.GetWeaponPrefab(WeaponType.LaserImpulse).impact, point, Quaternion.identity, null);
            WeaponAudioController.instance.PlayHitAtPosition(WeaponType.LaserImpulse, point);
        }
        protected override void StartFiring()
        {
            timerID = TimeManager.instance.AddTimer(fireRate, Fire);
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
                TimeManager.instance.RemoveTimer(timerID);
                timerID = -1;
            }
        }
    }
}