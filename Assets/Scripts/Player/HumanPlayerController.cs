using System.Linq;
using Mirror;
using StellarArmada.Entities;
using UnityEngine;
using StellarArmada.Entities.Ships;
using StellarArmada.Levels;
using StellarArmada.Teams;
using StellarArmada.UI;

#pragma warning disable 0649
namespace StellarArmada.Player
{
    // Human player controller inheriting from the player controller base class
    // Manages setting up the local stuff
    // TO-DO: PickCapitalShip is not automatic
    
    public class HumanPlayerController : PlayerController
    {
        // Handy reference to the local player controller, set when the player inits
        public static HumanPlayerController localPlayer;
        
        public GameObject localRig; // Stuff that's only for the local player -- cameras, menus, etc.
        
        [SerializeField] BodyController bodyController;

        public delegate void PlayerControllerInitializationEvent();

        public PlayerControllerInitializationEvent OnLocalPlayerInitialized;

        public PlayerControllerInitializationEvent OnNonLocalPlayerInitialized;

        
        void Start()
        {
            PlayerManager.instance.RegisterPlayer(this);
            if (isLocalPlayer)
            {
                localPlayer = this;
                localRig.SetActive(true);
            }
            else
            {
                localRig.SetActive(false);
            }
        }

        [Command]
        public void CmdAddShipToList(ShipType type, int group)
        {
            // create new prototype with type and group
            ShipPrototype p = new ShipPrototype();
            p.shipType = type;
            p.group = group;
            p.id = ShipPrototype.prototypeEntityIncrement++;
            // Add to team's prototoype list
            GetTeam().prototypes.Add(p);
        }

        [Command]
        public void CmdRemoveShipFromList(int prototypeId)
        {
            // Remove ship from team's prorotype list
            ShipPrototype proto = GetTeam().prototypes.Single(p => p.id == prototypeId);
            GetTeam().prototypes.Remove(proto);

        }
        
        [Command]
        public void CmdSetFlagshipForLocalPlayer(int newIndex, uint localPlayerId)
        {
            // If player already has a flagship, unset and dirty it
            if (GetTeam().prototypes.Where(p => p.hasCaptain && p.captain == localPlayerId).ToArray().Length > 0)
            {
                ShipPrototype proto = GetTeam().prototypes.FirstOrDefault(p => p.hasCaptain && p.captain == localPlayerId);
                proto.hasCaptain = false;
                int index = GetTeam().prototypes.IndexOf(
                    GetTeam().prototypes.FirstOrDefault(p => p.hasCaptain && p.captain == localPlayerId));
                GetTeam().prototypes[index] = proto;
            }
            
            SetShipCaptain(localPlayerId, newIndex);
            TargetFlagshipSetForPlayer(connectionToClient);
        }

        [TargetRpc]
        void TargetFlagshipSetForPlayer(NetworkConnection conn)
        {
            Debug.Log("<color=green>CLICK</color> TargetFlagshipSetForPlayer()");
        }
        
        [Command]
        public void CmdUpdatePrototype(int shipId, int groupId)
        {
            GetTeam().UpdatePrototype(shipId, groupId);
        }

        [Command]
        public void CmdInitialize()
        {
            Initialize();
            RpcInitialize();
        }

        [Command]
        public void CmdCreateShipsForTeam()
        {
            ShipFactory.instance.CmdCreateShipsForTeam(GetTeam().teamId);

        }

        [Server]
        public void SetShipCaptain(uint id, int prototypeIndex)
        {
            Team team = TeamManager.instance.GetTeamByID(teamId);
            ShipPrototype newProto = team.prototypes[prototypeIndex];

            newProto.hasCaptain = true;
            newProto.captain = id;

            // get index of prototype and dirty
            team.prototypes[prototypeIndex] = newProto;
        }

        [ClientRpc]
        public void RpcInitialize()
        {
            if (!isServer) Initialize();
        }
        
        void Initialize()
        {
            Debug.Log("Initialize called");
            bodyController.Init();

            // Server sets player's team
            if (isServer) TeamManager.instance.CmdJoinTeam(netId); // must happen after register player
            
            // If this is the local player's object, set up local player logic
            if (isLocalPlayer)
            {
                // The localrig in the MatchPlayer prefab contains all the local managers for selection, map control, etc.
                localRig.SetActive(true);
                
                CmdSetUserName(PlayerSettingsManager.GetSavedPlayerName());
                
                LocalMenuStateManager.instance.GoToShipyard();
                
                //localPlayer = this;

                // LocalPlayerRig.instance.Disable();
                OnLocalPlayerInitialized?.Invoke();
            }
            else
            {
                localRig.SetActive(false);
                OnNonLocalPlayerInitialized?.Invoke();
            }
        }
        
        

        // TO-DO: Refactor for when player selects the capital ship of their choice
        public void PickCapitalShip(Ship ship)
        {
            // Get entity where capital ship is this player
            Transform t = transform; // skip the gameObject.transform lookup
            t.parent = LocalPlayerBridgeSceneRoot.instance.transform;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            ShipSelectionManager.instance.InitializeSelectionSets();
        }


        
        public bool IsLocalPlayer() => isLocalPlayer;

        public bool IsServer() => isServer;

        public bool IsClient() => isClient;
        
        public override PlayerType GetPlayerType() => PlayerType.Player;
    }
}