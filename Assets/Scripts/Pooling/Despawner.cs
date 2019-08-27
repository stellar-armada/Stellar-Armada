using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Pooling
{
    // Despawns a pooled object after a time (delay)
    // For use with the pooling system and pool manager
    public class Despawner : MonoBehaviour
    {

        public float DespawnDelay; // Despawn delay in ms
        public bool DespawnFixedAmountOfTimeAfterSpawn; // Despawn on mouse up used for beams demo 

        AudioSource aSrc; // Cached audio source component

        void Awake()
        {
            // Get audio source component
            aSrc = GetComponent<AudioSource>();
        }

        // OnSpawned called by pool manager 
        public void OnSpawned()
        {
            // Invokes despawn using timer delay
            if (!DespawnFixedAmountOfTimeAfterSpawn)
                TimeManager.instance.AddTimer(DespawnDelay, 1, DespawnOnTimer);
        }

        // OnDespawned called by pool manager 
        public void OnDespawned()
        {

        }

        // Run required checks for the looping audio source and despawn the game object
        public void DespawnOnTimer()
        {
            if (aSrc != null)
            {
                if (aSrc.loop)
                    DespawnFixedAmountOfTimeAfterSpawn = true;
                else
                {
                    DespawnFixedAmountOfTimeAfterSpawn = false;
                    Despawn();
                }
            }
            else
            {
                Despawn();
            }
        }

        // Despawn game object this script attached to
        public void Despawn()
        {

            PoolManager.Pools["GeneratedPool"].Despawn(transform);
        }
    }
}