using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Ships{
    public class ShipPlacementUIManager : MonoBehaviour
    {
        // Handles the placement of little indicator points for where ships will go when the commander is placing
        public static ShipPlacementUIManager instance;
        
        public static List<ShipPlacementIndicator> placementIndicators;

// local reference variables
        private GameObject placementIndicatorObject;
        private ShipPlacementIndicator p;
        
        void Awake()
        {
            instance = this;
            placementIndicators = new List<ShipPlacementIndicator>();
        }
    }
}
