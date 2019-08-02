using System.Collections.Generic;
using SpaceCommander.Audio;
using UnityEngine;
using Mirror; // Replacement for HLAPI, uses virtually identical syntax but strips some of HLAPI's functionality/bloat
using SpaceCommander.UI;
using InputHandling;
using SpaceCommander.Game;
using SpaceCommander.Level;
using SpaceCommander.Ships;


namespace SpaceCommander.Player
{
    /* Main spawnable Player object script, instantiated by the NetworkManager when a new network session starts
     * All of the player-related networking logic is here (Level and match networking logic can be found on Match objects)
     * When local player spawns, the local player rig (camera, controller, ui, etc) is parented to this object
     * The main player object doesn't move in space, but the head and hand positions will be relayed over the network (look at TransformChild components on Player prefab)
     * Non-networked events are all called by SyncVar state change, subscribe your methods to them
     * If you don't understand what [ClientRpc] and [Command] do, google them + Unity HLAPI
     */

    public class PlayerController : NetworkBehaviour, IPlayer
    {
        public static PlayerController localInstance; // local player accessor
        
        public delegate void EventHandler();
        
        [SerializeField] BodyController bodyController;
        
        [SyncVar(hook = nameof(UpdateName))] string userName;

        [SyncVar(hook = nameof(HandleTeamChange))] int team = 0;
        
        [SyncVar] bool isHost; // Keeps track of which player is host, nice for displaying in scoreboard and the like

        public static event EventHandler EventOnLocalNetworkPlayerCreated;
        public event EventHandler EventOnPlayerNameChange;
        public event EventHandler EventOnPlayerClassChange;
        public event EventHandler EventOnPlayerTeamChange;
        public event EventHandler EventOnWeaponHandChange;

        public bool playerIsInitialized;

        private Transform t; // Store local transform in an easy reference to reduce lookups through gameObject
        
        int currentSound = 0;

        private bool playerIsReady;

        public static void ClearDelegates()
        {
            EventOnLocalNetworkPlayerCreated = null;
        }

        private void Start()
        {
            t = transform; // skip the gameObject.transform lookup
            bodyController = GetComponent<BodyController>();

            if (isLocalPlayer)
            {
                Debug.Log("Player is local player");
                
                HandleStartLocalPlayer();
                if (isServer)
                {
                    Debug.Log("Player is server");

                    //search for anchors, if none exist, create one automatically
                    //long lastAnchor = PlayerPrefs.GetInt("LastAnchor");
                    //AzureSpatialAnchorsController.OnAnchorFound += AnchorFound;
                    GameManager.CreateServerObject(); // Holds server-wide variables
                }
            }
            
            t.parent = LevelManager.instance.sceneRoot;
            bodyController.Init();
            PlayerManager.instance.RegisterPlayer(this);
        }

        void HandleStartLocalPlayer()
        {
            PlayerManager.SetLocalPlayer(this);

            if (isServer)
            {
                isHost = true; // Shorthand helper
                GameManager.instance.CmdCreateNewMatch();
            }

            LocalRig.instance.transform.parent = t;
            
            SetUserName(SettingsManager.GetSavedPlayerName());
            
            EventOnLocalNetworkPlayerCreated?.Invoke();
            
            LocalCameraController.instance.ShowLevelAndPlayers();
        }
        
        
        public bool IsGameHost()
        {
            return isHost;
        }

        void UpdateName(string nameToChangeTo)
        {
            t.name = nameToChangeTo;
            userName = nameToChangeTo;
            EventOnPlayerNameChange?.Invoke();
        }

        void HandleTeamChange(int pTeam)
        {
            if (team == pTeam) return;
            team = pTeam;
            EventOnPlayerTeamChange?.Invoke();
        }

        public bool IsLocalPlayer()
        {
            throw new System.NotImplementedException();
        }

        public bool IsServer()
        {
            throw new System.NotImplementedException();
        }

        public bool IsClient()
        {
            throw new System.NotImplementedException();
        }

        public void RegisterPlayer()
        {
            throw new System.NotImplementedException();
        }

        public void UnregisterPlayer()
        {
            throw new System.NotImplementedException();
        }

        public IPlayer GetPlayer()
        {
            throw new System.NotImplementedException();
        }

        public PlayerType GetPlayerType()
        {
            throw new System.NotImplementedException();
        }

        public GameObject GetGameObject()
        {
            throw new System.NotImplementedException();
        }

        Team IPlayer.GetTeam()
        {
            throw new System.NotImplementedException();
        }

        public uint GetId()
        {
            throw new System.NotImplementedException();
        }

        public string GetName()
        {
            throw new System.NotImplementedException();
        }

        public int GetTeam()
        {
            return team;
        }

        [Command]
        public void CmdSetTeam(int _team)
        {
            team = _team;
        }
        
        public string GetUserName()
        {
            return userName;
        }

        public void SetUserName(string newUserName)
        {
            if (isLocalPlayer)
            {
                CmdSetUserName(newUserName);
            }
        }

        public BodyController GetBodyController()
        {
            return bodyController;
        }

        [Command]
        void CmdSetUserName(string newUserName)
        {
            userName = newUserName;
        }
        
    }
}