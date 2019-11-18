using System.Collections.Generic;
using StellarArmada.Player;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Entities.Ships{
    public class ShipPlacementUIManager : MonoBehaviour
    {
        // Handles the placement of little indicator points for where ships will go when the commander is placing
        public static ShipPlacementUIManager instance;

        [SerializeField] private PlayerController humanPlayerController;

        public GameObject placementIndicatorPrefab;

        public static List<ShipPlacementIndicator> placementIndicators;

// local reference variables
        private GameObject placementIndicatorObject;
        private ShipPlacementIndicator p;
        
        public GameObject CreatePlacementIndicator()
        {
            return Instantiate(placementIndicatorPrefab);
        }

        void Awake()
        {
            instance = this;
        }
    }
}
