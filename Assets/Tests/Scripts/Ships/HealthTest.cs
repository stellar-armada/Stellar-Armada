using UnityEngine;

#pragma warning disable 0649
namespace SpaceCommander.Ships.Tests
{
    public class HealthTest : MonoBehaviour
    {
        ShipHull shipHull;
        ShipShield shipShield;

        float amount = 50;
        
        bool CheckPlayer()
        {
            if (ShipManager.GetShips().Count < 1) return false;
            Ship ship = ShipManager.GetShips()[0];
            shipHull = ship.shipHull;
            if (shipShield == null) shipShield = ship.shipShield;
            if (!ship.IsAlive()) return false;
            return true;
        }

        public void TakeDamage()
        {
            if(!CheckPlayer()) return;
            shipHull.TakeDamage(amount, Vector3.zero, null);
        }

        public void RechargeShields()
        {
            if(!CheckPlayer()) return;
            shipShield.CmdRechargeShields(amount);
        }

        public void RepairHull()
        {
            if(!CheckPlayer()) return;
            shipHull.CmdRepairHull(amount);
        }

        public void Die()
        {
            if(!CheckPlayer()) return;
            shipHull.TakeDamage(10000, Vector3.zero, null);
            shipHull.TakeDamage(10000, Vector3.zero, null);
        }
        
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Comma))
            {
                shipHull.TakeDamage(10, Vector3.zero, null);
            } else if (Input.GetKeyDown(KeyCode.Period))
            {
                shipShield.CmdRechargeShields(10);
            } else if (Input.GetKeyDown(KeyCode.Slash))
            {
                shipHull.CmdRepairHull(10);
            }
        }
    }
}