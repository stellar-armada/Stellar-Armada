using UnityEngine;
using StellarArmada.Pooling;

#pragma warning disable 0649
namespace StellarArmada.Weapons
{
    public class VulcanTurret : Turret
    {
        public override void Impact(Vector3 point)
        {
                // Spawn impact prefab at specified position
                PoolManager.Pools["GeneratedPool"].Spawn(weaponPrefabManager.GetWeaponPrefab(WeaponType.Vulcan).impact, point, Quaternion.identity, null);
                // Play impact sound effect
                weaponAudioController.PlayHitAtPosition(WeaponType.Vulcan, point);
        }
        protected override void StartFiring()
        {
            timerID = timeManager.AddTimer(fireRate, Fire);
        }

        void Fire()
        {
            Fire(WeaponType.Vulcan);
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