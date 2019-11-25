using StellarArmada.Ships;
using StellarArmada.Levels;
using UnityEngine;
using StellarArmada.Pooling;

#pragma warning disable 0649
namespace StellarArmada.Weapons
{
    public class Projectile : Damager, ISpawnable
    {
        public float lifeTime = 5f; // Projectile life time
        public float despawnDelay; // Delay despawn in ms
        float scaledVelocity = 300f; // Projectile velocity
        public float velocity = 300f; // Start velocy before scaling by scale manager
        public float RaycastAdvance = 2f; // Raycast advance multiplier 
        public bool DelayDespawn = false; // Projectile despawn flag 
        public ParticleSystem[] delayedParticles; // Array of delayed particles
        ParticleSystem[] particles; // Array of projectile particles 
        new Transform transform; // Cached transform 
        RaycastHit hitPoint; // Raycast structure 
        bool isHit = false; // Projectile hit flag
        bool isFXSpawned = false; // Hit FX prefab spawned flag 
        float timer = 0f; // Projectile timer
        float fxOffset; // Offset of fxImpact

        [SerializeField] private Transform miniMapRepresentation; // Copy of the projectile that displays on the minimap

        void Awake()
        {
            // Cache transform and get all particle systems attached
            transform = GetComponent<Transform>();
            particles = GetComponentsInChildren<ParticleSystem>();
            transform.SetParent(LevelRoot.instance.transform);
            scaledVelocity = velocity;
        }

        // OnSpawned called by pool manager 
        public void OnSpawned()
        {
            transform.SetParent(LevelRoot.instance.transform);
            // Reset flags and raycast structure
            isHit = false;
            isFXSpawned = false;
            timer = 0f;
            hitPoint = new RaycastHit();
            miniMapRepresentation.gameObject.SetActive(true);
            miniMapRepresentation.SetParent(VRMiniMap.instance.transform);
            miniMapRepresentation.localScale = transform.lossyScale;
        }

        // OnDespawned called by pool manager 
        public void OnDespawned()
        {
            OnProjectileDestroy();
        }

        // Stop attached particle systems emission and allow them to fade out before despawning
        void Delay()
        {
            if (particles.Length > 0 && delayedParticles.Length > 0)
            {
                bool delayed;
                for (int i = 0; i < particles.Length; i++)
                {
                    delayed = false;
                    for (int y = 0; y < delayedParticles.Length; y++)
                        if (particles[i] == delayedParticles[y])
                        {
                            delayed = true;
                            break;
                        }

                    particles[i].Stop(false);
                    if (!delayed)
                        particles[i].Clear(false);
                }
            }
        }

        // OnDespawned called by pool manager 
        void OnProjectileDestroy()
        {
            miniMapRepresentation.gameObject.SetActive(false);
            miniMapRepresentation.SetParent(transform);
            PoolManager.Pools["GeneratedPool"].Despawn(transform);
        }

        void HandleHit()
        {
            // Refactor to be a damageable thing
                ICollidable collidable = hitPoint.transform.GetComponent<ICollidable>();
                if (collidable == null)
                {
                    Debug.LogError("Couldn't find damagable on " + hitPoint.transform.name);
                    return;
                }

                collidable.GetDamageable().TakeDamage(owningWeaponSystem.GetDamage(), hitPoint.point, this);

                //TO-DO: Make this function call way less disgusting

                // Execute once
                if (!isFXSpawned)
                {
                    owningWeaponSystem.Impact(hitPoint.point);
                    isFXSpawned = true;
                }

                // Despawn current projectile 
                if (!DelayDespawn || (DelayDespawn && (timer >= despawnDelay)))
                    OnProjectileDestroy();
        }

        void StepForward()
        {
            // Projectile step per frame based on velocity and time
            Vector3 step = transform.forward * Time.deltaTime * scaledVelocity;

            // Raycast for targets with ray length based on frame step by ray cast advance multiplier
            if (Physics.Raycast(transform.position, transform.forward, out hitPoint,
                step.magnitude * RaycastAdvance,
                layerMask))
            {
                isHit = true;

                // Invoke delay routine if required
                if (DelayDespawn)
                {
                    // Reset projectile timer and let particles systems stop emitting and fade out correctly
                    timer = 0f;
                    Delay();
                }
            }
            // Nothing hit
            else
            {
                // Projectile despawn after run out of time
                if (timer >= lifeTime)
                    OnProjectileDestroy();
            }

            // Advances projectile forward
            transform.position += step;
            miniMapRepresentation.localPosition = transform.position;
            miniMapRepresentation.localRotation = transform.rotation;
        }
        
        void Update()
        {
            if (isHit)
                HandleHit();

            // No collision occurred yet
            else
                StepForward();

                // Updates projectile timer
                timer += Time.deltaTime;
        }
    }
}