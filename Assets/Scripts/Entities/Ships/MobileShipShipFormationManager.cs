using System.Collections.Generic;
using System.Linq;
using Mirror;
using StellarArmada.IO;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Entities.Ships
{
// Local player singleton that manages how ships group together when warping in or being placed by selection
    public class MobileShipShipFormationManager : ShipFormationManager
    {
        
        public void ScaleByDelta(float startPos, float currentPos)
        {
            
        }

        public void RotateToward()
        {
            
        }

        public void ScaleX(bool up)
        {
            float newXY = scaleXY + (up ? 1f : -1f) * scaleSpeed * Time.deltaTime;
            scaleXY = Mathf.Clamp(newXY, minScaleXY, maxScaleXY);
        }

        public void ScaleZUp(bool up)
        {
            float newZ = scaleZ + (up ? 1f : -1f) * scaleSpeed * Time.deltaTime;
            scaleZ = Mathf.Clamp(newZ, minScaleZ, maxScaleZ);
        }

    }
}