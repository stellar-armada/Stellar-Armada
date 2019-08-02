using UnityEngine;
using InputHandling;
using SpaceCommander.Game;
using SpaceCommander.Player;

namespace SpaceCommander.UI
{
    // Manager for in-game menu buttons, especially those below the scoreboard

    public class InGameMenuManager : MonoBehaviour
    {
        public static InGameMenuManager instance;

        [SerializeField] GameObject youAreHostPanel;
        [SerializeField] private GameObject startMatchButton;
        [SerializeField] private GameObject newMatchButton;
        [SerializeField] GameObject calibrateFirst;
        [SerializeField] GameObject menuButtons;
        [SerializeField] GameObject hostOptionsButton;
        [SerializeField] GameObject classSelectionMenu;
        [SerializeField] GameObject scoreboardPanel;
        [SerializeField] private HostOptionsMenuManager hostOptions;
        [SerializeField] private ClientOptionsMenuManager clientOptions;


        private void Awake()
        {
            instance = this;
        }

        private void OnDestroy()
        {
            instance = null;
        }

        public void ResetInGameMenuCanvas()
        {
            scoreboardPanel.SetActive(true);
            SharedMenuManager.instance.HideSharedMenus();
            hostOptions.HideMainHostPanel();
            clientOptions.HideClientOptionsMenu();

        }

        private void OnEnable()
        {

            if (PlayerManager.GetLocalNetworkPlayer() != null)
            {
                hostOptionsButton.SetActive(true);

                if (Match.GetCurrentMatch() == null) return;

                if (Match.InLobby())
                {
                    youAreHostPanel.SetActive(true);

                    startMatchButton.SetActive(true);

                    newMatchButton.SetActive(false);

                    calibrateFirst.SetActive(false);
                }
                else if (Match.IsFinished())
                {
                    youAreHostPanel.SetActive(true);

                    startMatchButton.SetActive(false);

                    newMatchButton.SetActive(true);

                    calibrateFirst.SetActive(false);
                }

            }
            else
            {
                hostOptionsButton.SetActive(false);
            }
        }

        public void StartMatch()
        {
            youAreHostPanel.SetActive(false);
            Match.GetCurrentMatch().StartMatch();
            PlayerCanvasController.instance.HideCanvas();
        }

        public void ShowClassSelectionMenu()
        {
            scoreboardPanel.SetActive(false);
            classSelectionMenu.SetActive(true);
        }

        public bool IsClassSelectionMenuShowing()
        {
            if (classSelectionMenu.activeSelf) { return true; }
            else
            {
                return false;
            }
        }

        public void HideClassSelectionMenu()
        {
            scoreboardPanel.SetActive(true);
            classSelectionMenu.SetActive(false);
        }

    }
}