using System.Collections.Generic;
using StellarArmada.Player;
using UnityEngine;

namespace StellarArmada.Ships
{
#pragma warning disable 0649
    public abstract class ShipPlacer : MonoBehaviour
    {
        public static ShipPlacer instance;

        protected List<ShipPlacementIndicator> activePlacements = new List<ShipPlacementIndicator>();
  
        protected bool lastVal;
        
        protected float doubleTapThreshold = .5f;

        protected float lastTap = 0;
        void Awake() => instance = this;

        void Start()
        {
            ShipSelectionManager.instance.OnSelectionChanged += ShowPlacements;
            ShipSelector.instance.OnHighlightTargetSet += HandleShipHighlighted;
        }

        public abstract void ShowPlacements();

        public abstract void Place(bool sendToFormation);

        protected abstract void HidePlacements();

        protected void StopAllShips()
        {
            foreach (ShipPlacementIndicator pi in activePlacements)
            {
                PlayerController.localPlayer.CmdOrderEntityToStop(pi.ship.GetEntityId());
            }
        }

        void HandleShipHighlighted(bool on)
        {
            if (on == lastVal) return;
            lastVal = on;
            if (on) HidePlacements();
            else ShowPlacements();
        }
    }
}