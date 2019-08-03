using Mirror;
using SpaceCommander.Game;
using SpaceCommander.Teams;
using SpaceCommander.Weapons;
using UnityEngine;

namespace SpaceCommander.Common.Tests
{
    public class TestPlayer : NetworkBehaviour, IPlayer
    {
        public uint teamId;
        public string playerName = "TestPlayer";

        public bool isEnemy;
        
        void Awake()
        {
            RegisterPlayer();
            FindObjectOfType<TestPlayerEntity>().SetPlayer(this);
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

        public uint GetId() => netId;

        public string GetName() => playerName;
        
        public bool IsEnemy(IPlayer player)
        {
            return isEnemy;
        }
    }
}
    
