using UnityEngine;
using UnityEngine.UI;
using StellarArmada.Player;
using TMPro;

#pragma warning disable 0649
namespace StellarArmada.UI
{
    // Settings menu manager scipt
    // TO-DO: Reimpliment with UI!
    public class SettingsMenuManager : MonoBehaviour
    {

        public static SettingsMenuManager instance; // Singleton accessor

        #region Private Fields Visible in Inspector
        [SerializeField] TextMeshProUGUI playerNamePreviewText;
        [SerializeField] TextMeshProUGUI playerNameKeyboardInputText;
        [SerializeField] Slider SoundFxSlider;
        [SerializeField] Slider MusicSlider;
        [SerializeField] GameObject mainSettingsPanel;
        [SerializeField] GameObject nameEntryPanel;
        #endregion

        GameObject[] settingsMenuMainObjects;

        #region Initialization / Deinitialization

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.LogError("Instance was already set.");
                Destroy(instance);
            }
            instance = this;
        }

        void OnDestroy()
        {
            instance = null;
        }

        void OnEnable()
        {
            PopulateUserName();
            PopulateMusicVolume();
            PopulateSoundFXVolume();
        }

        #endregion

        #region Public Methods

        public void ShowPlayernNamePanel()
        {
            nameEntryPanel.SetActive(true);
            mainSettingsPanel.SetActive(false);
        }

        public void ChangeNameAndReturnToMainSettings()
        {
            if (playerNameKeyboardInputText.text == "") playerNameKeyboardInputText.text = "PLAYER";

            PlayerSettingsManager.SavePlayerName(playerNameKeyboardInputText.text);
            
            if (PlayerController.localPlayer != null)
            {
                PlayerController.localPlayer.CmdSetUserName(playerNameKeyboardInputText.text);
            }
            
            PopulateUserName();
            nameEntryPanel.SetActive(false);
            mainSettingsPanel.SetActive(true);
        }

        #endregion

        #region Private Methods

        void PopulateMusicVolume()
        {
            MusicSlider.value = PlayerSettingsManager.GetMusicVolume();
        }

        void PopulateSoundFXVolume()
        {
            SoundFxSlider.value = PlayerSettingsManager.GetSoundFxVolume();
        }

      
        void PopulateUserName()
        {
            playerNamePreviewText.text = PlayerSettingsManager.GetSavedPlayerName();
            playerNameKeyboardInputText.text = PlayerSettingsManager.GetSavedPlayerName();
        }

        #endregion
    }
}