using SpaceCommander.Player;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceCommander.UI.Mobile
{
    public class UIController : MonoBehaviour
    {
        public static UIController instance;

        public static bool uiIsEnabled = false;

        [SerializeField] private CanvasGroup panelCanvasGroup;
        [SerializeField] private Button scopeInButton;
        [SerializeField] private Button menuButton;
        [SerializeField] private Button[] hideableButtons;

        // Start is called before the first frame update
        void Awake()
        {
            instance = this;

            HideInGameUI();
            HideMenuButton();
        }

        public void ShowMenuButton()
        {
            menuButton.gameObject.SetActive(true);
        }

        public void HideMenuButton()
        {
            menuButton.gameObject.SetActive(false);

        }

        public static void ShowScopeInButton(bool show)
        {
            instance.scopeInButton.gameObject.SetActive(show);
        }

        public void OnEnable()
        {
            PlayerCanvasController.instance.EventOnInGameMenuCanvasOpened += HideInGameUI;
            PlayerCanvasController.instance.EventOnInGameMenuCanvasClosed += ShowInGameUI;
        }

        public static void ShowInGameUI()
        {

            instance.panelCanvasGroup.alpha = 1.0f;
            instance.panelCanvasGroup.interactable = true;
            uiIsEnabled = true;
            foreach (Button button in instance.hideableButtons)
            {
                button.gameObject.SetActive(true);
            }
        }

        public static void HideInGameUI()
        {
            instance.panelCanvasGroup.alpha = 0.0f;
            instance.panelCanvasGroup.interactable = false;
            uiIsEnabled = false;
            foreach (Button button in instance.hideableButtons)
            {
                button.gameObject.SetActive(false);
            }
        }
    }
}