using System;
using UnityEngine;
using System.Collections.Generic;
using SpaceCommander.Pooling;

namespace SpaceCommander.Weapons
{
    public class LightningTurret : Turret
    {
        public override void Impact(Vector3 point)
        {
            PoolManager.Pools["GeneratedPool"].Spawn(WeaponPrefabManager.instance.sniperImpact, point, Quaternion.identity, null);
            WeaponAudioController.instance.SniperHit(point);
        }
        public override void StartFiring()
        {
            for (var i = 0; i < TurretSocket.Length; i++)
            {
                PoolManager.Pools["GeneratedPool"].SpawnDamager(this, WeaponPrefabManager.instance.lightningGunBeam, TurretSocket[i].position,
                    TurretSocket[i].rotation,
                    TurretSocket[i]);
            }

            WeaponAudioController.instance.LightningGunLoop(transform.position, transform);
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
            WeaponAudioController.instance.LightningGunClose(transform.position);

        }
    }
}