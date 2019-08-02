using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceCommander.Ships
{
    public class Health : NetworkBehaviour
    {
        private Ship ship;
        
        public float maxHull;
        public float maxShield;
        
        public bool isDead;

        [SerializeField] Slider hullSlider;
        [SerializeField] Slider shieldSlider;
        [SerializeField] private float shieldRechargeDelayTime;
        [SerializeField] private float shieldRechargeRate;
        
        private float lastHit;
        
        [SyncVar (hook=nameof(HandleHullChange))] public float hull;
        [SyncVar (hook=nameof(HandleShieldChange))] public float shield;

        void Awake()
        {
            ship = GetComponent<Ship>();
            hull = maxHull;
            shield = maxShield;
            hullSlider.value = hull / maxHull;
            shieldSlider.value = shield / maxShield;
        }

        void Update()
        {
            //Automatic shield regen
            if (isServer && shield < maxShield && Time.time > lastHit + shieldRechargeDelayTime)
            {
                shield = Mathf.Min(shield + shieldRechargeRate * Time.deltaTime, maxShield);
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


        [Command]
        public void CmdTakeDamage(float damage)
        {
            if (isDead) return;
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
            ship.health.isDead = true;
            ship.explosion.CmdExplode(); 
        }
        
        void HandleHullChange(float h)
        {
            hullSlider.value = h / maxShield;
        }
        
        void HandleShieldChange(float s)
        {
            shieldSlider.value = s / maxShield;
        }
    }  
}

