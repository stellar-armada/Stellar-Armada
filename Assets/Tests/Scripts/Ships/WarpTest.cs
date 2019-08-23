using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Ships.Test
{
    public class WarpTest : MonoBehaviour
    {
        [HideInInspector] public ShipWarp shipWarp;

        bool CheckPlayer()
        {
            if (EntityManager.GetEntities().Count < 1) return false;
            if (shipWarp == null) shipWarp = ((Ship)EntityManager.GetEntities()[0]).shipWarp;
            return true;
        }
        public void WarpIn()
        {
            if (!CheckPlayer()) return;
            shipWarp.InitWarp(Vector3.zero, Quaternion.identity);
        }
        
    }
}