using System;
using UnityEngine;
using System.Collections.Generic;
using SpaceCommander.Player;
using SpaceCommander.Pooling;
using SpaceCommander.Ships;

namespace SpaceCommander.Weapons
{
    public class Turret : MonoBehaviour, IWeaponSystem
    {
        private IDamager owningDamager;
        
        public Transform Mount;
        public Transform Swivel;

        private Vector3 defaultDir;
        private Quaternion defaultRot;

        public float HeadingTrackingSpeed = 2f;
        public float ElevationTrackingSpeed = 2f;

        private Vector3 targetPos;
        [HideInInspector] public Vector3 headingVetor;

        private float curHeadingAngle;
        private float curElevationAngle;

        public Vector2 HeadingLimit;
        public Vector2 ElevationLimit;

        public bool smoothControlling;

        public bool DebugDraw;

        public Transform targetShip; 
        
        private bool fullAccess;
        public Animator[] Animators;


        float timer = 0f;
        [SerializeField] float tickRate = .2f;
        [SerializeField] float maxRange = .25f;
        [SerializeField] LayerMask shipLayerMask;
        [SerializeField] LayerMask allRaycastableLayersMask;

        RaycastHit hitInfo; // Raycast structure
        bool isFiring; // Is turret currently in firing state
        public WeaponEffectController fxController;

        
        // Current firing socket
        private int curSocket = 0;

        // Timer reference                
        private int timerID = -1;

        [Header("Turret setup")] public Transform[] TurretSocket; // Sockets reference

        public WeaponType defaultWeaponType; // Default starting weapon type
        
        public IDamager GetDamager()
        {
            return owningDamager;
        }

        public void SetDamager(IDamager damager)
        {
            owningDamager = damager;
        }

        public void Impact(WeaponType type, Vector3 point)
        {
            switch (type)
            {
                case WeaponType.Sniper:
                    SniperImpact(point);
                    break;
                case WeaponType.Vulcan:
                    VulcanImpact(point);
                    break;
                case WeaponType.LaserImpulse:
                    LaserImpulseImpact(point);
                    break;
            }
        }
        
        // Switch to next weapon type
        public void NextWeapon()
        {
            if ((int) defaultWeaponType < Enum.GetNames(typeof(WeaponType)).Length - 1)
            {
                defaultWeaponType++;
            }
        }

        // Switch to previous weapon type
        public void PrevWeapon()
        {
            if (defaultWeaponType > 0)
            {
                defaultWeaponType--;
            }
        }

        // Advance to next turret socket
        private void AdvanceSocket()
        {
            curSocket++;
            if (curSocket >= TurretSocket.Length)
                curSocket = 0;
        }

        // Fire turret weapon
        public void Fire()
        {
            switch (defaultWeaponType)
            {
                case WeaponType.Vulcan:
                    // Fire vulcan at specified rate until canceled
                    timerID = TimeManager.time.AddTimer(WeaponEffectController.instance.VulcanFireRate, Vulcan);
                    // Invoke manually before the timer ticked to avoid initial delay
                    Vulcan();
                    break;
                
                case WeaponType.Sniper:
                    timerID = TimeManager.time.AddTimer(0.3f, Sniper);
                    Sniper();
                    break;

                case WeaponType.ShotGun:
                    timerID = TimeManager.time.AddTimer(0.3f, ShotGun);
                    ShotGun();
                    break;

                case WeaponType.LightningGun:
                    // Beams has no timer requirement
                    LightningGun();
                    break;

                case WeaponType.LaserImpulse:
                    timerID = TimeManager.time.AddTimer(0.15f, LaserImpulse);
                    LaserImpulse();
                    break;
            }
        }

        // Stop firing 
        public void Stop()
        {
            // Remove firing timer
            if (timerID != -1)
            {
                TimeManager.time.RemoveTimer(timerID);
                timerID = -1;
            }

            switch (defaultWeaponType)
            {
                case WeaponType.LightningGun:
                   WeaponAudioController.instance.LightningGunClose(transform.position);
                    break;
            }
        }

        // Fire vulcan weapon
        void Vulcan()
        {
            // Get random rotation that offset spawned projectile
            var offset = Quaternion.Euler(UnityEngine.Random.onUnitSphere);
            // Spawn muzzle flash and projectile with the rotation offset at current socket position
            PoolManager.Pools["GeneratedPool"].Spawn(WeaponEffectController.instance.vulcanMuzzle, TurretSocket[curSocket].position,
                TurretSocket[curSocket].rotation, TurretSocket[curSocket]);
            var newGO =
                PoolManager.Pools["GeneratedPool"].Spawn(WeaponEffectController.instance.vulcanProjectile,
                    TurretSocket[curSocket].position + TurretSocket[curSocket].forward,
                    offset * TurretSocket[curSocket].rotation, null).gameObject;

            var proj = newGO.gameObject.GetComponent<Projectile>();
            if (proj)
            {
                proj.SetOffset(WeaponEffectController.instance.vulcanOffset);
            }
            
            // Play shot sound effect
            WeaponAudioController.instance.VulcanShot(TurretSocket[curSocket].position);

            // Advance to next turret socket
            AdvanceSocket();
        }

        // Spawn vulcan weapon impact
        public void VulcanImpact(Vector3 pos)
        {
            // Spawn impact prefab at specified position
            PoolManager.Pools["GeneratedPool"].Spawn(WeaponEffectController.instance.vulcanImpact, pos, Quaternion.identity, null);
            // Play impact sound effect
            WeaponAudioController.instance.VulcanHit(pos);
        }
        
        // Fire sniper weapon
        private void Sniper()
        {
            var offset = Quaternion.Euler(UnityEngine.Random.onUnitSphere);

            PoolManager.Pools["GeneratedPool"].Spawn(WeaponEffectController.instance.sniperMuzzle, TurretSocket[curSocket].position,
                TurretSocket[curSocket].rotation, TurretSocket[curSocket]);
            var newGO =
                PoolManager.Pools["GeneratedPool"].Spawn(WeaponEffectController.instance.sniperBeam, TurretSocket[curSocket].position,
                    offset * TurretSocket[curSocket].rotation, null).gameObject;
            var beam = newGO.GetComponent<Beam>();
            if (beam)
            {
                beam.SetOffset(WeaponEffectController.instance.sniperOffset);
            }

            WeaponAudioController.instance.SniperShot(TurretSocket[curSocket].position);
            AdvanceSocket();
        }

        // Spawn sniper weapon impact
        void SniperImpact(Vector3 pos)
        {
            PoolManager.Pools["GeneratedPool"].Spawn(WeaponEffectController.instance.sniperImpact, pos, Quaternion.identity, null);
            WeaponAudioController.instance.SniperHit(pos);
        }

        // Fire shotgun weapon
        private void ShotGun()
        {
            var offset = Quaternion.Euler(UnityEngine.Random.onUnitSphere);
            PoolManager.Pools["GeneratedPool"].Spawn(WeaponEffectController.instance.shotGunMuzzle, TurretSocket[curSocket].position,
                TurretSocket[curSocket].rotation, TurretSocket[curSocket]);
            PoolManager.Pools["GeneratedPool"].Spawn(WeaponEffectController.instance.shotGunProjectile, TurretSocket[curSocket].position,
                offset * TurretSocket[curSocket].rotation, null);
            WeaponAudioController.instance.ShotGunShot(TurretSocket[curSocket].position);
            
            AdvanceSocket();
        }

        // Fire lightning gun weapon
        private void LightningGun()
        {
            for (var i = 0; i < TurretSocket.Length; i++)
            {
                PoolManager.Pools["GeneratedPool"].Spawn(WeaponEffectController.instance.lightningGunBeam, TurretSocket[i].position,
                    TurretSocket[i].rotation,
                    TurretSocket[i]);
            }

            WeaponAudioController.instance.LightningGunLoop(transform.position, transform);
        }

        // Fire laser pulse weapon
        private void LaserImpulse()
        {
            var offset = Quaternion.Euler(UnityEngine.Random.onUnitSphere);
            PoolManager.Pools["GeneratedPool"].Spawn(WeaponEffectController.instance.laserImpulseMuzzle, TurretSocket[curSocket].position,
                TurretSocket[curSocket].rotation, TurretSocket[curSocket]);
            var newGO =
                PoolManager.Pools["GeneratedPool"].Spawn(WeaponEffectController.instance.laserImpulseProjectile, TurretSocket[curSocket].position,
                    offset * TurretSocket[curSocket].rotation, null).gameObject;
            var proj = newGO.GetComponent<Projectile>();
            if (proj)
            {
                proj.SetOffset(WeaponEffectController.instance.laserImpulseOffset);
            }

            WeaponAudioController.instance.LaserImpulseShot(TurretSocket[curSocket].position);

            AdvanceSocket();
        }

        // Spawn laser pulse weapon impact
        void LaserImpulseImpact(Vector3 pos)
        {
            PoolManager.Pools["GeneratedPool"].Spawn(WeaponEffectController.instance.laserImpulseImpact, pos, Quaternion.identity, null);
            WeaponAudioController.instance.LaserImpulseHit(pos);
        }
        
        void CheckForFire()
        {

            if (targetShip == null) isFiring = false;

            // Fire turret
            if (!isFiring && targetShip != null)
            {
                Debug.Log("Raycasting to see if we should fire");
                // if we raycast, start shooting. otherwise, wait til next check :)
                if (Physics.Raycast(transform.position, (targetShip.position - transform.position), out hitInfo, maxRange, allRaycastableLayersMask))
                {
                    Debug.Log("Raycast successful, shooting at " + hitInfo);
                    isFiring = true;
                    Fire();
                }
            }

            // Stop firing
            else if (isFiring && targetShip == null)
            {
                isFiring = false;
                Stop();
            }

            

        }

        public void PlayAnimation()
        {
            for (int i = 0; i < Animators.Length; i++)
                Animators[i].SetTrigger("FireTrigger");
        }

        public void PlayAnimationLoop()
        {
            for (int i = 0; i < Animators.Length; i++)
                Animators[i].SetBool("FireLoopBool", true);
        }

        public void StopAnimation()
        {
            for (int i = 0; i < Animators.Length; i++)
                Animators[i].SetBool("FireLoopBool", false);
        }

        // Use this for initialization
        private void Start()
        {
            targetPos = Swivel.position + Swivel.forward * 100f;
            defaultDir = Swivel.transform.forward;
            defaultRot = Quaternion.FromToRotation(transform.forward, defaultDir);
            if (HeadingLimit.y - HeadingLimit.x >= 359.9f)
                fullAccess = true;
            StopAnimation();
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

        // Angle between mount and target
        public bool CanHitPosition()
        {
            if (targetShip == null) return false;

            return CanHitPosition(targetShip.position);


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
            else
            {
                return false;
            }


        }

        void CheckTarget()
        {
            // if current target cant be hit or object isn't within distance

            Debug.Log("Checking target");
            if(targetShip == null || !CanHitPosition() || Vector3.Distance(transform.position, targetShip.position) > maxRange)
            {
                Debug.Log("tARGET SHIP: " + targetShip + " | CanHitPosition: " + CanHitPosition() + " | Distance: " + Vector3.Distance(transform.position, targetShip.position));
                targetShip = null;
                Debug.Log("Lost target! ");
                isFiring = false;
                Stop();
            }

        }

        void AcquireTarget()
        {

            Debug.Log("Going for target acquisition");

            // overlap sphere to get list of hitting objects
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, maxRange, shipLayerMask);

            Debug.Log(hitColliders.Length);

            if (hitColliders.Length < 1) return; // No colliders hit

            // if object is an enemy
            List<IDamageable> damageables = new List<IDamageable>();

            foreach(Collider col in hitColliders)
            {
                if(col.transform.parent.GetComponent<IDamageable>().GetPlayer() != owningDamager.GetPlayer())
                {
                    damageables.Add(col.transform.parent.GetComponent<Ship>());
                }
            }

            Debug.Log("Hit " + damageables.Count + " ships!");

            foreach (IDamageable damageable in damageables)
            {
                Debug.Log("Checking against ship " + damageable);
                // if enemy object can be hit

                // Reference to 
                IPlayer sPlayerController = damageable.GetPlayer();
                IPlayer owningShipPlayerController = owningDamager.GetPlayer();
                
                if (damageable.GetGameObject().tag == "Debug" || (sPlayerController.GetTeam().teamID != owningShipPlayerController.GetTeam().teamID && CanHitPosition(damageable.GetGameObject().transform.position)))
                {
                    // set target
                    targetShip = damageable.GetGameObject().transform;
                    Debug.Log("Set target to: " + damageable.GetGameObject().name + " owned by " + damageable.GetPlayer().GetName());
                    return;
                }
            }


        }

        private void Update()
        {

            timer += Time.deltaTime;
            if(timer >= tickRate)
            {
                timer -= tickRate;
                
                    Debug.Log("Running check loop");
                if (targetShip != null)
                {
                    CheckTarget();
                } if (targetShip == null)
                {
                    AcquireTarget();

                }
                CheckForFire();
            


            }

            if (Input.GetMouseButtonDown(0))
                PlayAnimation();
            else if (Input.GetMouseButtonDown(1))
                PlayAnimationLoop();
            else if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
                StopAnimation();

            if (targetShip != null)
                targetPos = targetShip.transform.position;
            
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