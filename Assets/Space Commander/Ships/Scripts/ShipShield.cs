using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace SpaceCommander.Ships
{
    public class ShipShield : NetworkBehaviour, IDamageable
    {
        private IPlayerEntity owningEntity;

        public float maxShield;

        public MonoBehaviour[] shieldComponents;
        
        [SerializeField] private float shieldRechargeDelayTime;
        [SerializeField] private float shieldRechargeRate;
        
        private float lastHit;
        
        [FormerlySerializedAs("shield")] [SyncVar (hook=nameof(HandleShieldChange))] public float currentShield;
        
        public UnityEvent ShieldChanged;

        void Awake()
        {
            currentShield = maxShield;
        }

        void RechargeShieldsOverTime()
        {
            //Automatic shield regen
            if (isServer && Time.time > lastHit + shieldRechargeDelayTime)
            {
                currentShield = Mathf.Min(currentShield + shieldRechargeRate * Time.deltaTime, maxShield);
            }
        }

        [Command]
        public void CmdRechargeShields(float charge)
        {
            currentShield = Mathf.Min(currentShield + charge, maxShield);
            Debug.Log("Shields recharged to " + currentShield);
        }


        public IPlayerEntity GetOwningEntity()
        {
            return owningEntity;
        }

        public void SetOwningEntity(IPlayerEntity playerEntity)
        {
            owningEntity = playerEntity;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }
        
        public void TakeDamage(float damage)
        {
            if (!owningEntity.IsAlive() && isServer) CmdTakeDamage(damage);
        }

        [Command]
        void CmdTakeDamage(float damage)
        {
            if (currentShield > 0)
            {
                currentShield -= damage;
            } else if (owningEntity.IsAlive())
                {
                    CmdDie();
                }

            lastHit = Time.time;
        }

        void EnableShieldComponents()
        {
            foreach (MonoBehaviour c in shieldComponents)
            {
                c.enabled = true;
            }
        }

        void DisableShieldComponents()
        {
            foreach (MonoBehaviour c in shieldComponents)
            {
                c.enabled = false;
            }
        }

        [Command]
        public void CmdDie()
        {
            
        }

        void HandleShieldChange(float s)
        {
            if (s > 0 && currentShield < 0)
            {
                // Shields back up
                EnableShieldComponents();
            }

            if (s <= 0 && currentShield > 0)
            {
                // Shields down
                DisableShieldComponents();
            }
            ShieldChanged.Invoke();

        }
    }  
}

