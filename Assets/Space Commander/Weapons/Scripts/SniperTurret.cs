using System;
using UnityEngine;
using System.Collections.Generic;
using SpaceCommander.Pooling;

namespace SpaceCommander.Weapons
{
    public class SniperTurret : Turret
    {
        public override void Impact(Vector3 point)
        {
            Debug.Log("Impact called");
            PoolManager.Pools["GeneratedPool"].Spawn(WeaponEffectController.instance.sniperImpact, point, Quaternion.identity, null);
            WeaponAudioController.instance.SniperHit(point);
        }
        public override void StartFiring()
        {
            timerID = TimeManager.instance.AddTimer(0.3f, Fire);
            Fire();
        }

        public override void Fire()
        {
            Debug.Log("Firing " + Time.time);
            var offset = Quaternion.Euler(UnityEngine.Random.onUnitSphere);

            PoolManager.Pools["GeneratedPool"].Spawn(WeaponEffectController.instance.sniperMuzzle, TurretSocket[curSocket].position,
                TurretSocket[curSocket].rotation, TurretSocket[curSocket]);
            PoolManager.Pools["GeneratedPool"].SpawnDamager(this, WeaponEffectController.instance.sniperBeam, TurretSocket[curSocket].position,
                    offset * TurretSocket[curSocket].rotation, null);
            WeaponAudioController.instance.SniperShot(TurretSocket[curSocket].position);
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