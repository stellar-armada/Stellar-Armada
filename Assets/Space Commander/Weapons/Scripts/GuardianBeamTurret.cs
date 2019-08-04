using System;
using UnityEngine;
using System.Collections.Generic;
using SpaceCommander.Pooling;

namespace SpaceCommander.Weapons
{
    public class GuardianBeamTurret : Turret
    {
        public override void StartFiring()
        {
            for (var i = 0; i < TurretSocket.Length; i++)
            {
                PoolManager.Pools["GeneratedPool"].SpawnDamager(this, WeaponPrefabManager.instance.guardianBeam, TurretSocket[i].position,
                    TurretSocket[i].rotation,
                    TurretSocket[i]);
            }

            WeaponAudioController.instance.GuardianBeamLoop(transform.position, transform);
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
            WeaponAudioController.instance.GuardianBeamClose(transform.position);

        }
    }
}