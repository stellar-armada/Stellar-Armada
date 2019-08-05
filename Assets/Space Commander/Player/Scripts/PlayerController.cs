using SpaceCommander.Teams;
using UnityEngine;
using Mirror; // Replacement for HLAPI, uses virtually identical syntax but strips some of HLAPI's functionality/bloat
using SpaceCommander.Game;


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
        
        [SyncVar(hook = nameof(UpdateName))] string playerName;

        [SyncVar(hook = nameof(HandleTeamChange))] uint teamId = 0;
        
        [SyncVar] bool isHost; // Keeps track of which player is host, nice for displaying in scoreboard and the like

        public static event EventHandler EventOnLocalNetworkPlayerCreated;
        public event EventHandler EventOnPlayerNameChange;
        public event EventHandler EventOnPlayerClassChange;
        public event EventHandler EventOnPlayerTeamChange;
        public event EventHandler EventOnWeaponHandChange;

        public bool playerIsInitialized;

        private Transform t; // Store local transform in an easy reference to reduce lookups through gameObject

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
            
            bodyController.Init();
            PlayerManager.instance.RegisterPlayer(this);
        }

        void HandleStartLocalPlayer()
        {
            PlayerManager.SetLocalPlayer(this);

            if (isServer)
            {
                isHost = true; // Shorthand helper
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
            playerName = nameToChangeTo;
            EventOnPlayerNameChange?.Invoke();
        }

        void HandleTeamChange(uint pTeam)
        {
            if (teamId == pTeam) return;
            teamId = pTeam;
            EventOnPlayerTeamChange?.Invoke();
        }

        public bool IsLocalPlayer()
        {
            return isLocalPlayer;
        }

        public bool IsServer()
        {
            return isServer;
        }

        public bool IsClient()
        {
            return isClient;
        }

        public void RegisterPlayer()
        {
            PlayerManager.instance.RegisterPlayer(this);
        }

        public void UnregisterPlayer()
        {
            PlayerManager.instance.UnregisterPlayer(this);
        }

        public IPlayer GetPlayer()
        {
            return this;
        }

        public PlayerType GetPlayerType()
        {
            return PlayerType.Player;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public uint GetId()
        {
            return netId;
        }

        public string GetName()
        {
            return name;
        }
        
        public bool IsEnemy(IPlayer player)
        {
            //Temporary enemy check -- this could be smarter and more extensible?
            if (player != this) return true;
            return false;
        }

        public Team GetTeam()
        {
            return TeamManager.instance.GetTeamByID(teamId);
        }

        public uint GetTeamId()
        {
            return teamId;
        }

        [Command]
        public void CmdSetTeam(uint _team)
        {
            teamId = _team;
        }
 

        public void SetUserName(string newUserName)
        {
            if (isLocalPlayer)
            {
                CmdSetUserName(newUserName);
            }
        }

        [Command]
        void CmdSetUserName(string newUserName)
        {
            playerName = newUserName;
        }
        
    }
}