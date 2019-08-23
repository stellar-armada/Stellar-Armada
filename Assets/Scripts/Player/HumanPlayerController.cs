using UnityEngine;
using StellarArmada.Game;
using StellarArmada.Ships;
using StellarArmada.Teams;

#pragma warning disable 0649
namespace StellarArmada.Players
{
    public class HumanPlayerController : PlayerController
    {
        public static HumanPlayerController localPlayer;
        
        public GameObject localRig; // Stuff that's only for the local player -- cameras, menus, etc.
        
        [SerializeField] BodyController bodyController;
        
        private bool playerIsReady;

        private void Awake()
        {

            
            PlayerManager.instance.RegisterPlayer(this);

            localRig.SetActive(false);
            
            
        }

        void Start()
        {
            bodyController.Init();

            if (isServer) TeamManager.instance.CmdJoinTeam(netId); // must happen after register player
            
            if (isLocalPlayer)
            {
                localRig.SetActive(true);
                
                CmdSetUserName(SettingsManager.GetSavedPlayerName());
                
                localPlayer = this;

                Invoke(nameof(PickCapitalShip), .5f);
            }
            else
            {
                Destroy(localRig);
            }
        }

        void PickCapitalShip()
        {
            foreach (NetworkEntity e in GetTeam().entities)
            {
                Ship s = (Ship) e;
                if (s.availableAsFlagship)
                {
                    s.bridge.ActivateBridgeForLocalPlayer();
                    Transform t = transform; // skip the gameObject.transform lookup
                    t.parent = SceneRoot.instance.transform;
                    t.localPosition = Vector3.zero;
                    t.localRotation = Quaternion.identity;
                    return;
                }
            }
        }
        public bool IsLocalPlayer() => isLocalPlayer;

        public bool IsServer() => isServer;

        public bool IsClient() => isClient;
        
        public override PlayerType GetPlayerType() => PlayerType.Player;
    }
}