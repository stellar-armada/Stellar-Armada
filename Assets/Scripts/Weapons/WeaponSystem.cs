using UnityEngine;

#pragma warning disable 0649
namespace SpaceCommander
{
    public abstract class WeaponSystem : MonoBehaviour
    {
        public Transform owningWeaponSystemTransform; // hack until we can update to 2019.3
        public IWeaponSystemController owningWeaponSystemController;

        public Transform target;
        public IEntity targetEntity;

        public IEntity owningEntity;

        [SerializeField] private float damagePerHit;

        // Timer reference                
        [HideInInspector] public int timerID = -1;

        public float timer = 0f;
        public float tickRate = .2f;
        public float maxRange = .25f;
        public LayerMask damageableLayerMask;
        public LayerMask allRaycastableLayersMask;

        RaycastHit hitInfo; // Raycast structure
        public bool isFiring; // Is turret currently in firing state

        void Awake()
        {
            owningWeaponSystemController = owningWeaponSystemTransform.GetComponent<IWeaponSystemController>();
            owningWeaponSystemController.RegisterWeaponSystem(this);
        }

        public IWeaponSystemController GetWeaponSystemController()
        {
            return owningWeaponSystemController;
        }

        public void SetWeaponSystemController(IWeaponSystemController weaponSystemController)
        {
            owningWeaponSystemController = weaponSystemController;
        }

        public Transform GetTarget()
        {
            return target;
        }

        public void SetTarget(Transform t)
        {
            target = t;
            targetEntity = t.GetComponent<IDamageable>().GetOwningEntity();
        }

        public void ClearTarget()
        {
            target = null;
            targetEntity = null;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public float GetDamage()
        {
            return damagePerHit;
        }

        protected abstract void AcquireTarget();

        // Fire turret weapon
        protected abstract void StartFiring();

        // Stop firing 
        protected abstract void StopFiring();

        public abstract void Impact(Vector3 point);

        protected void CheckForFire()
        {
            if (target == null) isFiring = false;

            // Fire turret
            if (!isFiring && target != null)
            {
                Debug.Log("Raycasting to see if we should fire");
                // if we raycast, start shooting. otherwise, wait til next check :)
                if (Physics.Raycast(transform.position, (target.position - transform.position), out hitInfo, maxRange,
                    allRaycastableLayersMask))
                {
                    Debug.Log("Raycast successful, shooting at " + hitInfo);
                    isFiring = true;
                    StartFiring();
                }
            }

            // Stop firing
            else if (isFiring && target == null)
            {
                isFiring = false;
                StopFiring();
            }
        }
    }
}