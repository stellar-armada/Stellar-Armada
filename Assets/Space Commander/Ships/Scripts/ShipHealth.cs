using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SpaceCommander.Ships
{
    public class ShipHealth : NetworkBehaviour, IDamageable
    {
        private IPlayerOwnedEntity owner;

        public float maxHull;
        public float maxShield;
        
        [SyncVar(hook=nameof(HandleDeath))] public bool isDead;

        public UnityEvent ShipDestroyed;
        public UnityEvent HullChanged;
        public UnityEvent ShieldChanged;


        [SerializeField] private float shieldRechargeDelayTime;
        [SerializeField] private float shieldRechargeRate;
        
        private float lastHit;
        
        [SyncVar (hook=nameof(HandleHullChange))] public float hull;
        [SyncVar (hook=nameof(HandleShieldChange))] public float shield;

        void Awake()
        {
            hull = maxHull;
            shield = maxShield;

        }

        void Update()
        {
            //Automatic shield regen
            if (isServer && shield < maxShield && Time.time > lastHit + shieldRechargeDelayTime)
            {
                shield = Mathf.Min(shield + shieldRechargeRate * Time.deltaTime, maxShield);
            }
        }

        void HandleDeath(bool dead)
        {
            if (dead)
            {
                ShipDestroyed.Invoke();
            }
        }

        [Command]
        public void CmdRechargeShields(float charge)
        {
            shield = Mathf.Min(shield + charge, maxShield);
            Debug.Log("Shields recharged to " + shield);
        }
        
        [Command]
        public void CmdRepairHull(float amount)
        {
            hull = Mathf.Min(hull + amount, maxShield);
            Debug.Log("Hull repaired to " + hull);
        }


        public IPlayerOwnedEntity GetOwnable()
        {
            return owner;
        }

        public void SetOwnable(IPlayerOwnedEntity playerOwnedEntity)
        {
            owner = playerOwnedEntity;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }
        
        public void TakeDamage(float damage)
        {
            if (!isDead && isServer) CmdTakeDamage(damage);
        }

        [Command]
        void CmdTakeDamage(float damage)
        {
            if (shield > 0)
            {
                shield -= damage;
            }
            else
            {
                hull -= damage;
                if (hull <= 0 && !isDead)
                {
                    CmdDie();
                }
            }

            lastHit = Time.time;
        }

        [Command]
        public void CmdDie()
        {
            isDead = true;
        }
        
        void HandleHullChange(float h)
        {
            HullChanged.Invoke();
        }
        
        void HandleShieldChange(float s)
        {
            ShieldChanged.Invoke();
        }
    }  
}

