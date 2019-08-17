using UnityEngine;

#pragma warning disable 0649
namespace SpaceCommander.Ships.Tests
{
    public class HealthTest : MonoBehaviour
    {
        Hull _hull;
        Shield _shield;

        float amount = 50;
        
        bool CheckPlayer()
        {
            if (EntityManager.GetEntities().Count < 1) return false;
            Ship ship = (Ship)EntityManager.GetEntities()[0];
            _hull = ship.hull;
            if (_shield == null) _shield = ship.shield;
            if (!ship.IsAlive()) return false;
            return true;
        }

        public void TakeDamage()
        {
            if(!CheckPlayer()) return;
            _hull.TakeDamage(amount, Vector3.zero, null);
        }

        public void RechargeShields()
        {
            if(!CheckPlayer()) return;
            _shield.CmdRechargeShields(amount);
        }

        public void RepairHull()
        {
            if(!CheckPlayer()) return;
            _hull.CmdRepairHull(amount);
        }

        public void Die()
        {
            if(!CheckPlayer()) return;
            _hull.TakeDamage(10000, Vector3.zero, null);
            _hull.TakeDamage(10000, Vector3.zero, null);
        }
        
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Comma))
            {
                _hull.TakeDamage(10, Vector3.zero, null);
            } else if (Input.GetKeyDown(KeyCode.Period))
            {
                _shield.CmdRechargeShields(10);
            } else if (Input.GetKeyDown(KeyCode.Slash))
            {
                _hull.CmdRepairHull(10);
            }
        }
    }
}