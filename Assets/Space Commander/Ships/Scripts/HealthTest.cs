using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SpaceCommander.Ships
{
    public class HealthTest : MonoBehaviour, IDamageable
    {
        private IPlayerEntity owningEntity;

        public float maxHull;
        public float maxShield;
        
        public bool isDead;

        public UnityEvent ShipDestroyed;
        public UnityEvent HullChanged;
        public UnityEvent ShieldChanged;


        [SerializeField] private float shieldRechargeDelayTime;
        [SerializeField] private float shieldRechargeRate;
        
        private float lastHit;
        
        public float hull;
        public float shield;

        void Awake()
        {
            hull = maxHull;
            shield = maxShield;

        }

        void Update()
        {
            //Automatic shield regen
            if (shield < maxShield && Time.time > lastHit + shieldRechargeDelayTime)
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

        public void CmdRechargeShields(float charge)
        {
            shield = Mathf.Min(shield + charge, maxShield);
            Debug.Log("Shields recharged to " + shield);
        }
        
        public void CmdRepairHull(float amount)
        {
            hull = Mathf.Min(hull + amount, maxShield);
            Debug.Log("Hull repaired to " + hull);
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
            if (!isDead) CmdTakeDamage(damage);
        }

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

