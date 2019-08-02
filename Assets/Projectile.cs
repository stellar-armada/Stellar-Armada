using UnityEngine;
using System.Collections;
using Mirror;
using SpaceCommander.Pooling;
using SpaceCommander.Ships;

namespace SpaceCommander.Weapons
{
    public class Projectile : NetworkBehaviour, IDamager
    {
        public IWeaponSystem owningWeaponSystem;
        public float damage = 1f;
        public WeaponType weaponType; // Weapon type 
        public LayerMask layerMask;
        public float lifeTime = 5f; // Projectile life time
        public float despawnDelay; // Delay despawn in ms
        public float velocity = 300f; // Projectile velocity
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

        void Awake()
        {
            // Cache transform and get all particle systems attached
            transform = GetComponent<Transform>();
            particles = GetComponentsInChildren<ParticleSystem>();
        }

        // OnSpawned called by pool manager 
        public void OnSpawned()
        {
            // Reset flags and raycast structure
            isHit = false;
            isFXSpawned = false;
            timer = 0f;
            hitPoint = new RaycastHit();
        }

        // OnDespawned called by pool manager 
        public void OnDespawned()
        {
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
            PoolManager.Pools["GeneratedPool"].Despawn(transform);
        }

        // Apply hit force on impact
        void ApplyForce(float force)
        {
            if (hitPoint.rigidbody != null)
                hitPoint.rigidbody.AddForceAtPosition(transform.forward*force, hitPoint.point, ForceMode.VelocityChange);
        }

        void Update()
        {
            // If something was hit
            if (isHit)
            {
                if(isServer){
                // Refactor to be a damageable thing
                IDamageable damageable = hitPoint.transform.parent.GetComponent<IDamageable>();
                if (damageable != null)
                    damageable.CmdTakeDamage(damage);
                //TO-DO: Make this function call way less disgusting
                
                
                // Execute once
                if (!isFXSpawned)
                {
                    // Invoke corresponding method that spawns FX
                    switch (weaponType)
                    {
                        case WeaponType.Vulcan:
                            owningWeaponSystem.Impact(WeaponType.Vulcan, hitPoint.point + hitPoint.normal*fxOffset);
                            ApplyForce(2.5f);
                            break;

                        case WeaponType.LaserImpulse:
                            owningWeaponSystem.Impact(WeaponType.LaserImpulse, hitPoint.point + hitPoint.normal*fxOffset);
                            ApplyForce(25f);
                            break; 
                    }

                    isFXSpawned = true;
                }

                // Despawn current projectile 
                if (!DelayDespawn || (DelayDespawn && (timer >= despawnDelay)))
                    OnProjectileDestroy();
            }

            // No collision occurred yet
            else
            {
                // Projectile step per frame based on velocity and time
                Vector3 step = transform.forward*Time.deltaTime*velocity;

                // Raycast for targets with ray length based on frame step by ray cast advance multiplier
                if (Physics.Raycast(transform.position, transform.forward, out hitPoint, step.magnitude*RaycastAdvance,
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
            }

            // Updates projectile timer
            timer += Time.deltaTime;
        }
        }

        public IPlayer GetPlayer()
        {
            throw new System.NotImplementedException();
        }

        public void SetPlayer(IPlayer player)
        {
            throw new System.NotImplementedException();
        }
    }