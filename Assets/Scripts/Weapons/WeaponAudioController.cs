using System;
using UnityEngine;
using SpaceCommander.Pooling;
using Random = UnityEngine.Random;

[Serializable]
public class EnumeratedAudioClip
{
    
    public AudioClip shotPrefab;
    public AudioClip[] hitPrefabs;
    public float shotDelay; // in ms
    public float hitDelay; // in ms
}

#pragma warning disable 0649
namespace SpaceCommander.Weapons
{
    public class WeaponAudioController : MonoBehaviour
    {
        [SerializeField] WeaponAudioClipDictionary weaponAudioClips = new WeaponAudioClipDictionary();

        // Singleton instance 
        public static WeaponAudioController instance;

        // Audio timers
        float shotTimer, hitTimer;
        public Transform audioSource;
        
        [Header("Guardian Beam")] 
        public AudioClip guardianBeamOpen;
        public AudioClip guardianBeamLoop;
        public AudioClip guardianBeamClose;

        void Awake()
        {
            // Initialize singleton
            instance = this;
        }

        void Update()
        {
            // Update timers
            shotTimer += Time.deltaTime;
            hitTimer += Time.deltaTime;
        }

        public void PlayShotAtPosition(WeaponType weaponType, Vector3 pos)
        {
            // Audio source can only be played once for each vulcanDelay
            if (shotTimer >= weaponAudioClips[weaponType].shotDelay)
            {
                // Spawn audio source prefab from pool
                AudioSource aSrc =
                    PoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, weaponAudioClips[weaponType].shotPrefab, pos, null)
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
                    shotTimer = 0f;
                }
            }
        }
        
        public void PlayHitAtPosition(WeaponType weaponType, Vector3 pos)
        {
            // Audio source can only be played once for each vulcanDelay
            if (hitTimer >= weaponAudioClips[weaponType].hitDelay)
            {
                // Spawn audio source prefab from pool
                AudioSource aSrc =
                    PoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource,
                            weaponAudioClips[weaponType].hitPrefabs[Random.Range(0, weaponAudioClips[weaponType].hitPrefabs.Length)],
                            pos, null)
                        .gameObject.GetComponent<AudioSource>();

                if (aSrc != null)
                {
                    // Modify audio source settings specific to it's type
                    aSrc.pitch = Random.Range(0.95f, 1f);
                    aSrc.volume = Random.Range(0.6f, 1f);
                    aSrc.minDistance = 7f;
                    aSrc.loop = false;
                    aSrc.Play();

                    // Reset delay timer
                    hitTimer = 0f;
                }
            }
        }


        // Play lightning gun shot and loop audio at specific position
        public void GuardianBeamLoop(Vector3 pos, Transform loopParent)
        {
            AudioSource aOpen =
                PoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, guardianBeamOpen, pos, null)
                    .gameObject.GetComponent<AudioSource>();
            AudioSource aLoop =
                PoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, guardianBeamLoop, pos, loopParent.parent)
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
        public void GuardianBeamClose(Vector3 pos)
        {
            AudioSource aClose =
                PoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, guardianBeamClose, pos, null)
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
    }
}