using System.Collections.Generic;
using SpaceCommander.Players;
using SpaceCommander.Ships;
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
            p.entity = e;
            
            // parent placement markers to placer
            placementIndicatorObject.transform.SetParent(Placer.instance.placementPositionRoot);
            
            // add to list
            placementIndicators.Add(p);

            // deactivate
            placementIndicatorObject.SetActive(false);
        }
    }

}
