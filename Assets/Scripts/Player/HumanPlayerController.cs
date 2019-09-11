using System.Linq;
using Mirror;
using StellarArmada.Entities;
using UnityEngine;
using StellarArmada.Entities.Ships;
using StellarArmada.Levels;
using StellarArmada.Teams;

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
            OnLocalPlayerInitialized += () => Debug.Log("<color=green>Initialized local player</color>");
            PlayerManager.instance.RegisterPlayer(this);
            if (isLocalPlayer)
            {
                localPlayer = this;
            }
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

        [Command]
        public void CmdSetShipCaptain(uint id, int prototypeIndex)
        {
            Team team = TeamManager.instance.GetTeamByID(teamId);
            ShipPrototype newProto = team.prototypes[prototypeIndex];

            newProto.hasCaptain = true;
            newProto.captain = id;

            // get index of prototype and dirty
            team.prototypes[prototypeIndex] = newProto;

            team.prototypes.Dirty(prototypeIndex); // Do we need this?
        }

        [ClientRpc]
        public void RpcInitialize()
        {
            if (!isServer) Initialize();
        }
        
        void Initialize()
        {
            Debug.Log("<color=blue>Initializing player...</color>");
            bodyController.Init();

            // Server sets player's team
            if (isServer) TeamManager.instance.CmdJoinTeam(netId); // must happen after register player
            
            // If this is the local player's object, set up local player logic
            if (isLocalPlayer)
            {
                // The localrig in the MatchPlayer prefab contains all the local managers for selection, map control, etc.
                localRig.SetActive(true);
                
                CmdSetUserName(PlayerSettingsManager.GetSavedPlayerName());
                
                Debug.Log("Populate shipyard menu here");
                
                LocalMenuStateManager.instance.GoToShipyard();
                
                //localPlayer = this;

                // LocalPlayerRig.instance.Disable();
                OnLocalPlayerInitialized?.Invoke();
            }
            else
            {
                Destroy(localRig);
                OnNonLocalPlayerInitialized?.Invoke();
            }
        }
        
        

        // TO-DO: Refactor for when player selects the capital ship of their choice
        public void PickCapitalShip(Ship ship)
        {
            Debug.Log("Captal ship called");
            // Get entity where capital ship is this player
            Transform t = transform; // skip the gameObject.transform lookup
            t.parent = LocalPlayerBridgeSceneRoot.instance.transform;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
        }


        
        public bool IsLocalPlayer() => isLocalPlayer;

        public bool IsServer() => isServer;

        public bool IsClient() => isClient;
        
        public override PlayerType GetPlayerType() => PlayerType.Player;
    }
}