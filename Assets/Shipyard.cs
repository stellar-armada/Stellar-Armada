using System.Collections.Generic;
using System.Linq;
using Mirror;
using StellarArmada.Entities.Ships;
using StellarArmada.Player;
using StellarArmada.Teams;
using StellarArmada.UI;
using UnityEngine;
using UnityEngine.UI;

public struct ShipPrototype
{
    public bool hasCaptain;
    public ShipType shipType;
    public int group;
}

public class SyncListShipPrototype : SyncList<ShipPrototype>
{
}

public class Shipyard : MonoBehaviour
{
    public static Shipyard instance;

    private List<UIShipyardShip> shipyardShips = new List<UIShipyardShip>();
    
    public List<GroupContainer> shipyardGroups = new List<GroupContainer>();

    public Transform availableShipsContainer;

    [SerializeField] Text availablePointsText;

    public ShipPrototype flagship; // public for debug

    void Awake()
    {
        instance = this;
    }

    private Team t;

    void Start()
    {
        t = TeamManager.instance.GetTeamByID(HumanPlayerController.localPlayer.teamId);
        t.prototypes.Callback +=
            NetworkUpdateShips; // This is sensitive to team changes and will need refactor for later flexibility
    }
    public void PopulateShipyard()
    {
        UIShipFactory.instance.GetShipyardShips(HumanPlayerController.localPlayer.teamId);
        ShowAvailableShips();
    }
    void PopulateShipyardShips()
    {
        // Delete all shipyard ships
        foreach (var s in shipyardShips)
        {
            Destroy(s.gameObject);
        }
        
        shipyardShips = new List<UIShipyardShip>();

        foreach (var prototype in t.prototypes)
        {
            // Create a new ship
            // Assign prototype to it
            // Set it's parent to the right group container
            // Add it to the ship list
        }
        
    }

    
    public void NetworkUpdateShips(SyncList<ShipPrototype>.Operation op, int itemindex, ShipPrototype proto)
    {
        PopulateShipyardShips();

        ShowAvailableShips();
        }

    public void SetFlagshipForLocalPlayer(UIShipyardShip shipyardShip)
    {
        // shipyard --

        // if local player's flagship is already set / claimed, unset / unclaim
        if (flagship.hasCaptain == true)
        {
            // Make ship available as flagship
            flagship.hasCaptain = false;

            // Unset flag
            //flagship.SetFlagToInactive();
        }

        shipyardShip.prototype.hasCaptain = true;

        flagship = shipyardShip.prototype;

        // flagship.SetFlagToActiveForLocalUser();
    }
    
    public void ShowAvailableShips()
    {

        int currentPoints = Mathf.Max(0, HumanPlayerController.localPlayer.GetTeam().pointsToSpend - ShipPriceManager.instance.GetGroupPrice(t.prototypes.ToList()));
        availablePointsText.text = currentPoints.ToString();
        // Get all entity types represented
        System.Collections.Generic.List<ShipType> availableShipTypes =
            HumanPlayerController.localPlayer.GetTeam().availableShipTypes;

        // Get all children of the availableship container
        //
        List<UIShipyardShip> ships = new List<UIShipyardShip>();
        var g = availableShipsContainer.GetComponentsInChildren<UIShipyardShip>();
        List<ShipType> addedShipTypes = new List<ShipType>();

        foreach (UIShipyardShip s in g)
        {
            // If any is already reped, destroy it
            if (addedShipTypes.Contains(s.prototype.shipType))
            {
                Destroy(s);
                Debug.Log("Destroyed double ship");
                continue;
            }

            addedShipTypes.Add(s.prototype.shipType);
            ships.Add(s);
            // if ship costs less than $ available, show it
            if (ShipPriceManager.instance.GetShipPrice(s) > currentPoints) Destroy(s.gameObject);
            // otherwise, hide it
        }

        // iterate through available ship types
        foreach (ShipType type in availableShipTypes)
        {
            // if any are not represented in the availableship container, create one
            if (!addedShipTypes.Contains(type))
            {
                Transform ship = UIShipFactory.instance.CreateShipyardShip(type).GetComponent<Transform>();
                ship.SetParent(availableShipsContainer);
                ship.localPosition = Vector3.zero;
                ship.localRotation = Quaternion.identity;
                ship.localScale = Vector3.one;
            }
        }
    }

  

    public void MoveShip(UIShipyardShip ship, Transform to)
    {
        // If moving to a group...
        GroupContainer g = to.GetComponent<GroupContainer>();
        if (g != null)
        {
            Transform s = ship.transform;
            s.SetParent(g.transform);
            ship.group = g.groupId;
        }

        // If moving from group to available
        if (to == availableShipsContainer)
        {
            Destroy(ship.gameObject);
        }

        Invoke(nameof(ShowAvailableShips), .02f);
    }

    public void PlaceShipInGroup(UIShipyardShip ship, int group)
    {
        Transform t = ship.transform;
        // A valid group (and not a trash point or something
        if (group > -1 && group < shipyardGroups.Count)
        {
            t.SetParent(shipyardGroups[group].transform);
            t.localScale = Vector3.one;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
        } // Not a valid group (a good way to dispose?)
    }
}