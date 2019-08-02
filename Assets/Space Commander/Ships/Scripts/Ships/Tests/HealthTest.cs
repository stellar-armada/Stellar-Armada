using UnityEngine;

namespace SpaceCommander.Ships.Tests
{
    public class HealthTest : MonoBehaviour
    {
        Health health;

        float amount = 50;
        
        bool CheckPlayer()
        {
            if (ShipManager.GetShips().Count < 1) return false;
            if (health == null) health = ShipManager.GetShips()[0].health;
            if (health.isDead) return false;
            return true;
        }

        public void TakeDamage()
        {
            if(!CheckPlayer()) return;
            health.CmdTakeDamage(amount);
        }

        public void RechargeShields()
        {
            if(!CheckPlayer()) return;
            health.CmdRechargeShields(amount);
        }

        public void RepairHull()
        {
            if(!CheckPlayer()) return;
            health.CmdRepairHull(amount);
        }

        public void Die()
        {
            if(!CheckPlayer()) return;
            health.CmdTakeDamage(10000);
            health.CmdTakeDamage(10000);
        }
        
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Comma))
            {
                health.CmdTakeDamage(10);
            } else if (Input.GetKeyDown(KeyCode.Period))
            {
                health.CmdRechargeShields(10);
            } else if (Input.GetKeyDown(KeyCode.Slash))
            {
                health.CmdRepairHull(10);
            }
        }
    }
}