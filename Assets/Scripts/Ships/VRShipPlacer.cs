using System.Collections.Generic;
using System.Linq;
using StellarArmada.IO;
using StellarArmada.Levels;
using StellarArmada.Player;
using UnityEngine;

namespace StellarArmada.Ships
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
        
        public override void ShowPlacements()
        {
            if (IsInvoking(nameof(ShowPlacements))) CancelInvoke(nameof(ShowPlacements));

            HidePlacements(); // reset our placements for another round of formation calculations

            // Get current ships
            List<Ship> ships = ShipSelectionManager.instance.GetCurrentSelection()
                .Select(selectable => selectable.GetShip() as Ship).ToList();

            // Get formation positions for selection
            var positions = ShipFormationManager.instance.GetFormationPositionsForShips(ships);
            
            foreach (Ship s in ships)
            {
                // Get the placement indicator
                ShipPlacementIndicator pI = ShipPlacementUIManager.placementIndicators.Single(p => p.ship == s);

                //Activate
                pI.gameObject.SetActive(true);
                activePlacements.Add(pI); // Quick cache for disabling on placement
                pI.transform.SetParent(ShipPlacementCursor.instance.transform, true);
                pI.transform.localScale = Vector3.one;
                pI.transform.localRotation = Quaternion.identity;
                pI.Show(positions[s]);
            }
            
            Invoke(nameof(ShowPlacements), .2f);
        }
        
        protected override void HidePlacements()
        {
            if (IsInvoking(nameof(ShowPlacements))) CancelInvoke(nameof(ShowPlacements));
            foreach (ShipPlacementIndicator pI in activePlacements)
                pI.Hide();
            activePlacements = new List<ShipPlacementIndicator>();
        }
        
        public override void Place()
        {
            // If we're not highlighting a ship
            if (ShipSelector.instance.currentSelectables.Count == 0)
            {
                // Is it a double tap?
                if (Time.time - lastTap < doubleTapThreshold)
                {
                    StopAllShips();
                }
                else
                {
                    foreach (ShipPlacementIndicator pi in activePlacements)
                    {
                        // For each placer, set the minimap parent so we can grab local pos and rot easier
                        // Otherwise we could optimise with some matrix/quaternion math!
                        pi.transform.SetParent(VRMiniMap.instance.transform, true);
                        PlayerController.localPlayer.CmdOrderEntityToMoveToPoint(pi.ship.GetEntityId(), pi.transform.localPosition, pi.transform.localRotation);
                        pi.transform.SetParent(ShipPlacementCursor.instance.transform, true);
                    }
                }
                lastTap = Time.time;
            }
            // We are highlighting a ship, so pursue it
            else
            {
                foreach (ShipPlacementIndicator pi in activePlacements)
                {
                    PlayerController.localPlayer.CmdOrderEntityToPursue(pi.ship.GetEntityId(),
                        VRShipSelector.instance.currentSelectables[0].GetShip().GetEntityId(), VRShipSelector.instance.targetIsFriendly);
                }
            }
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