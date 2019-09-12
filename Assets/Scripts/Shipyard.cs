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
    public static uint prototypeEntityIncrement = 0;
    public uint id;
    public bool hasCaptain;
    public uint captain;
    public ShipType shipType;
    public int group;
}

#pragma warning disable 0649
public class Shipyard : MonoBehaviour
{
    public static Shipyard instance;

    public GameObject WarpButton;

    public GameObject FlagshipWarning;

    private List<UIShipyardShip> shipyardShips = new List<UIShipyardShip>();

    public List<GroupContainer> shipyardGroups = new List<GroupContainer>();

    public Transform availableShipsContainer;

    [SerializeField] Text availablePointsText;

    private HumanPlayerController localPlayer;

    private Team team;

    void Awake()
    {
        instance = this;
        WarpButton.SetActive(false);
        FlagshipWarning.SetActive(true);
    }


    public void InitializeShipyard()
    {
        Debug.Log("<color=blue>Calling init");
        localPlayer = HumanPlayerController.localPlayer;

        localPlayer.EventOnPlayerTeamChange += HandleTeamChange;

        team = TeamManager.instance.GetTeamByID(localPlayer.teamId);
        team.prototypes.Callback += OnShipListUpdated;
        
        PopulateUIShipyardShips();
        Debug.Log("<color=blue>InitializeShipyard called");
    }

    void HandleTeamChange()
    {
        Debug.Log("<color=blue>Handle team change called");
        if (team != null)
            team.prototypes.Callback -= OnShipListUpdated;
        team = localPlayer.GetTeam();
        team.prototypes.Callback += OnShipListUpdated;
        PopulateUIShipyardShips();
        ShowAvailableShips();
    }

    public void OnShipListUpdated(SyncList<ShipPrototype>.Operation op, int itemindex, ShipPrototype proto)
    {
        switch (op)
        {
            case SyncList<ShipPrototype>.Operation.OP_ADD:
                CreateUIShip(proto);
                break;
            case SyncList<ShipPrototype>.Operation.OP_DIRTY:
            case SyncList<ShipPrototype>.Operation.OP_SET:
                UpdateUIShip(proto);
                break;

            case SyncList<ShipPrototype>.Operation.OP_REMOVE:
                RemoveUIShip(proto);
                break;
        }

        ShowAvailableShips();
        ValidateCaptain();
    }

    void CreateUIShip(ShipPrototype proto)
    {
        // instantiate new shipyard ship
        UIShipyardShip ship = UIShipFactory.instance.CreateShipyardShip(proto.shipType).GetComponent<UIShipyardShip>();
        // set prototypeId to proto's Id
        ship.id = (int) proto.id;
        // Parent to appropriate container
        ship.transform.SetParent(shipyardGroups[proto.group].transform);
        ship.transform.localPosition = Vector3.zero;
        ship.transform.localScale = Vector3.one;
        ship.transform.localRotation = Quaternion.identity;
        // add to list of UI ships
        shipyardShips.Add(ship);
    }

    void RemoveUIShip(ShipPrototype proto)
    {
        // find ship in list
        UIShipyardShip ship = shipyardShips.Single(s => s.id == proto.id);
        // remove from list
        shipyardShips.Remove(ship);
        // destroy object
        Destroy(ship);
        // Recalculate ship costs, etc
        ShowAvailableShips();
    }

    void UpdateUIShip(ShipPrototype proto)
    {
        // Get ship
        UIShipyardShip ship = shipyardShips.Single(s => s.id == proto.id);
        // Set correct container as the parent
        ship.transform.SetParent(shipyardGroups[proto.group].transform);
        // set flagship state
        ship.SetFlagshipStatus();
    }

    void AddShipToList(ShipType type, int group)
    {
        // create new prototype with type and group
        ShipPrototype p = new ShipPrototype();
        p.shipType = type;
        p.group = group;
        p.id = ShipPrototype.prototypeEntityIncrement++;
        // Add to team's prototoype list
        team.prototypes.Add(p);
    }

    void RemoveShipFromList(int prototypeId)
    {
        // Remove ship from team's prorotype list
        ShipPrototype proto = team.prototypes.Single(p => p.id == prototypeId);
        team.prototypes.Remove(proto);

    }


    void PopulateUIShipyardShips()
    {
        Debug.Log("Populating shipyard");
        // Destroy all shipyard ships and clear the list
        foreach (var ship in shipyardShips)
        {
            Destroy(ship);
        }

        shipyardShips = new List<UIShipyardShip>();
        // Foreach ship in the team's list
        foreach (var proto in team.prototypes)
        {
            // Create a new shipyard ship and assign it the prototype's ID
            CreateUIShip(proto);
        }
        
        ShowAvailableShips();
       
    }


    public void ShowAvailableShips()
    {
        int currentPoints = Mathf.Max(0,
            HumanPlayerController.localPlayer.GetTeam().pointsToSpend -
            ShipPriceManager.instance.GetGroupPrice(team.prototypes.ToList()));
        availablePointsText.text = currentPoints.ToString();

        // Get all entity types represented
        SyncListShipType availableShipTypes = team.availableShipTypes;

        // Get all children of the availableship container
        var g = availableShipsContainer.GetComponentsInChildren<UIShipyardShip>();
        List<ShipType> addedShipTypes = new List<ShipType>();

        foreach (UIShipyardShip s in g)
        {
            // If any is already rein our available ships pool, destroy it so we don't have two of any ships
            if (!addedShipTypes.Contains(s.shipType))
            {
                Destroy(s);
                Debug.Log("Destroyed double ship");
                continue;
            }

            addedShipTypes.Add(s.GetPrototype().shipType);

            s.SetFlagshipStatus();

            // if ship costs less than $ available, show it
            if (ShipPriceManager.instance.GetShipPrice(s.shipType) > currentPoints) Destroy(s.gameObject);
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


    public void ValidateCaptain()
    {
        int localPlayerCaptainCount = team.prototypes
            .Where(s => s.hasCaptain && s.captain == HumanPlayerController.localPlayer.netId).Count();

        if (localPlayerCaptainCount == 1)
        {
            WarpButton.SetActive(true);
            FlagshipWarning.SetActive(false);
            return; // If there's a captain for current player, we're good
        }

        // If there's no captain, pick one of the available un-captained ships (the one with the highest price)
        var sortedPrototypes =
            team.prototypes.Where(p => p.hasCaptain == false).OrderByDescending(p => ShipPriceManager.instance.shipPriceDictionary[p.shipType]).ToList();

        WarpButton.SetActive(false);
        FlagshipWarning.SetActive(true);
    }


    public void DestroyShip(UIShipyardShip ship)
    {
        if (ship.id >= 0) // If the id is below zero then it's a new ship, not a fleet ship
            RemoveShipFromList(ship.id);

        Destroy(ship.gameObject);
    }

    public void MoveShip(UIShipyardShip ship, GroupContainer g)
    {
        if (ship.id < 0
        ) // Shipyard ships are inited with an id of -1 -- so this much be a ship in our "available" container, not already in our fleet
        {
            AddShipToList(ship.shipType, g.groupId);
            Destroy(ship.gameObject);
        }
        else
        {
            ShipPrototype proto = team.prototypes.Single(p => p.id == ship.id);
            if (proto.group == g.groupId)
            {
                // Player moved item back to it's original, so we can do nothing
                ship.transform.SetParent(g.transform);
                return;
            }
            
            HumanPlayerController.localPlayer.CmdUpdatePrototype(ship.id, g.groupId);
        }

        ShowAvailableShips();
    }


    public void SetFlagshipForLocalPlayer(UIShipyardShip shipyardShip)
    {
        // If player already has a flagship, unset and dirty it
        if (team.prototypes.Where(p => p.hasCaptain && p.captain == localPlayer.netId).ToArray().Length > 0)
        {
            ShipPrototype proto = team.prototypes.FirstOrDefault(p => p.hasCaptain && p.captain == localPlayer.netId);
            proto.hasCaptain = false;
            int index = team.prototypes.IndexOf(
                team.prototypes.FirstOrDefault(p => p.hasCaptain && p.captain == localPlayer.netId));
            team.prototypes[index] = proto;
        }

        int i = team.prototypes.IndexOf(shipyardShip.GetPrototype());

        HumanPlayerController.localPlayer.CmdSetShipCaptain(localPlayer.netId, i);
    }
}