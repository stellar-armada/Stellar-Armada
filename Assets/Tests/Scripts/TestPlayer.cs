using Mirror;
using SpaceCommander.Game;
using SpaceCommander.Teams;
using UnityEngine;

#pragma warning disable 0649
namespace SpaceCommander.Common.Tests
{
    public class TestPlayer : NetworkBehaviour, IPlayer
    {
        [SyncVar(hook=nameof(HandleTeamChange))] public uint teamId;
        public string playerName = "TestPlayer";

        public bool isEnemy = true;

        public override void OnStartServer()
        {
            
            
        }

        void HandleTeamChange(uint newTeamId)
        {
            Debug.Log("Team changed to " +  newTeamId);
        }
        
        void Awake()
        {
            RegisterPlayer();
            GameObject[] gos = GameObject.FindObjectsOfType<GameObject>();

            foreach (GameObject go in gos)
            {
                if (GetComponent<TestPlayerEntity>() != null)
                {
                    GetComponent<TestPlayerEntity>().SetPlayer(this);
                }
            }
        }

        public bool IsLocalPlayer() => isLocalPlayer;

        public bool IsServer() => isServer;

        public bool IsClient() => isClient;
        public void RegisterPlayer() => PlayerManager.instance.RegisterPlayer(this);

        public void UnregisterPlayer() => PlayerManager.instance.UnregisterPlayer(this);

        public IPlayer GetPlayer() => this;

        public PlayerType GetPlayerType() => PlayerType.None;

        public GameObject GetGameObject() => gameObject;
        public Team GetTeam() => TeamManager.instance.GetTeamByID(teamId);
        public uint GetTeamId() => teamId;
        public void SetTeamId(uint teamId)
        {
            throw new System.NotImplementedException();
        }

        public uint GetId() => netId;

        public string GetName() => playerName;
        
        public bool IsEnemy(IPlayer player)
        {
            return isEnemy;
        }
    }
}
    
