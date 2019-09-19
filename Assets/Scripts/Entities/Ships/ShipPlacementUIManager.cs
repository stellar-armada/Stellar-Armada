using System.Collections.Generic;
using StellarArmada.Player;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Entities.Ships{
    public class ShipPlacementUIManager : MonoBehaviour
    {
        // Handles the placement of little indicator points for where ships will go when the commander is placing
        public static ShipPlacementUIManager instance;

        public List<ShipPlacementIndicator> placementIndicators = new List<ShipPlacementIndicator>();

        void Awake()
        {
            instance = this;
        }
    }
}
