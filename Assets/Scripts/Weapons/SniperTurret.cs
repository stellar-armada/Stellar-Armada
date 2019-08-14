using UnityEngine;
using SpaceCommander.Pooling;

#pragma warning disable 0649
namespace SpaceCommander.Weapons
{
    public class SniperTurret : Turret
    {
        public override void Impact(Vector3 point)
        {
            PoolManager.Pools["GeneratedPool"].Spawn(WeaponPrefabManager.instance.GetWeaponPrefab(WeaponType.Sniper).impact, point, Quaternion.identity, null);
            WeaponAudioController.instance.PlayHitAtPosition(WeaponType.Sniper, point);
        }
        protected override void StartFiring()
        {
            timerID = TimeManager.instance.AddTimer(0.3f, Fire);
        }

        void Fire()
        {
            Fire(WeaponType.Sniper);
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