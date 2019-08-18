using SpaceCommander.Ships;
using UnityEngine;
using VRKeys;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager instance;
    
    public ShipDictionary placementShipPrefabs;

    public GameObject CreatePlacementShip(ShipType type)
    {
        return Instantiate(placementShipPrefabs[type]);
    }

    void Awake()
    {
        instance = this;
    }

}
