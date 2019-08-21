using SpaceCommander.Players;
using UnityEngine;
using TMPro;
using SpaceCommander.Teams;

#pragma warning disable 0649
namespace SpaceCommander.Player
{
    // Class to control the nameplate (health and name) over player's head
    public class Nameplate : MonoBehaviour
    {
        [SerializeField] TextMeshPro playerName;
        [SerializeField] HumanPlayerController humanPlayerController;
        [SerializeField] Transform transformToFollow;
        [SerializeField] private Renderer nameRenderer;
        
        private Transform _t;

        private bool isLocalPlayer;
        private Color color;
        
        #region Initialization and Deinitialization

        void Awake()
        {
            humanPlayerController.EventOnPlayerNameChange += HandleBasePlayerNameChange;
            humanPlayerController.EventOnPlayerTeamChange += HandleTeamChange;
            _t = transform;
        }
        void Start()
        {

            HandleBasePlayerNameChange();
            HandleTeamChange();
            isLocalPlayer = humanPlayerController.isLocalPlayer;
            if (humanPlayerController == HumanPlayerController.localPlayer) nameRenderer.enabled = false;
        }
        #endregion

        #region Private Methods

        void LateUpdate()
        {
            _t.position = transformToFollow.position; // Follow player
            if (!isLocalPlayer) FaceLocalPlayer();
        }

        void FaceLocalPlayer()
        {
            _t.LookAt(HumanPlayerController.localPlayer.GetGameObject().transform);
            _t.rotation = Quaternion.Euler(0, _t.rotation.eulerAngles.y + 180f, 0);
        }

        #endregion

        #region Public Methods

        public void HandleBasePlayerNameChange()
        {
            playerName.text = humanPlayerController.GetName();
        }

        public void HandleTeamChange()
        {
            color = TeamManager.instance.templates[humanPlayerController.GetTeamId()].color;

            playerName.color = color;
        }

        public void ActivateNameplate()
        {
            playerName.gameObject.SetActive(true);
        }

        public void DeactivateNameplate()
        {
            playerName.gameObject.SetActive(false);
        }

        #endregion
    }
}