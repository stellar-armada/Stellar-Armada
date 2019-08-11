using UnityEngine;
using TMPro;
using SpaceCommander.Teams;

namespace SpaceCommander.Player
{
    // Class to control the nameplate (health and name) over player's head
    public class Nameplate : MonoBehaviour
    {
        [SerializeField] TextMeshPro playerName;
        [SerializeField] PlayerController playerController;
        [SerializeField] Transform transformToFollow;

        public IPlayer localPlayer;
        
        private Transform _t;

        private bool isLocalPlayer;
        private Color color;
        
        #region Initialization and Deinitialization

        void Awake()
        {
            playerController.EventOnPlayerNameChange += HandlePlayerNameChange;
            playerController.EventOnPlayerTeamChange += HandleTeamChange;
            _t = transform;
        }
        void Start()
        {

            HandlePlayerNameChange();
            HandleTeamChange();
            isLocalPlayer = playerController.isLocalPlayer;
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
            color = TeamManager.instance.templates[playerController.GetTeamId()].color;

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