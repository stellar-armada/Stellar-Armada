using System.Collections.Generic;
using System.Linq;
using StellarArmada.IO;
using StellarArmada.Levels;
using StellarArmada.Player;
using UnityEngine;

namespace StellarArmada.Entities.Ships
{
#pragma warning disable 0649
    public class ShipPlacer : MonoBehaviour
    {
        public static ShipPlacer instance;

        private List<ShipPlacementIndicator> activePlacements = new List<ShipPlacementIndicator>();

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            ShipSelectionManager.instance.OnSelectionChanged += ShowPlacements;
            VRShipSelector.instance.OnHighlightTargetSet += HandleShipHighlighted;
        }

        private bool lastVal;

        void HandleShipHighlighted(bool on)
        {
            if (on == lastVal) return;
            lastVal = on;
            if (on) HidePlacements();
            else ShowPlacements();
        }

        public void ShowPlacements()
        {
            if (IsInvoking(nameof(ShowPlacements))) CancelInvoke(nameof(ShowPlacements));

            HidePlacements(); // reset our placements for another round of formation calculations

            // Get current ships
            List<Ship> ships = ShipSelectionManager.instance.GetCurrentSelection()
                .Select(selectable => selectable.GetOwningEntity() as Ship).ToList();

            // Get formation positions for selection
            var positions = VrShipShipFormationManager.instance.GetFormationPositionsForShips(ships);

            foreach (Ship s in ships)
            {
                // Get the placement indicator
                ShipPlacementIndicator pI = ShipPlacementUIManager.instance.placementIndicators.Single(p => p.entity == s);

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

        public void HidePlacements()
        {
            if (IsInvoking(nameof(ShowPlacements))) CancelInvoke(nameof(ShowPlacements));
            foreach (ShipPlacementIndicator pI in activePlacements)
                pI.Hide();
            activePlacements = new List<ShipPlacementIndicator>();
        }
        private float doubleTapThreshold = .5f;

        private float lastTap = 0;

        void StopAllShips()
        {
            foreach (ShipPlacementIndicator pi in activePlacements)
            {
                PlayerController.localPlayer.CmdOrderEntityToStop(pi.entity.GetEntityId());
            }
        }


        public void Place()
        {
            // If we're not highlighting a ship
            if (VRShipSelector.instance.currentSelectables.Count == 0)
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
                        PlayerController.localPlayer.CmdOrderEntityToMoveToPoint(pi.entity.GetEntityId(), pi.transform.localPosition, pi.transform.localRotation);
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
                    PlayerController.localPlayer.CmdOrderEntityToPursue(pi.entity.GetEntityId(),
                        VRShipSelector.instance.currentSelectables[0].GetOwningEntity().GetEntityId(), VRShipSelector.instance.targetIsFriendly);
                }
            }
        }
    }
}