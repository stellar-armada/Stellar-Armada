using System;
using UnityEngine;
using System.Collections.Generic;
using SpaceCommander.Pooling;

namespace SpaceCommander.Weapons
{
    public class ShotgunTurret : Turret
    {
        public override void Impact(Vector3 point)
        {
                // Spawn impact prefab at specified position
                PoolManager.Pools["GeneratedPool"].Spawn(WeaponPrefabManager.instance.shotGunImpact, point, Quaternion.identity, null);
                // Play impact sound effect
                WeaponAudioController.instance.VulcanHit(point);
        }
        public override void StartFiring()
        {
            timerID = TimeManager.instance.AddTimer(.3f, Fire);
            Fire();

        }

        public override void Fire()
        {
            var offset = Quaternion.Euler(UnityEngine.Random.onUnitSphere);
            PoolManager.Pools["GeneratedPool"].Spawn( WeaponPrefabManager.instance.shotGunMuzzle, TurretSocket[curSocket].position,
                TurretSocket[curSocket].rotation, TurretSocket[curSocket]);
            PoolManager.Pools["GeneratedPool"].SpawnDamager(this, WeaponPrefabManager.instance.shotGunProjectile, TurretSocket[curSocket].position,
                offset * TurretSocket[curSocket].rotation, null);
            WeaponAudioController.instance.ShotGunShot(TurretSocket[curSocket].position);
            
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