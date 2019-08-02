using SpaceCommander.UI;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceCommander.Networking
{
    public class NetworkDiscoveryUIController : MonoBehaviour
    {

        public static NetworkDiscoveryUIController instance;

        [Header("UI")]
        public GameObject mainPanel;
        public Transform content;
        public ServerStatusSlot slotPrefab;
        public Button serverAndPlayButton;
        public Button joinButton;
        public GameObject connectingPanel;
        public TMPro.TextMeshProUGUI connectingText;
        public Button connectingCancelButton;

        [SerializeField] MainMenuManager mainMenuManager;

        [HideInInspector] public Button currentButton;

        private void Awake()
        {
            instance = this;
        }
        private void OnDestroy()
        {
            if (instance == this) instance = null;
        }
    }
}
