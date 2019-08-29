using UnityEngine;
using TMPro;
using StellarArmada.Teams;

#pragma warning disable 0649
namespace StellarArmada.Player
{
    // Class to control the nameplate (health and name) over player's head
    public class Nameplate : MonoBehaviour
    {
        [SerializeField] TextMeshPro playerName;
        [SerializeField] HumanPlayerController humanPlayerController;
        [SerializeField] Transform transformToFollow;
        [SerializeField] private Renderer nameRenderer;
        
        // local reference vars
        private Transform t;
        private bool isLocalPlayer;
        private Color color;

        void Awake()
        {
            humanPlayerController.EventOnPlayerNameChange += HandlePlayerNameChange;
            humanPlayerController.EventOnPlayerTeamChange += HandleTeamChange;
            t = transform;
        }
        void Start()
        {
            HandlePlayerNameChange();
            isLocalPlayer = humanPlayerController.isLocalPlayer;
            if (humanPlayerController == HumanPlayerController.localPlayer) nameRenderer.enabled = false; // Hide from local player
        }

        void LateUpdate()
        {
            t.position = transformToFollow.position; // Follow player
            if (!isLocalPlayer) FaceLocalPlayer();
        }

        void FaceLocalPlayer()
        {
            if (!HumanPlayerController.localPlayer) return;
            t.LookAt(HumanPlayerController.localPlayer.GetGameObject().transform);
            t.rotation = Quaternion.Euler(0, t.rotation.eulerAngles.y + 180f, 0);
        }
        
        public void HandlePlayerNameChange()
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
    }
}