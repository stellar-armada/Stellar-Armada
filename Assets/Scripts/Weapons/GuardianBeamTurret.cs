using SpaceCommander.Pooling;

#pragma warning disable 0649
namespace SpaceCommander.Weapons
{
    public class GuardianBeamTurret : Turret
    {
        protected override void StartFiring()
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
        protected override void StopFiring()
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