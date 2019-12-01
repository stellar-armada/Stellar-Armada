using System.Linq;
using Mirror;
using StellarArmada.Ships;
using UnityEngine;
using StellarArmada.Match;
using StellarArmada.Teams;
using ShipFactory = StellarArmada.Ships.ShipFactory;

#pragma warning disable 0649
namespace StellarArmada.Player
{
    // Human player controller inheriting from the player controller base class
    // Manages setting up the local stuff
    // TO-DO: PickCapitalShip is not automatic
    
    public class PlayerController : NetworkBehaviour
    {
        // Handy reference to the local player controller, set when the player inits
        public static PlayerController localPlayer;
        
        public GameObject localRig; // Stuff that's only for the local player -- cameras, menus, etc.
        
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
        public void CmdOrderEntityToStop(uint entityId)
        {
            Ship ship = ShipManager.GetEntityById(entityId);
            ship.movement.ServerStopMovement();
        }

        [Command]
        public void CmdOrderEntityToPursue(uint pursuerId, uint quarryId, bool friendly)
        {
            Ship pursuerShip = ShipManager.GetEntityById(pursuerId);
            Ship quarryShip = ShipManager.GetEntityById(quarryId);
            pursuerShip.movement.ServerPursue(quarryShip.GetEntityId());
            pursuerShip.weaponSystemController.ServerSetTarget(quarryShip.GetEntityId(), friendly);
        }
        
        [Command]
        public void CmdOrderEntityToMoveToPoint(uint entityId, Vector3 pos, Quaternion rot)
        {
            Ship ship = ShipManager.GetEntityById(entityId);
            ship.movement.ServerMoveToPoint(pos, rot);
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
                Debug.Log("Player already has");
                
                ShipPrototype proto = GetTeam().prototypes.FirstOrDefault(p => p.hasCaptain && p.captain == localPlayerId);
                proto.hasCaptain = false;
                int index = GetTeam().prototypes.IndexOf(
                    GetTeam().prototypes.FirstOrDefault(p => p.hasCaptain && p.captain == localPlayerId));
                Debug.Log("Index is: " + index);
                GetTeam().prototypes[index] = proto;

            }
            Debug.Log("SetShipCaptain");

            SetShipCaptain(localPlayerId, newIndex);
            
        }
        
        [Command]
        public void CmdSetRandomFlagshipForLocalPlayer(uint localPlayerId)
        {
            // TO-DO -- this whole check routine could be abstracted to another function
            
            // If player already has a flagship, unset and dirty it
            if (GetTeam().prototypes.Where(shipPrototype => shipPrototype.hasCaptain && shipPrototype.captain == localPlayerId).ToArray().Length > 0)
            {
                Debug.Log("Player already has");
                
                ShipPrototype proto = GetTeam().prototypes.FirstOrDefault(shipPrototype => shipPrototype.hasCaptain && shipPrototype.captain == localPlayerId);
                proto.hasCaptain = false;
                int index = GetTeam().prototypes.IndexOf(
                    GetTeam().prototypes.FirstOrDefault(shipPrototype => shipPrototype.hasCaptain && shipPrototype.captain == localPlayerId));
                Debug.Log("Index is: " + index);
                GetTeam().prototypes[index] = proto;

            }
            Debug.Log("SetShipCaptain");

            // Get the first ship that doesn't have a captain on this team
            var p = GetTeam().prototypes.Where(shipPrototype => !shipPrototype.hasCaptain).ToArray()[0];

                SetShipCaptain(localPlayerId, GetTeam().prototypes.IndexOf(p));
            
        }
        
        [Command]
        public void CmdUpdatePrototype(int shipId, int groupId)
        {
            GetTeam().UpdatePrototype(shipId, groupId);
        }

        [Command]
        public void CmdInitialize()
        {
            MatchStateManager.instance.CmdChangeMatchState(MatchState.Lobby);
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
            Debug.Log("Setting shpi captain");
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
            // Server sets player's team
            if (isServer) TeamManager.instance.CmdJoinTeam(netId); // must happen after register player
            
            // If this is the local player's object, set up local player logic
            if (isLocalPlayer)
            {
                // The localrig in the MatchPlayer prefab contains all the local managers for selection, map control, etc.
                localRig.SetActive(true);
                
                CmdSetUserName(PlayerSettingsManager.GetSavedPlayerName());
                
                Shipyard.instance.InitializeShipyard();
                
                LocalMenuStateManager.instance.GoToShipyard();
                
                OnLocalPlayerInitialized?.Invoke();
            }
            else
            {
                localRig.SetActive(false);
                OnNonLocalPlayerInitialized?.Invoke();
            }
        }

        public bool IsLocalPlayer() => isLocalPlayer;

        public bool IsServer() => isServer;

        public bool IsClient() => isClient;
        
        public PlayerType GetPlayerType() => PlayerType.Player;
        
        // Generic event handler for this class
        public delegate void PlayerControllerEvent();

        // Generic event handler for this class
        public delegate void PlayerControllerSpecificEvent(PlayerController playerController);

        // Set the player name. When set by the server, call the UpdateName callback
        [SyncVar(hook = nameof(UpdateName))] public string playerName;

        // Set the player's team
        //UInts are used here, mostly because uints are used for net IDs and incremented entity index values
        [SyncVar(hook = nameof(HandleTeamChange))]
        public uint teamId = 255;

        public event PlayerControllerEvent EventOnPlayerNameChange;
        public event PlayerControllerEvent EventOnPlayerTeamChange;

        public PlayerControllerSpecificEvent OnPlayerControllerDeath;

        [SyncVar] public bool isAlive = true;

        [ServerCallback]
        public void HandleWin()
        {
            if (isServer) TargetHandleWin(connectionToClient);
        }
        
        [ServerCallback]
        public void HandleLoss()
        {
            if (isServer) TargetHandleLoss(connectionToClient);
        }

        [TargetRpc]
        public void TargetHandleWin(NetworkConnection conn)
        {
            ReturnToPurgatory();
            LocalMenuStateManager.instance.ShowVictoryMenu();
        }
        
        [TargetRpc]
        public void TargetHandleLoss(NetworkConnection conn)
        {
            ReturnToPurgatory();
            LocalMenuStateManager.instance.ShowDefeatMenu();
        }

        void ReturnToPurgatory()
        {
            transform.SetParent(PurgatoryRoot.instance.transform);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            PlayerCamera.instance.ShowPurgatoryView();
         
        }

        [Server]
        public void Die() // Message sent to all players attached to an entity when it dies
        {
            if (isServerOnly)
            {
                isAlive = false;
                OnPlayerControllerDeath?.Invoke(this);
            }

            // Our ship has died, so we are dead
            Debug.Log("<color=red>DEATH</color> Player " + netId + " has died. (server)");
            RpcDie();
        }

        [ClientRpc]
        public void RpcDie()
        {
            isAlive = false;
            ReturnToPurgatory();
            OnPlayerControllerDeath?.Invoke(this);
        }


        // Called when the server updates the player's name variable
        protected void UpdateName(string nameToChangeTo)
        {
            transform.name = nameToChangeTo;
            playerName = nameToChangeTo;
            EventOnPlayerNameChange?.Invoke();
        }

        // Called when the server updates the player's team
        public void HandleTeamChange(uint pTeam)
        {
            if (teamId == pTeam) return;
            teamId = pTeam;
            EventOnPlayerTeamChange?.Invoke();
        }
        public PlayerController GetPlayer() => this;
        public GameObject GetGameObject() => gameObject;

        public uint GetId() => netId;

        public string GetName() => name;

        public bool IsEnemy(PlayerController playerController) => playerController.GetTeamId() != teamId;

        public Team GetTeam() => TeamManager.instance.GetTeamByID(teamId);
        public uint GetTeamId() => teamId;
        public void SetTeamId(uint newTeamId) => teamId = newTeamId;

        [Command]
        public void CmdSetTeam(uint _team) => teamId = _team;

        [Command]
        public void CmdSetUserName(string newUserName)
        {
            playerName = newUserName;
        }
    }
}