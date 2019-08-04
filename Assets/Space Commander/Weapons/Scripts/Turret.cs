using UnityEngine;
using System.Collections.Generic;
using SpaceCommander.Pooling;

namespace SpaceCommander.Weapons
{
    public class Turret : WeaponSystem
    {

        [Header("Turret setup")] public Transform[] TurretSocket; // Sockets reference

        public Transform Mount;
        public Transform Swivel;

        private Vector3 defaultDir;
        private Quaternion defaultRot;

        public float HeadingTrackingSpeed = 2f;
        public float ElevationTrackingSpeed = 2f;

        [HideInInspector] public Vector3 headingVetor;

        private float curHeadingAngle;
        private float curElevationAngle;

        public Vector2 HeadingLimit;
        public Vector2 ElevationLimit;

        public bool smoothControlling;
        public bool DebugDraw;
        private bool fullAccess;
        RaycastHit hitInfo; // Raycast structure
        private Vector3 targetPos;

        // Current firing socket
        [HideInInspector] public int curSocket = 0;

        public float maximumAngleDifferenceToTarget = 10f;


        public float fireRate = .3f;

        public override void Impact(Vector3 point)
        {
            PoolManager.Pools["GeneratedPool"].Spawn(WeaponPrefabManager.instance.laserImpulseImpact, point,
                Quaternion.identity, null);
            WeaponAudioController.instance.PlayHitAtPosition(WeaponType.LaserImpulse, point);
        }


        // Advance to next turret socket
        public void AdvanceSocket()
        {
            curSocket++;
            if (curSocket >= TurretSocket.Length)
                curSocket = 0;
        }

        // Use this for initialization
        private void Start()
        {
            targetPos = Swivel.position + Swivel.forward * 100f;
            defaultDir = Swivel.transform.forward;
            defaultRot = Quaternion.FromToRotation(transform.forward, defaultDir);
            if (HeadingLimit.y - HeadingLimit.x >= 359.9f)
                fullAccess = true;
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
            Vector3 targetDirection = (pos - Mount.transform.position).normalized;

            Quaternion rotationToTarget = Quaternion.LookRotation(targetDirection);

            float angle = Quaternion.Angle(rotationToTarget, Mount.transform.rotation);

            if (angle <= HeadingLimit.y)
            {
                return true;
            }

            return false;
        }

        void CheckTarget()
        {

            // if current target cant be hit or object isn't within distance
            if (!owningWeaponSystemController.WeaponSystemsEnabled() || target == null || !CanHitPosition() || Vector3.Distance(transform.position, target.position) > maxRange)
            {
                Debug.Log("Target Ship: " + target + " | CanHitPosition: " + CanHitPosition() + " | Distance: " +
                          Vector3.Distance(transform.position, target.position));
                target = null;
                Debug.Log("Lost target! ");
                isFiring = false;
                StopFiring();
            }
        }



        public override void AcquireTarget()
        {
            if (!owningWeaponSystemController.WeaponSystemsEnabled()) return;
            // overlap sphere to get list of hitting objects
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, maxRange, damageableLayerMask);

            if (hitColliders.Length < 1) return; // No colliders hit

            // if object is an enemy
            List<IDamageable> damageables = new List<IDamageable>();

            foreach (Collider col in hitColliders)
            {

                IDamageable d = col.GetComponent<ICollidable>().GetDamageable();
                if (d == null) Debug.LogError("Damageable was not found on collidable reference on " + col.name);
                if (d.GetOwningEntity().GetPlayer().IsEnemy(owningWeaponSystemController.GetEntity().GetPlayer()))
                {
                    damageables.Add(d);
                }
            }

            Debug.Log("Hit " + damageables.Count + " ships!");

            foreach (IDamageable damageable in damageables)
            {
                // if enemy object can be hit

                IPlayer sPlayerController = damageable.GetOwningEntity().GetPlayer();
                IPlayer owningShipPlayerController = owningWeaponSystemController.GetEntity().GetPlayer();

                if (damageable.GetGameObject().tag == "Debug" ||
                    (owningShipPlayerController.IsEnemy(sPlayerController) &&
                     CanHitPosition(damageable.GetGameObject().transform.position)))
                {
                    // set target
                    target = damageable.GetGameObject().transform;
                    Debug.Log("Set target to: " + damageable.GetGameObject().name + " owned by " +
                              damageable.GetOwningEntity().GetPlayer().GetName());
                    return;
                }
            }
        }

        public override void StartFiring()
        {
            timerID = TimeManager.instance.AddTimer(fireRate, Fire);
        }

        void Fire()
        {
            Fire(WeaponType.LaserImpulse); // Debug weapon type
        }

        public void Fire(WeaponType type)
        {
            if (!IsFacingTarget()) return;
            var offset = Quaternion.Euler(UnityEngine.Random.onUnitSphere);
            PoolManager.Pools["GeneratedPool"].Spawn(WeaponPrefabManager.instance.laserImpulseMuzzle,
                TurretSocket[curSocket].position,
                TurretSocket[curSocket].rotation, TurretSocket[curSocket]);
            var newGO =
                PoolManager.Pools["GeneratedPool"].SpawnDamager(this,
                    WeaponPrefabManager.instance.laserImpulseProjectile, TurretSocket[curSocket].position,
                    offset * TurretSocket[curSocket].rotation, null).gameObject;
            WeaponAudioController.instance.PlayShotAtPosition(WeaponType.LaserImpulse, TurretSocket[curSocket].position);
            
            AdvanceSocket();
        }

        public override void StopFiring()
        {
            // Remove firing timer
            if (timerID != -1)
            {
                TimeManager.instance.RemoveTimer(timerID);
                timerID = -1;
            }
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= tickRate)
            {
                timer -= tickRate;

                if (owningWeaponSystemController.WeaponSystemsEnabled())
                {
                    SetTarget(null);
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
                    headingVetor =
                        Vector3.Normalize(Math3d.ProjectVectorOnPlane(Swivel.up,
                            targetPos - Swivel.position));
                    float headingAngle =
                        Math3d.SignedVectorAngle(Swivel.forward, headingVetor, Swivel.up);
                    float turretDefaultToTargetAngle = Math3d.SignedVectorAngle(defaultRot * Swivel.forward,
                        headingVetor, Swivel.up);
                    float turretHeading = Math3d.SignedVectorAngle(defaultRot * Swivel.forward,
                        Swivel.forward, Swivel.up);

                    float headingStep = HeadingTrackingSpeed * Time.deltaTime;

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
                    Vector3 elevationVector =
                        Vector3.Normalize(Math3d.ProjectVectorOnPlane(Swivel.right,
                            targetPos - Mount.position));
                    float elevationAngle =
                        Math3d.SignedVectorAngle(Mount.forward, elevationVector, Swivel.right);

                    // Elevation step and correction
                    float elevationStep = Mathf.Sign(elevationAngle) * ElevationTrackingSpeed * Time.deltaTime;
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
                Transform barrelX = Mount;
                Transform barrelY = Swivel.transform;

                //finding position for turning just for X axis (down-up)
                Vector3 targetX = targetPos - barrelX.transform.position;
                Quaternion targetRotationX = Quaternion.LookRotation(targetX, Mount.up);

                barrelX.transform.rotation = Quaternion.Slerp(barrelX.transform.rotation, targetRotationX,
                    HeadingTrackingSpeed * Time.deltaTime);
                barrelX.transform.localEulerAngles = new Vector3(barrelX.transform.localEulerAngles.x, 0f, 0f);

                //checking for turning up too much
                if (barrelX.transform.localEulerAngles.x >= 180f &&
                    barrelX.transform.localEulerAngles.x < (360f - ElevationLimit.y))
                {
                    barrelX.transform.localEulerAngles = new Vector3(360f - ElevationLimit.y, 0f, 0f);
                }

                //down
                else if (barrelX.transform.localEulerAngles.x < 180f &&
                         barrelX.transform.localEulerAngles.x > -ElevationLimit.x)
                {
                    barrelX.transform.localEulerAngles = new Vector3(-ElevationLimit.x, 0f, 0f);
                }

                //finding position for turning just for Y axis
                Vector3 targetY = targetPos;
                targetY.y = barrelY.position.y;

                Quaternion targetRotationY = Quaternion.LookRotation(targetY - barrelY.position, barrelY.transform.up);

                barrelY.transform.rotation = Quaternion.Slerp(barrelY.transform.rotation, targetRotationY,
                    ElevationTrackingSpeed * Time.deltaTime);
                barrelY.transform.localEulerAngles = new Vector3(0f, barrelY.transform.localEulerAngles.y, 0f);

                if (!fullAccess)
                {
                    //checking for turning left
                    if (barrelY.transform.localEulerAngles.y >= 180f &&
                        barrelY.transform.localEulerAngles.y < (360f - HeadingLimit.y))
                    {
                        barrelY.transform.localEulerAngles = new Vector3(0f, 360f - HeadingLimit.y, 0f);
                    }

                    //right
                    else if (barrelY.transform.localEulerAngles.y < 180f &&
                             barrelY.transform.localEulerAngles.y > -HeadingLimit.x)
                    {
                        barrelY.transform.localEulerAngles = new Vector3(0f, -HeadingLimit.x, 0f);
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