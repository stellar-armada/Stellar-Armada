using UnityEngine;
using UnityEngine.Serialization;

#pragma warning disable 0649
namespace StellarArmada.Entities
{
    public class EntityShield : MonoBehaviour, IDamageable, ICollidable
    {
        [SerializeField] private NetworkEntity entity;
        
        public float maxShield;

        public delegate void ShieldChangeEvent(float shieldVal);

        public bool isEnaled = true;
        
        public ShieldChangeEvent ShieldChanged;

        [SerializeField] private float shieldRechargeDelayTime;
        [SerializeField] private float shieldRechargeRate;
        public ShieldEffectController shieldEffectController;
        
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
            if (!isEnaled) return;
            
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
            if (shieldEffectController && !shieldEffectController.IsDuringActivationAnim())
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
            }
            else if (entity.IsAlive())
                CmdDie();

            lastHit = Time.time;
        }

        public void CmdRechargeShields(float charge)
        {
            currentShield = Mathf.Min(currentShield + charge, maxShield);
        }

        public NetworkEntity GetOwningEntity()
        {
            return entity;
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
            if (!shieldsUp && s >= maxShield / 10f) // Shield regenerates when it hits 1/10th HP
            {
                // Shields back up
                EnableShield();
            }

            if (shieldsUp && s <= 0)
            {
                // Shields down
                DisableShieldComponents();
            }

            ShieldChanged.Invoke(s);
        }
        
        public IDamageable GetDamageable()
        {
            return this;
        }

    }
}