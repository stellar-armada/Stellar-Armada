using SpaceCommander.Game;
using SpaceCommander.Player;
using UnityEngine;

namespace SpaceCommander.UI
{

    /* Shared menu manager -- manages the menus that can be accessed from in game or from main screen
     * Current shared menus are settings and "host match" / "change match options"
     */

    public class SharedMenuManager : MonoBehaviour
    {

        public static SharedMenuManager instance;
        [SerializeField] MenuScreen hostGameMenu;
        [SerializeField] MenuScreen inGameLinkToHost;
        [SerializeField] MenuScreen mainMenuLinkToHost;
        [SerializeField] MenuScreen settingsMenu;
        [SerializeField] MenuScreen inGameLinkToSettings;
        [SerializeField] MenuScreen mainMenuLinkToSettings;

        void Awake()
        {
            instance = this;
        }

        public void HideSharedMenus()
        {
            hostGameMenu.Deactivate();
            settingsMenu.Deactivate();

        }
        
        public void HideHostGameMenuAndReturn()
        {
            hostGameMenu.Deactivate();

            if (PlayerManager.GetLocalNetworkPlayer() != null) // This means we're not in game!
            {
                inGameLinkToHost.Activate();
            }
            else
            {
                mainMenuLinkToHost.Activate();
            }
        }

        public void HideSettingsAndReturn()
        {
            settingsMenu.Deactivate();

            if (PlayerManager.GetLocalNetworkPlayer() != null) // This means we're not in game!
            {
                inGameLinkToSettings.Activate();
            }
            else
            {
                mainMenuLinkToSettings.Activate();
            }
        }
    }

}