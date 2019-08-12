using UnityEngine;
using UnityEngine.UI;
using SpaceCommander.Game;
using SpaceCommander.Player;
using TMPro;

namespace SpaceCommander.UI
{
    // Settings menu manager scipt

    public class SettingsMenuManager : MonoBehaviour
    {

        public static SettingsMenuManager instance; // Singleton accessor

        #region Private Fields Visible in Inspector
        [SerializeField] TextMeshProUGUI playerNamePreviewText;
        [SerializeField] TextMeshProUGUI playerNameKeyboardInputText;
        [SerializeField] Slider SoundFxSlider;
        [SerializeField] Slider MusicSlider;
        [SerializeField] TextMeshProUGUI weaponHandPreviewText;
        [SerializeField] GameObject mainSettingsPanel;
        [SerializeField] GameObject weaponHandEntryPanel;
        [SerializeField] GameObject nameEntryPanel;
        #endregion

        GameObject[] settingsMenuMainObjects;

        #region Initialization / Deinitialization

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.Log("Instance was already set. Replacing.");
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

            SettingsManager.SavePlayerName(playerNameKeyboardInputText.text);
            
            if (PlayerController.localPlayer != null)
            {
                PlayerController.localPlayer.SetUserName(playerNameKeyboardInputText.text);
            }
            
            PopulateUserName();
            nameEntryPanel.SetActive(false);
            mainSettingsPanel.SetActive(true);
        }

        #endregion

        #region Private Methods

        void PopulateMusicVolume()
        {
            MusicSlider.value = SettingsManager.GetMusicVolume();
        }

        void PopulateSoundFXVolume()
        {
            SoundFxSlider.value = SettingsManager.GetSoundFxVolume();
        }

      
        void PopulateUserName()
        {
            playerNamePreviewText.text = SettingsManager.GetSavedPlayerName();
            playerNameKeyboardInputText.text = SettingsManager.GetSavedPlayerName();
        }

        #endregion
    }
}