using UnityEngine;

#pragma warning disable 0649
namespace SpaceCommander.Weapons
{
    public class ShotgunTurret : Turret
    {
        public override void Impact(Vector3 point)
        {
            Impact(point, WeaponType.Shotgun);
        }

        protected override void StartFiring()
        {
            timerID = TimeManager.instance.AddTimer(.3f, Fire);
            Fire();
        }

        void Fire()
        {
            Fire(WeaponType.Shotgun);
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