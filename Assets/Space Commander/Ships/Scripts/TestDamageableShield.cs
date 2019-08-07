using UnityEngine;
using UnityEngine.Events;

namespace SpaceCommander.Ships
{
    [RequireComponent(typeof(Collider))]
    public class TestDamageableShield : MonoBehaviour, IDamageable
    {
        private IEntity owningEntity;
        
        public float maxShield;
        
        public UnityEvent ShieldChanged;
        
        [SerializeField] private float shieldRechargeDelayTime;
        [SerializeField] private float shieldRechargeRate;
        [SerializeField] private ShieldEffectController shieldEffectController;

        private float lastHit;
        
        public float currentShield;
        private float lastShield;

        private Collider col;
        private bool shieldsUp = false;
        void Awake()
        {
            currentShield = maxShield;
            col = GetComponent<Collider>();
        }

        void Update()
        {
            //Automatic shield regen
            if (currentShield < maxShield && Time.time > lastHit + shieldRechargeDelayTime)
            {
                currentShield = Mathf.Min(currentShield + shieldRechargeRate * Time.deltaTime, maxShield);
            }
            if (lastShield != currentShield)
            {
                HandleShieldChange(currentShield);
                lastShield = currentShield;
            }
        }
        
        
        public void TakeDamage(float damage, Vector3 point, Damager damager)
        {
            if (shieldEffectController && !shieldEffectController.GetIsDuringActivationAnim())
            {
                shieldEffectController.OnHit(point, damager.GetImpactSize());
            }
            
            CmdTakeDamage(damage);
        }

        void CmdTakeDamage(float damage)
        {
            if (currentShield > 0)
            {
                currentShield -= damage;
            }  else if (owningEntity.IsAlive())
                CmdDie();

            lastHit = Time.time;
        }

        public void CmdRechargeShields(float charge)
        {
            currentShield = Mathf.Min(currentShield + charge, maxShield);
        }

        public IEntity GetOwningEntity()
        {
            return owningEntity;
        }

        public void SetOwningEntity(IEntity playerEntity)
        {
            owningEntity = playerEntity;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public void CmdDie()
        {
            shieldEffectController.SetShieldActive(false, true);
            GetComponent<Collider>().enabled = false;
        }
        
        void EnableShield()
        {
            shieldEffectController.SetShieldActive(true, true);
            col.enabled = true;
            shieldsUp = true;
        }

        void DisableShieldComponents()
        {
            shieldEffectController.SetShieldActive(false, true);
            col.enabled = false;
            shieldsUp = false;
        }


        void HandleShieldChange(float s)
        {
            if (!shieldsUp && s >= maxShield/10f) // Shield regenerates when it hits 1/10th HP
            {
                // Shields back up
                EnableShield();
            }

            if (shieldsUp && s <= 0)
            {
                // Shields down
                DisableShieldComponents();
            }
            ShieldChanged.Invoke();
        }
    }  
}

