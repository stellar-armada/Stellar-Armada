using System.Collections.Generic;
using StellarArmada.Player;
using StellarArmada.UI;
using UnityEngine;

public class Shipyard : MonoBehaviour
{
    public static Shipyard instance;
    
    public List<ShipyardGroup> shipyardGroups = new List<ShipyardGroup>();

    void Awake()
    {
        instance = this;
    }
    public void PopulateShipyard()
    {
        Debug.Log("Mocking shipyard population");
        Debug.Log("Local player controler: " + (HumanPlayerController.localPlayer != null));
        Debug.Log("Team: " + HumanPlayerController.localPlayer.teamId);
        Debug.Log("Populating shipyard...");
        var shipyardShips = UIShipFactory.instance.GetShipyardShips(HumanPlayerController.localPlayer.teamId);
        Debug.Log("<color=green>Successfully created shipyard ships. Count: " + shipyardShips);
    }

    public void PlaceShipInGroup(UIShipyardShip ship, int group)
    {
        ship.transform.SetParent(shipyardGroups[group].transform);
        ship.transform.localScale = Vector3.one;
    }
    
}
