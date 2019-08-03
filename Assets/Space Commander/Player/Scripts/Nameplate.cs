using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SpaceCommander.Game;
using SpaceCommander.Teams;

namespace SpaceCommander.Player
{
    // Class to control the nameplate (health and name) over player's head
    public class Nameplate : MonoBehaviour
    {
        [SerializeField] TextMeshPro playerName;
        [SerializeField] PlayerController playerController;
        [SerializeField] Transform transformToFollow;
        [SerializeField] Vector3 transformOffset;
        [SerializeField] Slider playerHealth;
        [SerializeField] private Image healthBarFill;

        [SerializeField] IPlayer player;

        public IPlayer localPlayer;
        
        private Transform _t;

        private bool isInited = false;
        private bool isLocalPlayer;
        private Color color;
        
        #region Initialization and Deinitialization

        void Awake()
        {
            playerController.EventOnPlayerNameChange += HandlePlayerNameChange;
            playerController.EventOnPlayerTeamChange += HandleTeamChange;
            PlayerController.EventOnLocalNetworkPlayerCreated += Init;

            if (PlayerManager.GetLocalNetworkPlayer().GetId() == player.GetId())
                Init();
        }

        void Init()
        {            
            _t = transform;
            localPlayer = PlayerManager.GetLocalNetworkPlayer(); // this is who the nameplate will face
            HandlePlayerNameChange();
            HandleTeamChange();
            isInited = true;
            if (PlayerManager.GetLocalNetworkPlayer().IsLocalPlayer()) isLocalPlayer = true;
        }

        #endregion

        #region Private Methods

        void LateUpdate()
        {
            if (!isInited) return;
            _t.position = transformToFollow.position + transformOffset; // Follow player
            if (!isLocalPlayer) FaceLocalPlayer();
        }

        void FaceLocalPlayer()
        {
            _t.LookAt(localPlayer.GetGameObject().transform);
            _t.rotation = Quaternion.Euler(0, _t.rotation.eulerAngles.y + 180f, 0);
        }

        #endregion

        #region Public Methods

        public void HandlePlayerNameChange()
        {
            playerName.text = playerController.GetName();
        }

        public void HandleTeamChange()
        {
            color = TeamManager.instance.infos[playerController.GetTeamId()].color;

            playerName.color = color;
            healthBarFill.color = color;
        }

        public void ActivateNameplate()
        {
            playerHealth.gameObject.SetActive(true);
            playerName.gameObject.SetActive(true);
        }

        public void DeactivateNameplate()
        {
            playerName.gameObject.SetActive(false);
            playerHealth.gameObject.SetActive(false);
        }

        #endregion
    }
}