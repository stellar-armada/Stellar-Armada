using System.Collections.Generic;
using System.Linq;
using StellarArmada.IO;
using StellarArmada.Levels;
using StellarArmada.Player;
using UnityEngine;

namespace StellarArmada.Entities.Ships
{
#pragma warning disable 0649
    public class VRShipPlacer : ShipPlacer
    {
        private bool uiPointerIsActive;

        private bool leftThumbstickIsDown;
        private bool rightThumbstickIsDown;

        private bool rightPlaceButtonIsDown;
        private bool leftPlaceButtonIsDown;

        void Start()
        {
            // Thumbsticks show hide our placementPositionRoot
            InputManager.instance.OnLeftThumbstickButton += HandleLeftThumbstick;
            InputManager.instance.OnRightThumbstickButton += HandleRightThumbstick;

            InputManager.instance.OnButtonOne += HandleRightPlaceButton;
            InputManager.instance.OnButtonThree += HandleLeftPlaceButton;
        }

        void HandleRightPlaceButton(bool down)
        {
            if (!HandSwitcher.instance.CurrentHandIsRight()) return;
            if (leftPlaceButtonIsDown) return; // if the other button is down, ignore this input
            if (!down && !rightPlaceButtonIsDown)
                return; // if button going up but down state was blocked by other side button, ignore action beyond this point
            rightPlaceButtonIsDown = down;
            if (!uiPointerIsActive && down) Place();
        }

        void HandleLeftPlaceButton(bool down)
        {
            if (!HandSwitcher.instance.CurrentHandIsLeft()) return; // If this isn't the current hand, ignore input
            if (rightPlaceButtonIsDown) return; // if the other button is down, ignore this input
            if (!down && !leftPlaceButtonIsDown)
                return; // if button going up but down state was blocked by other side button, ignore action beyond this point
            leftPlaceButtonIsDown = down;
            if (!uiPointerIsActive && down) Place();
        }

        void HandleLeftThumbstick(bool down)
        {
            if (rightThumbstickIsDown) return; // if the other button is down, ignore this input
            if (!down && !leftThumbstickIsDown)
                return; // if button going up but down state was blocked by other side button, ignore action beyond this point
            leftThumbstickIsDown = down;
            ShipPlacementCursor.instance.gameObject.SetActive(!down);
            uiPointerIsActive = down;
        }

        void HandleRightThumbstick(bool down)
        {
            if (leftThumbstickIsDown) return; // if the other button is down, ignore this input
            if (!down && !rightThumbstickIsDown)
                return; // if button going up but down state was blocked by other side button, ignore action beyond this point
            rightThumbstickIsDown = down;
            ShipPlacementCursor.instance.gameObject.SetActive(!down);
            uiPointerIsActive = down;
        }

    }
}