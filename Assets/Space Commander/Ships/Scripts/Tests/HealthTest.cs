using UnityEngine;

namespace SpaceCommander.Ships.Tests
{
    public class HealthTest : MonoBehaviour
    {
        ShipHealth _shipHealth;

        float amount = 50;
        
        bool CheckPlayer()
        {
            if (ShipManager.GetShips().Count < 1) return false;
            if (_shipHealth == null) _shipHealth = ShipManager.GetShips()[0].shipHealth;
            if (_shipHealth.isDead) return false;
            return true;
        }

        public void TakeDamage()
        {
            if(!CheckPlayer()) return;
            _shipHealth.TakeDamage(amount);
        }

        public void RechargeShields()
        {
            if(!CheckPlayer()) return;
            _shipHealth.CmdRechargeShields(amount);
        }

        public void RepairHull()
        {
            if(!CheckPlayer()) return;
            _shipHealth.CmdRepairHull(amount);
        }

        public void Die()
        {
            if(!CheckPlayer()) return;
            _shipHealth.TakeDamage(10000);
            _shipHealth.TakeDamage(10000);
        }
        
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Comma))
            {
                _shipHealth.TakeDamage(10);
            } else if (Input.GetKeyDown(KeyCode.Period))
            {
                _shipHealth.CmdRechargeShields(10);
            } else if (Input.GetKeyDown(KeyCode.Slash))
            {
                _shipHealth.CmdRepairHull(10);
            }
        }
    }
}