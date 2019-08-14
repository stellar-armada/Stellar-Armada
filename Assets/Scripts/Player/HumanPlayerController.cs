using SpaceCommander.Teams;
using UnityEngine;
using Mirror; // Replacement for HLAPI, uses virtually identical syntax but strips some of HLAPI's functionality/bloat
using SpaceCommander.Game;

#pragma warning disable 0649
namespace SpaceCommander.Players
{
    public class HumanPlayerController : PlayerController
    {
        public static HumanPlayerController localPlayer;
        
        public GameObject localRig; // Stuff that's only for the local player -- cameras, menus, etc.
        
        [SerializeField] BodyController bodyController;

        [SyncVar] public bool isHost; // Keeps track of which player is host, nice for displaying in scoreboard and the like

        private bool playerIsReady;

        private void Start()
        {
            Transform t = transform; // skip the gameObject.transform lookup
            t.parent = EnvironmentTransformRoot.instance.transform;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            
            bodyController.Init();
            
            PlayerManager.instance.RegisterPlayer(this);
            
            if (isServer) TeamManager.instance.CmdJoinTeam(netId); // must happen after register player

            if (isLocalPlayer)
            {
                localRig.SetActive(true);
                
                CmdSetUserName(SettingsManager.GetSavedPlayerName());
                
                localPlayer = this;

                if (isServer)
                {
                    isHost = true; // Shorthand helper
                }
            }
            else
            {
                Destroy(localRig);
            }
        }
        public bool IsLocalPlayer() => isLocalPlayer;

        public bool IsServer() => isServer;

        public bool IsClient() => isClient;
        
        public override PlayerType GetPlayerType() => PlayerType.Player;
    }
}