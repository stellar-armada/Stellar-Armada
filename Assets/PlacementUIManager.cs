using System.Collections.Generic;
using StellarArmada.Players;
using StellarArmada.Ships;
using UnityEngine;

public class PlacementUIManager : MonoBehaviour
{
    public static PlacementUIManager instance;

    [SerializeField] private HumanPlayerController humanPlayerController;
    
    public ShipDictionary placementShipPrefabs;

    public static List<PlacementIndicator> placementIndicators;

    public GameObject CreatePlacementShip(ShipType type)
    {
        return Instantiate(placementShipPrefabs[type]);
    }

    void Awake()
    {
        instance = this;
        placementIndicators = new List<PlacementIndicator>();
    }
    
    void Start()
    {
        UpdatePlacementMarkers();
        humanPlayerController.EventOnPlayerTeamChange += UpdatePlacementMarkers;
    }

    public void UpdatePlacementMarkers()
    {
        Debug.Log("UpdatePlacementMarkers called");
        // clear indicators if there already are some
        if(placementIndicators.Count > 0)
            foreach (var entity in placementIndicators)
                Destroy(entity);
        placementIndicators = new List<PlacementIndicator>();

        // foreach entity in team entities, create a placement marker
        foreach (var e in humanPlayerController.GetTeam().entities)
        {
            // create a new placement indicator of the type of the entity
            GameObject placementIndicatorObject = CreatePlacementShip(((Ship) e).type); // Hard casting for now, should be entity wide in the future
            PlacementIndicator p = placementIndicatorObject.GetComponent<PlacementIndicator>();
            
            // Set references to the entity
            p.entityId = e.GetEntityId(); // cache the ID for future ref
            Debug.Log("Entity ID: " + e.GetEntityId());
            p.entity = e;

            // add to list
            placementIndicators.Add(p);

        }
    }

}
