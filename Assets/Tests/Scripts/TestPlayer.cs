using Mirror;
using SpaceCommander.Game;
using SpaceCommander.Teams;
using UnityEngine;

#pragma warning disable 0649
namespace SpaceCommander.Players.Tests
{
    public class TestPlayerController : PlayerController
    {
        [SyncVar(hook=nameof(HandleTeamChange))] public uint teamId;
        public string playerName = "TestPlayer";

        public bool isEnemy = true;

        public override void HandleTeamChange(uint newTeamId)
        {
            Debug.Log("Team changed to " +  newTeamId);
        }
        
        void Awake()
        {
            RegisterPlayer();
            GameObject[] gos = GameObject.FindObjectsOfType<GameObject>();

            foreach (GameObject go in gos)
            {
                if (GetComponent<IPlayerEntity>() != null)
                {
                    GetComponent<IPlayerEntity>().CmdSetPlayer(netId);
                }
            }
        }

        public bool IsLocalPlayer() => isLocalPlayer;

        public bool IsServer() => isServer;

        public bool IsClient() => isClient;
        public void RegisterPlayer() => PlayerManager.instance.RegisterPlayer(this);

        public void UnregisterPlayer() => PlayerManager.instance.UnregisterPlayer(this);
        
        public override PlayerType GetPlayerType() => PlayerType.None;
        
        // Always be enemy. Or not.
        public override bool IsEnemy(PlayerController playerController)
        {
            return isEnemy;
        }
    }
}