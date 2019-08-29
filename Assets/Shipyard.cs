using System.Linq;
using Boo.Lang;
using StellarArmada.Entities.Ships;
using StellarArmada.Player;
using StellarArmada.UI;
using UnityEngine;
using UnityEngine.UI;

public class Shipyard : MonoBehaviour
{
    public static Shipyard instance;
    
    public System.Collections.Generic.List<GroupContainer> shipyardGroups = new System.Collections.Generic.List<GroupContainer>();

    public Transform availableShipsContainer;
    
    [SerializeField] Text availablePointsText;
    
    [HideInInspector] public UIShipyardShip flagship;

    void Awake()
    {
        instance = this;
    }
    
    // tell shipyard that a ship of this type in this group is the flagship

    public void SetFlagshipForLocalPlayer(UIShipyardShip shipyardShip)
    {
        // shipyard --
            
        // if local player's flagship is already set / claimed, unset / unclaim
        if (flagship != null)
        {
            // Make ship available as flagship
            flagship.hasCaptain = false;
            
            // Unset flag
            flagship.SetFlagToInactive();
            
            flagship = null;
        }

        shipyardShip.hasCaptain = true;

        flagship = shipyardShip;
        
        flagship.SetFlagToActiveForLocalUser();

        // TO-DO: make ship not available as a flagship


    }

    public int ComputeCurrentShipCost()
    {
        System.Collections.Generic.List<UIShipyardShip> ships = new System.Collections.Generic.List<UIShipyardShip>();
        foreach (var group in shipyardGroups)
        {
           var g = group.GetComponentsInChildren<UIShipyardShip>();
            foreach (UIShipyardShip s in g)
            {
                ships.Add(s);
            }
        }

        return ShipPriceManager.instance.GetGroupPrice(ships);
    }

    public int ComputeAvailablePoints()
    {
        return Mathf.Max(0, HumanPlayerController.localPlayer.GetTeam().pointsToSpend - ComputeCurrentShipCost());
    }
    
    public void ShowAvailableShips()
    {
        int currentPoints = ComputeAvailablePoints();
        
        // Get all entity types represented
        System.Collections.Generic.List<ShipType> availableShipTypes = HumanPlayerController.localPlayer.GetTeam().availableShipTypes;
        
        // Get all children of the availableship container
        System.Collections.Generic.List<UIShipyardShip> ships = new System.Collections.Generic.List<UIShipyardShip>();
        var g = availableShipsContainer.GetComponentsInChildren<UIShipyardShip>();
        List<ShipType> addedShipTypes = new List<ShipType>();
        
        foreach (UIShipyardShip s in g)
        {
            // If any is already reped, destroy it
            if (addedShipTypes.Contains(s.shipType))
            {
                Destroy(s);
                continue;
            }

            addedShipTypes.Add(s.shipType); 
            ships.Add(s);
            // if ship costs less than $ available, show it
            if (ShipPriceManager.instance.GetShipPrice(s) >= currentPoints) s.gameObject.SetActive(true);
            // otherwise, hide it
            else s.gameObject.SetActive(false);
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
    
    public void PopulateShipyard()
    {
        UIShipFactory.instance.GetShipyardShips(HumanPlayerController.localPlayer.teamId);
        ShowAvailableShips();
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
       
        ShowAvailableShips();
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
        else
        {
            
        }
    }
    
}
