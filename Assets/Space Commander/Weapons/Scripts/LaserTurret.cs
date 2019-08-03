using System;
using UnityEngine;
using System.Collections.Generic;
using SpaceCommander.Pooling;

namespace SpaceCommander.Weapons
{
    public class LaserTurret : Turret
    {
        public override void Impact(Vector3 point)
        {
            PoolManager.Pools["GeneratedPool"].Spawn(WeaponPrefabManager.instance.laserImpulseImpact, point, Quaternion.identity, null);
            WeaponAudioController.instance.LaserImpulseHit(point);
        }
        public override void StartFiring()
        {
            timerID = TimeManager.instance.AddTimer(fireRate, Fire);
            Fire();
        }

        void Fire()
        {
            var offset = Quaternion.Euler(UnityEngine.Random.onUnitSphere);
            PoolManager.Pools["GeneratedPool"].Spawn(WeaponPrefabManager.instance.laserImpulseMuzzle, TurretSocket[curSocket].position,
                TurretSocket[curSocket].rotation, TurretSocket[curSocket]);
            var newGO =
                PoolManager.Pools["GeneratedPool"].SpawnDamager(this, WeaponPrefabManager.instance.laserImpulseProjectile, TurretSocket[curSocket].position,
                    offset * TurretSocket[curSocket].rotation, null).gameObject;
            WeaponAudioController.instance.LaserImpulseShot(TurretSocket[curSocket].position);

            AdvanceSocket();
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