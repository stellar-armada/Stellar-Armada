using StellarArmada.IO;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Ships
{
// Local player singleton that manages how ships group together when warping in or being placed by selection
    public class VrShipShipFormationManager : ShipFormationManager
    {
        private Vector2 dPad = Vector2.zero;

        void Start()
        {
            
            InputManager.instance.OnLeftThumbstickAnalog += (direction) =>
            {
                if (HandSwitcher.instance.CurrentHandIsLeft()) dPad = direction;
            };
            InputManager.instance.OnRightThumbstickAnalog += (direction) =>
            {
                if (HandSwitcher.instance.CurrentHandIsRight()) dPad = direction;
            };
        }
        
        void HandleThumbstick()
        {
            
            if (Mathf.Abs(dPad.x) > deadZone) // dPad X value is above deadzone
            {
                float newXY = scaleXY + dPad.x * scaleSpeed * Time.deltaTime;
                scaleXY = Mathf.Clamp(newXY, minScaleXY, maxScaleXY);
            }
            
            if (Mathf.Abs(dPad.y) > deadZone) // dPad Y value is above deadzone
            {
                float newZ = scaleZ + dPad.y * scaleSpeed * Time.deltaTime;
                scaleZ = Mathf.Clamp(newZ, minScaleZ, maxScaleZ);
            }
        }

        void Update()
        {
            HandleThumbstick();
        }
        
    }
}