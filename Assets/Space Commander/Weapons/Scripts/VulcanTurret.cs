using UnityEngine;
using SpaceCommander.Pooling;

namespace SpaceCommander.Weapons
{
    public class VulcanTurret : Turret
    {
        public override void Impact(Vector3 point)
        {
                // Spawn impact prefab at specified position
                PoolManager.Pools["GeneratedPool"].Spawn(WeaponPrefabManager.instance.vulcanImpact, point, Quaternion.identity, null);
                // Play impact sound effect
                WeaponAudioController.instance.VulcanHit(point);
        }
        public override void StartFiring()
        {
            timerID = TimeManager.instance.AddTimer(fireRate, Fire);

            Fire();
         
        }

        public override void Fire()
        {
            // Get random rotation that offset spawned projectile
            var offset = Quaternion.Euler(UnityEngine.Random.onUnitSphere);
            // Spawn muzzle flash and projectile with the rotation offset at current socket position
            PoolManager.Pools["GeneratedPool"].Spawn( WeaponPrefabManager.instance.vulcanMuzzle, TurretSocket[curSocket].position,
                TurretSocket[curSocket].rotation, TurretSocket[curSocket]);
            var newGO =
                PoolManager.Pools["GeneratedPool"].SpawnDamager(this, WeaponPrefabManager.instance.vulcanProjectile,
                    TurretSocket[curSocket].position + TurretSocket[curSocket].forward,
                    offset * TurretSocket[curSocket].rotation, null).gameObject;
            
            // Play shot sound effect
            WeaponAudioController.instance.VulcanShot(TurretSocket[curSocket].position);
            // Advance to next turret socket
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