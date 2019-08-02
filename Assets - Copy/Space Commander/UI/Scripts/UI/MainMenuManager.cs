using UnityEngine;

namespace SpaceCommander.UI
{
    /* "Main Menu" Manager -- i.e. button handler for the "Play  / Settings / Tutorial" Screen
     */ 
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] MenuScreen mainMenu;
        [SerializeField] MenuScreen playMenu;
        [SerializeField] MenuScreen settingsMenu;
        [SerializeField] MenuScreen hostMenu;
        [SerializeField] GameObject warningPanel;
        [SerializeField] GameObject setupPanel;
        
        public static bool hasShownWarning = false;

        public void SetWarningToShown()
        {
            hasShownWarning = true;
        }

        void Awake()
        {
            if (hasShownWarning) // Let's not show the dumb warnings to a player twice in a session
            {
                warningPanel.SetActive(false);
                setupPanel.SetActive(false);
                mainMenu.Activate();
            }
        }

        public void ShowPlayMenu()
        {
            mainMenu.Deactivate();
            playMenu.Activate();
        }

        public void ShowSettingsMenu()
        {
            mainMenu.Deactivate();
            settingsMenu.Activate();
        }

        public void GoBack()
        {
            mainMenu.Activate();
            playMenu.Deactivate();
        }

        public void PlayMenuHost()
        {
            hostMenu.Activate();
            playMenu.Deactivate();

        }

        public void PlayMenuBack()
        {
            mainMenu.Activate();
            playMenu.Deactivate();

        }


    }
}