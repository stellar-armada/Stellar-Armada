using UnityEngine;
using System.Collections.Generic;
using StellarArmada.Entities;
using StellarArmada.Pooling;

#pragma warning disable 0649
namespace StellarArmada.Weapons
{
    // Base class for all turrets / most ship weapons
    // Most methods are overridable, so this could be extended to missiles, etc.
    public class Turret : WeaponSystem
    {
        [Header("Turret setup")] public Transform[] TurretSocket; // Sockets reference

        public Transform Mount;
        public Transform Swivel;
        public float HeadingTrackingSpeed = 2f;
        public float ElevationTrackingSpeed = 2f;
        public float maximumAngleDifferenceToTarget = 10f;
        public float fireRate = .3f;
        public Vector2 HeadingLimit;
        public Vector2 ElevationLimit;
        public bool smoothControlling;
        public bool DebugDraw;

        // Local references (ugly optimization for GC prevention)
        protected WeaponPrefabManager weaponPrefabManager;
        protected WeaponAudioController weaponAudioController;
        protected TimeManager timeManager;
        Vector3 headingVector;
        private float curHeadingAngle;
        private float curElevationAngle;
        private Vector3 defaultDir;
        private Quaternion defaultRot;
        private Vector3 targetDirection;
        private bool fullAccess;
        RaycastHit hitInfo; // Raycast structure
        private Vector3 targetPos;
        int currentFiringSocket = 0;
        private Collider[] hitColliders;
        private List<IDamageable> damageables;
        private IDamageable d;
        NetworkEntity damaged;
        NetworkEntity damager;
        Vector3 targetX;
        Quaternion targetRotationX;
        Vector3 targetY;
        Quaternion targetRotationY;
        float headingAngle;
        float turretDefaultToTargetAngle;
        float turretHeading;
        float headingStep;
        Vector3 elevationVector;
        float elevationAngle;
        float elevationStep;
        
        private Transform t;

        public override void Awake()
        {
            base.Awake();
            t = transform;
        }
        
        private void Start()
        {
            weaponPrefabManager = WeaponPrefabManager.instance;
            weaponAudioController = WeaponAudioController.instance;
            timeManager = TimeManager.instance;
            
            targetPos = Swivel.position + Swivel.forward * 100f;
            defaultDir = Swivel.transform.forward;
            defaultRot = Quaternion.FromToRotation(t.forward, defaultDir);
            if (HeadingLimit.y - HeadingLimit.x >= 359.9f)
                fullAccess = true;
        }
        
        public override void Impact(Vector3 point)
        {
            PoolManager.Pools["GeneratedPool"].Spawn(weaponPrefabManager.GetWeaponPrefab(WeaponType.LaserImpulse).impact, point,
                Quaternion.identity, null);
            weaponAudioController.PlayHitAtPosition(WeaponType.LaserImpulse, point);
        }


        // Advance to next turret socket
        public void AdvanceSocket()
        {
            currentFiringSocket++;
            if (currentFiringSocket >= TurretSocket.Length)
                currentFiringSocket = 0;
        }


        // Autotrack
        public void SetNewTarget(Vector3 _targetPos)
        {
            targetPos = _targetPos;
        }

        // Angle between mount and target
        public float GetAngleToTarget()
        {
            return Vector3.Angle(Mount.transform.forward, targetPos - Mount.transform.position);
        }
        
        bool IsFacingTarget()
        {
            return (GetAngleToTarget() < maximumAngleDifferenceToTarget);
        }

        // Angle between mount and target
        public bool CanHitPosition()
        {
            if (target == null) return false;

            return CanHitPosition(target.position);
        }

        public bool CanHitPosition(Vector3 pos)
        {
            targetDirection = (pos - Mount.transform.position).normalized;
            
            if(Vector3.Dot(t.forward, targetDirection) > 0.8f) // Magic number .8f = pretty facing
            {
                return true;
            }

            return false;
        }

        void CheckTarget()
        {

            // if current target cant be hit or object isn't within distance
            if (!owningWeaponSystemController.WeaponSystemsEnabled() || target == null || !CanHitPosition() || Vector3.Distance(t.localPosition, target.localPosition) > maxRange)
            {
                ClearTarget();
                isFiring = false;
                StopFiring();
            }
        }



        protected override void AcquireTarget()
        {
            if (!owningWeaponSystemController.WeaponSystemsEnabled()) return;
            // overlap sphere to get list of hitting objects
            hitColliders = Physics.OverlapSphere(t.position, maxRange, damageableLayerMask);

            if (hitColliders.Length < 1)
            {
                Debug.LogError("No colliders found");
                return; // No colliders hit
            }

            // if object is an enemy
            damageables = new List<IDamageable>();
            foreach (Collider col in hitColliders)
            {
                d = col.GetComponent<ICollidable>().GetDamageable();
                if (d == null) Debug.LogError("Damageable was not found on collidable reference on " + col.name);
                if (d.GetOwningEntity().IsEnemy(owningWeaponSystemController.GetEntity()))
                {
                    damageables.Add(d);
                }
            }
            
            foreach (IDamageable damageable in damageables)
            {
                // if enemy object can be hit
                damaged = damageable.GetOwningEntity();
                damager = owningWeaponSystemController.GetEntity();
                if (damager.IsEnemy(damaged) && damaged.IsAlive())
                {
                    // set target
                    SetTarget(damageable.GetGameObject().transform);
                    return;
                }
            }
        }

        protected override void StartFiring()
        {
            timerID = timeManager.AddTimer(fireRate, Fire);
        }

        void Fire()
        {
            Fire(WeaponType.LaserImpulse); // Debug weapon type
        }

        protected void Fire(WeaponType type)
        {
            if (targetNetworkEntity == null || !IsFacingTarget() || !targetNetworkEntity.IsAlive() || !CanHitPosition(targetNetworkEntity.transform.position))
            {
                // IF we're not facing the target, let's see if we can get one
                ClearTarget();
                return;
            };
            var offset = Quaternion.Euler(UnityEngine.Random.onUnitSphere);
            
            // Muzzle flash
            PoolManager.Pools["GeneratedPool"].Spawn(weaponPrefabManager.GetWeaponPrefab(type).muzzle,
                TurretSocket[currentFiringSocket].position,
                TurretSocket[currentFiringSocket].rotation, TurretSocket[currentFiringSocket]);
            // Projectile
            
            PoolManager.Pools["GeneratedPool"].SpawnDamager(this,
                    weaponPrefabManager.GetWeaponPrefab(type).projectile, TurretSocket[currentFiringSocket].position,
                    offset * TurretSocket[currentFiringSocket].rotation, null);
           weaponAudioController.PlayShotAtPosition(type, TurretSocket[currentFiringSocket].position);
            
            AdvanceSocket();
        }
        
        protected void Impact(Vector3 point, WeaponType weaponType)
        {
            // Spawn impact prefab at specified position
            PoolManager.Pools["GeneratedPool"].Spawn(weaponPrefabManager.GetWeaponPrefab(weaponType).impact, point,
                Quaternion.identity, null);
            // Play impact sound effect
           weaponAudioController.PlayHitAtPosition(weaponType, point);
        }

        protected override void StopFiring()
        {
            // Remove firing timer
            if (timerID != -1)
            {
                timeManager.RemoveTimer(timerID);
                timerID = -1;
            }
        }
        
        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= tickRate)
            {
                timer -= tickRate;

                if (!owningWeaponSystemController.WeaponSystemsEnabled())
                {
                    ClearTarget();
                }
                
                if (target == null)
                    AcquireTarget();
                else
                    CheckTarget();
            }

            CheckForFire();

            if (target != null)
                targetPos = target.transform.position;

            if (!smoothControlling)
            {
                if (Mount != null)
                {

                    /////// Heading
                    headingVector =
                        Vector3.Normalize(Math3d.ProjectVectorOnPlane(Swivel.up,
                            targetPos - Swivel.position));
                    headingAngle =
                        Math3d.SignedVectorAngle(Swivel.forward, headingVector, Swivel.up);
                    turretDefaultToTargetAngle = Math3d.SignedVectorAngle(defaultRot * Swivel.forward,
                        headingVector, Swivel.up);
                    turretHeading = Math3d.SignedVectorAngle(defaultRot * Swivel.forward,
                        Swivel.forward, Swivel.up);

                    headingStep = HeadingTrackingSpeed * Time.deltaTime;

                    // Heading step and correction
                    // Full rotation
                    if (HeadingLimit.x <= -180f && HeadingLimit.y >= 180f)
                        headingStep *= Mathf.Sign(headingAngle);
                    else // Limited rotation
                        headingStep *= Mathf.Sign(turretDefaultToTargetAngle - turretHeading);

                    // Hard stop on reach no overshooting
                    if (Mathf.Abs(headingStep) > Mathf.Abs(headingAngle))
                        headingStep = headingAngle;

                    // Heading limits
                    if (curHeadingAngle + headingStep > HeadingLimit.x &&
                        curHeadingAngle + headingStep < HeadingLimit.y ||
                        HeadingLimit.x <= -180f && HeadingLimit.y >= 180f || fullAccess)
                    {
                        curHeadingAngle += headingStep;
                        Swivel.rotation = Swivel.rotation * Quaternion.Euler(0f, headingStep, 0f);
                    }

                    /////// Elevation
                    elevationVector =
                        Vector3.Normalize(Math3d.ProjectVectorOnPlane(Swivel.right,
                            targetPos - Mount.position));
                    elevationAngle =
                        Math3d.SignedVectorAngle(Mount.forward, elevationVector, Swivel.right);

                    // Elevation step and correction
                    elevationStep = Mathf.Sign(elevationAngle) * ElevationTrackingSpeed * Time.deltaTime;
                    if (Mathf.Abs(elevationStep) > Mathf.Abs(elevationAngle))
                        elevationStep = elevationAngle;

                    // Elevation limits
                    if (curElevationAngle + elevationStep < ElevationLimit.y &&
                        curElevationAngle + elevationStep > ElevationLimit.x)
                    {
                        curElevationAngle += elevationStep;
                        Mount.rotation = Mount.rotation * Quaternion.Euler(elevationStep, 0f, 0f);
                    }
                }
            }
            else
            {

                //finding position for turning just for X axis (down-up)
                targetX = targetPos - Mount.transform.position;
                targetRotationX = Quaternion.LookRotation(targetX, Mount.up);

                Mount.transform.rotation = Quaternion.Slerp(Mount.transform.rotation, targetRotationX,
                    HeadingTrackingSpeed * Time.deltaTime);
                Mount.transform.localEulerAngles = new Vector3(Mount.transform.localEulerAngles.x, 0f, 0f);

                //checking for turning up too much
                if (Mount.transform.localEulerAngles.x >= 180f &&
                    Mount.transform.localEulerAngles.x < (360f - ElevationLimit.y))
                {
                    Mount.transform.localEulerAngles = new Vector3(360f - ElevationLimit.y, 0f, 0f);
                }

                //down
                else if (Mount.transform.localEulerAngles.x < 180f &&
                         Mount.transform.localEulerAngles.x > -ElevationLimit.x)
                {
                    Mount.transform.localEulerAngles = new Vector3(-ElevationLimit.x, 0f, 0f);
                }

                //finding position for turning just for Y axis
                targetY = targetPos;
                targetY.y = Swivel.position.y;

                targetRotationY = Quaternion.LookRotation(targetY - Swivel.position, Swivel.transform.up);

                Swivel.transform.rotation = Quaternion.Slerp(Swivel.transform.rotation, targetRotationY,
                    ElevationTrackingSpeed * Time.deltaTime);
                Swivel.transform.localEulerAngles = new Vector3(0f, Swivel.transform.localEulerAngles.y, 0f);

                if (!fullAccess)
                {
                    //checking for turning left
                    if (Swivel.transform.localEulerAngles.y >= 180f &&
                        Swivel.transform.localEulerAngles.y < (360f - HeadingLimit.y))
                    {
                        Swivel.transform.localEulerAngles = new Vector3(0f, 360f - HeadingLimit.y, 0f);
                    }

                    //right
                    else if (Swivel.transform.localEulerAngles.y < 180f &&
                             Swivel.transform.localEulerAngles.y > -HeadingLimit.x)
                    {
                        Swivel.transform.localEulerAngles = new Vector3(0f, -HeadingLimit.x, 0f);
                    }
                }
            }

            if (DebugDraw)
                Debug.DrawLine(Mount.position,
                    Mount.position +
                    Mount.forward * Vector3.Distance(Mount.position, targetPos), Color.red);
        }
    }
}