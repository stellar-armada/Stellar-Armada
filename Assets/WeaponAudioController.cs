using UnityEngine;
using System.Collections;
using SpaceCommander.Pooling;

namespace SpaceCommander.Weapons
{
    public class WeaponAudioController : MonoBehaviour
    {
        // Singleton instance 
        public static WeaponAudioController instance;

        // Audio timers
        float timer_01, timer_02;
        public Transform audioSource;
        [Header("Vulcan")] public AudioClip[] vulcanHit; // Impact prefabs array  
        public AudioClip vulcanShot; // Shot prefab
        public float vulcanDelay; // Shot delay in ms
        public float vulcanHitDelay; // Hit delay in ms

        [Header("Sniper")] public AudioClip[] sniperHit;
        public AudioClip sniperShot;
        public float sniperDelay;
        public float sniperHitDelay;

        [Header("Shot gun")] public AudioClip[] shotGunHit;
        public AudioClip shotGunShot;
        public float shotGunDelay;
        public float shotGunHitDelay;

        [Header("Lightning gun")] public AudioClip lightningGunOpen;
        public AudioClip lightningGunLoop;
        public AudioClip lightningGunClose;

        [Header("Laser impulse")] public AudioClip[] laserImpulseHit;
        public AudioClip laserImpulseShot;
        public float laserImpulseDelay;
        public float laserImpulseHitDelay;

        void Awake()
        {
            // Initialize singleton
            instance = this;
        }

        void Update()
        {
            // Update timers
            timer_01 += Time.deltaTime;
            timer_02 += Time.deltaTime;
        }

        // Play vulcan shot audio at specific position
        public void VulcanShot(Vector3 pos)
        {
            // Audio source can only be played once for each vulcanDelay
            if (timer_01 >= vulcanDelay)
            {
                // Spawn audio source prefab from pool
                AudioSource aSrc =
                    PoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, vulcanShot, pos, null)
                        .gameObject.GetComponent<AudioSource>();

                if (aSrc != null)
                {
                    // Modify audio source settings specific to it's type
                    aSrc.pitch = Random.Range(0.95f, 1f);
                    aSrc.volume = Random.Range(0.8f, 1f);
                    aSrc.minDistance = 5f;
                    aSrc.loop = false;
                    aSrc.Play();

                    // Reset delay timer
                    timer_01 = 0f;
                }
            }
        }

        // Play vulcan hit audio at specific position
        public void VulcanHit(Vector3 pos)
        {
            if (timer_02 >= vulcanHitDelay)
            {
                // Spawn random hit audio prefab from pool for specific weapon type
                AudioSource aSrc =
                    PoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource,
                        vulcanHit[Random.Range(0, vulcanHit.Length)], pos, null).gameObject.GetComponent<AudioSource>();
                if (aSrc != null)
                {
                    aSrc.pitch = Random.Range(0.95f, 1f);
                    aSrc.volume = Random.Range(0.6f, 1f);
                    aSrc.minDistance = 7f;
                    aSrc.loop = false;
                    aSrc.Play();

                    timer_02 = 0f;
                }
            }
        }

        // Play sniper shot audio at specific position
        public void SniperShot(Vector3 pos)
        {
            if (timer_01 >= sniperDelay)
            {
                AudioSource aSrc =
                    PoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, sniperShot, pos, null)
                        .gameObject.GetComponent<AudioSource>();

                if (aSrc != null)
                {
                    aSrc.pitch = Random.Range(0.9f, 1f);
                    aSrc.volume = Random.Range(0.8f, 1f);
                    aSrc.minDistance = 6f;
                    aSrc.loop = false;

                    aSrc.Play();

                    timer_01 = 0f;
                }
            }
        }

        // Play sniper hit audio at specific position
        public void SniperHit(Vector3 pos)
        {
            if (timer_02 >= sniperHitDelay)
            {
                AudioSource aSrc =
                    PoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource,
                        sniperHit[Random.Range(0, sniperHit.Length)], pos, null).gameObject.GetComponent<AudioSource>();
                if (aSrc != null)
                {
                    aSrc.pitch = Random.Range(0.9f, 1f);
                    aSrc.volume = Random.Range(0.8f, 1f);
                    aSrc.minDistance = 8f;
                    aSrc.loop = false;
                    aSrc.Play();

                    timer_02 = 0f;
                }
            }
        }

        // Play shotgun shot audio at specific position
        public void ShotGunShot(Vector3 pos)
        {
            if (timer_01 >= shotGunDelay)
            {
                AudioSource aSrc =
                    PoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, shotGunShot, pos, null)
                        .gameObject.GetComponent<AudioSource>();

                if (aSrc != null)
                {
                    aSrc.pitch = Random.Range(0.9f, 1f);
                    aSrc.volume = Random.Range(0.8f, 1f);
                    aSrc.minDistance = 8f;
                    aSrc.loop = false;

                    aSrc.Play();

                    timer_01 = 0f;
                }
            }
        }

        // Play shotgun hit audio at specific position
        public void ShotGunHit(Vector3 pos)
        {
            if (timer_02 >= shotGunHitDelay)
            {
                AudioSource aSrc =
                    PoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource,
                        shotGunHit[Random.Range(0, shotGunHit.Length)], pos, null)
                        .gameObject.GetComponent<AudioSource>();
                if (aSrc != null)
                {
                    aSrc.pitch = Random.Range(0.9f, 1f);
                    aSrc.volume = Random.Range(0.8f, 1f);
                    aSrc.minDistance = 7f;
                    aSrc.loop = false;
                    aSrc.Play();

                    timer_02 = 0f;
                }
            }
        }

        // Play lightning gun shot and loop audio at specific position
        public void LightningGunLoop(Vector3 pos, Transform loopParent)
        {
            AudioSource aOpen =
                PoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, lightningGunOpen, pos, null)
                    .gameObject.GetComponent<AudioSource>();
            AudioSource aLoop =
                PoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, lightningGunLoop, pos, loopParent.parent)
                    .gameObject.GetComponent<AudioSource>();


            if (aOpen != null && aLoop != null)
            {
                aOpen.pitch = Random.Range(0.8f, 1f);
                aOpen.volume = Random.Range(0.8f, 1f);
                aOpen.minDistance = 50f;
                aOpen.loop = false;
                aOpen.Play();

                aLoop.pitch = Random.Range(0.95f, 1f);
                aLoop.volume = Random.Range(0.95f, 1f);
                aLoop.loop = true;
                aLoop.minDistance = 50f;

                aLoop.Play();
            }
        }

        // Play lightning gun closing audio at specific position
        public void LightningGunClose(Vector3 pos)
        {
            AudioSource aClose =
                PoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, lightningGunClose, pos, null)
                    .gameObject.GetComponent<AudioSource>();

            if (aClose != null)
            {
                aClose.pitch = Random.Range(0.8f, 1f);
                aClose.volume = Random.Range(0.8f, 1f);
                aClose.minDistance = 50f;
                aClose.loop = false;
                aClose.Play();
            }
        }

        // Play laser pulse shot audio at specific position
        public void LaserImpulseShot(Vector3 pos)
        {
            if (timer_01 >= laserImpulseDelay)
            {
                AudioSource aSrc =
                    PoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, laserImpulseShot, pos, null)
                        .gameObject.GetComponent<AudioSource>();

                if (aSrc != null)
                {
                    aSrc.pitch = Random.Range(0.9f, 1f);
                    aSrc.volume = Random.Range(0.8f, 1f);
                    aSrc.minDistance = 20f;
                    aSrc.loop = false;
                    aSrc.Play();

                    timer_01 = 0f;
                }
            }
        }

        // Play laser pulse hit audio at specific position
        public void LaserImpulseHit(Vector3 pos)
        {
            if (timer_02 >= laserImpulseHitDelay)
            {
                AudioSource aSrc =
                    PoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource,
                        laserImpulseHit[Random.Range(0, laserImpulseHit.Length)], pos, null)
                        .gameObject.GetComponent<AudioSource>();

                if (aSrc != null)
                {
                    aSrc.pitch = Random.Range(0.8f, 1f);
                    aSrc.volume = Random.Range(0.8f, 1f);
                    aSrc.minDistance = 20f;
                    aSrc.loop = false;
                    aSrc.Play();

                    timer_02 = 0f;
                }
            }
        }
    }
}