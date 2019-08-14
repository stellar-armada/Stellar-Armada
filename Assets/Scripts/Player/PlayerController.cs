using SpaceCommander.Teams;
using UnityEngine;
using Mirror; // Replacement for HLAPI, uses virtually identical syntax but strips some of HLAPI's functionality/bloat
using SpaceCommander.Game;

#pragma warning disable 0649
namespace SpaceCommander.Player
{
    public class PlayerController : NetworkBehaviour, IPlayer
    {
        public delegate void EventHandler();

        public GameObject localRig; // Stuff that's only for the local player -- cameras, menus, etc.
        
        [SerializeField] BodyController bodyController;
        
        [SyncVar(hook = nameof(UpdateName))] public string playerName;

        [SyncVar(hook = nameof(HandleTeamChange))] public uint teamId = 255;
        
        [SyncVar] public bool isHost; // Keeps track of which player is host, nice for displaying in scoreboard and the like

        public event EventHandler EventOnPlayerNameChange;
        public event EventHandler EventOnPlayerTeamChange;
        
        private Transform t; // Store local transform in an easy reference to reduce lookups through gameObject

        private bool playerIsReady;

        public static PlayerController localPlayer;
        
        private void Start()
        {
            t = transform; // skip the gameObject.transform lookup
            t.parent = EnvironmentTransformRoot.instance.transform;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            
            bodyController.Init();
            PlayerManager.instance.RegisterPlayer(this);
            
            if (isServer) TeamManager.instance.CmdJoinTeam(netId); // must happen after register player

            if (isLocalPlayer)
            {
                localRig.SetActive(true);
                Debug.Log("Player is local player");
                CmdSetUserName(SettingsManager.GetSavedPlayerName());
                
                localPlayer = this;
                Debug.Log("Called activation code");

                if (isServer)
                {
                    isHost = true; // Shorthand helper
                    Debug.Log("Player is server");
                }
            }
            else
            {
                Destroy(localRig);
            }
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

        public bool IsLocalPlayer() => isLocalPlayer;

        public bool IsServer() => isServer;

        public bool IsClient() => isClient;

        public IPlayer GetPlayer() => this;

        public PlayerType GetPlayerType() => PlayerType.Player;

        public GameObject GetGameObject() => gameObject;

        public uint GetId() => netId;

        public string GetName() => name;

        public bool IsEnemy(IPlayer player) => player.GetTeamId() != teamId;

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