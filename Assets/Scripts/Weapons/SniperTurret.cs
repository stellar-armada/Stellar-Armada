using UnityEngine;
using StellarArmada.Pooling;

#pragma warning disable 0649
namespace StellarArmada.Weapons
{
    public class SniperTurret : Turret
    {
        public override void Impact(Vector3 point)
        {
            PoolManager.Pools["GeneratedPool"].Spawn(weaponPrefabManager.GetWeaponPrefab(WeaponType.Sniper).impact, point, Quaternion.identity, null);
            weaponAudioController.PlayHitAtPosition(WeaponType.Sniper, point);
        }
        protected override void StartFiring()
        {
            timerID = timeManager.AddTimer(0.3f, Fire);
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
                timeManager.RemoveTimer(timerID);
                timerID = -1;
            }
        }
    }
}