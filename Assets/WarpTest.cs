using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCommander.Ships.Test
{
    public class WarpTest : MonoBehaviour
    {
        [HideInInspector] public Warp warp;

        bool CheckPlayer()
        {
            if (ShipManager.GetShips().Count < 1) return false;
            if (warp == null) warp = ShipManager.GetShips()[0].warp;
            return true;
        }
        public void WarpIn()
        {
            if (!CheckPlayer()) return;
            warp.InitWarp();
        }
        
    }
}