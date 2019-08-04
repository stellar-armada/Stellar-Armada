using UnityEngine;

namespace SpaceCommander.Ships.Test
{
    public class WarpTest : MonoBehaviour
    {
        [HideInInspector] public ShipWarp shipWarp;

        bool CheckPlayer()
        {
            if (ShipManager.GetShips().Count < 1) return false;
            if (shipWarp == null) shipWarp = ShipManager.GetShips()[0].shipWarp;
            return true;
        }
        public void WarpIn()
        {
            if (!CheckPlayer()) return;
            shipWarp.InitWarp();
        }
        
    }
}