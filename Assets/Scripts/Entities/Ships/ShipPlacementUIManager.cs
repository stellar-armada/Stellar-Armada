using System.Collections.Generic;
using StellarArmada.Player;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Entities.Ships{
    public class ShipPlacementUIManager : MonoBehaviour
    {
        // Handles the placement of little indicator points for where ships will go when the commander is placing
        // TO-DO: Add pooling for placement indicators!
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
            placementIndicators = new List<ShipPlacementIndicator>();
        }

        void Start()
        {
            humanPlayerController.EventOnPlayerTeamChange += UpdatePlacementMarkers;
        }

        public void UpdatePlacementMarkers()
        {
            // clear indicators if there already are some
            if (placementIndicators.Count > 0)
                foreach (var entity in placementIndicators)
                    Destroy(entity);
            placementIndicators = new List<ShipPlacementIndicator>();

            // foreach entity in team entities, create a placement marker
            foreach (var e in humanPlayerController.GetTeam().entities)
            {
                // create a new placement indicator of the type of the entity
                placementIndicatorObject = CreatePlacementIndicator();
                p = placementIndicatorObject.GetComponent<ShipPlacementIndicator>();

                // Set references to the entity
                p.entityId = e.GetEntityId(); // cache the ID for future ref
                p.entity = e;

                // add to list
                placementIndicators.Add(p);

            }
        }
    }
}
