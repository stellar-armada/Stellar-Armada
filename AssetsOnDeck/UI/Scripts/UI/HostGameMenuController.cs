using UnityEngine;
using SpaceCommander.Game.GameModes;
using Mirror;
using SpaceCommander.Game;
using SpaceCommander.Player;

namespace SpaceCommander.UI
{
    /* Controller for the "Host Game" / "Start New Match" UI
     * Gets called from main menu and in game menu, so it lives inside the Shared Menu UI subpanel of the local player canvas
    */

    public class HostGameMenuController : MonoBehaviour
    {
        #region Private Fields Serialized In Inspector

        [SerializeField] TMPro.TextMeshProUGUI matchLengthText;
        [SerializeField] TMPro.TextMeshProUGUI matchLengthMenuText;
        [SerializeField] TMPro.TextMeshProUGUI mapText;
        [SerializeField] TMPro.TextMeshProUGUI gameModeText;
        [SerializeField] TMPro.TextMeshProUGUI killsToWinText;
        [SerializeField] TMPro.TextMeshProUGUI killsToWinMenuText;
        [SerializeField] GameObject hostMainPanel;
        [SerializeField] GameObject hostGameModePanel;
        [SerializeField] GameObject hostMapPanel;
        [SerializeField] GameObject hostMatchLengthPanel;
        [SerializeField] GameObject hostKillsToWinPanel;

        #endregion

        #region Initialization

        private void Awake()
        {
            PopulateGameModeText();
            PopulateMapText();
            PopulateMatchLengthText();
        }

        #endregion

        #region Public Methods

        public void StartHost()
        {
            if (PlayerManager.GetLocalNetworkPlayer() == null)
            {
                NetworkManager.singleton.StartHost();
            }
            else
            {
                Match.GetCurrentMatch().ResetMatch();
            }
        }

        public void ShowGameModeOptions()
        {
            hostMainPanel.SetActive(false);
            hostGameModePanel.SetActive(true);
        }

        public void ShowMapOptions()
        {
            hostMainPanel.SetActive(false);
            hostMapPanel.SetActive(true);
        }

        public void ShowMatchLengthOptions()
        {
            hostMainPanel.SetActive(false);
            hostMatchLengthPanel.SetActive(true);
        }

        public void ShowKillsToWinOptions()
        {
            hostMainPanel.SetActive(false);
            hostKillsToWinPanel.SetActive(true);
        }
        
        public void SetMap(string map)
        {
            SettingsManager.SetHostLevel(map.ToUpper());
            PopulateMapText();
            hostMapPanel.SetActive(false);
            hostMainPanel.SetActive(true);
        }

        public void SetMatchLength()
        {
            float matchLength = float.Parse(matchLengthMenuText.text);
            SettingsManager.SetMatchLength(matchLength * 60); // Multiply by 60 for seconds
            PopulateMatchLengthText();
            hostMatchLengthPanel.SetActive(false);
            hostMainPanel.SetActive(true);
        }

        [EnumAction(typeof(GameModeType))] // black magic so we can pass enums as button method calls in the inspector
        public void SetGameMode(int gameMode) // cast as int for C#
        {
            SetGameModeAndCloseWindow((GameModeType)gameMode);
        }


        #endregion

        #region Private Methods
        void SetGameModeAndCloseWindow(GameModeType gameMode)
        {
            SettingsManager.SaveHostGameMode(gameMode);
            PopulateGameModeText();
            hostMainPanel.SetActive(true);
            hostGameModePanel.SetActive(false);
        }
        
        void PopulateGameModeText()
        {
            gameModeText.text = SettingsManager.GetStoredHostGameMode().ToString().ToUpper();
        }

        void PopulateMapText()
        {
            Debug.Log(SettingsManager.GetStoredHostLevel());
            mapText.text = SettingsManager.GetStoredHostLevel().ToUpper();
        }

        void PopulateMatchLengthText()
        {

            matchLengthText.text = Mathf.FloorToInt(SettingsManager.GetMatchLength() / 60f).ToString() + " MINUTES";
            matchLengthMenuText.text = Mathf.FloorToInt(SettingsManager.GetMatchLength()).ToString();

        }
        #endregion

    }
}