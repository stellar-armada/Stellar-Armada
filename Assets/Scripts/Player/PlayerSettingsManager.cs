using System.IO;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Player
{

    /* Serialize and store all player data here
     * Settings are stored as JSON, converted from their original type
     */

    public class PlayerSettings
    {
        public bool desktopDisplayMode;
        public float musicVolume;
        public float sfxVolume;
        public string playerName;
    }

    public class PlayerSettingsManager : MonoBehaviour
    {
        public static PlayerSettingsManager instance;
        
        private static PlayerSettings settings;
        private static string settingsPath;
        #region Initialization

        void Awake()
        {
            instance = this;
            settingsPath = Application.streamingAssetsPath + "/settings.txt";
            Init();
        }
        
        void InitSettings()
        {
            Debug.Log("initing settings");
            settings = new PlayerSettings
            {
                desktopDisplayMode = false,
                musicVolume = .8f,
                sfxVolume = .8f,
                playerName = "PLAYERONE"
            };

            SaveSettings();
        }

        void PopulateSettings()
        {
            settings = JsonUtility.FromJson<PlayerSettings>(File.ReadAllText(settingsPath));
        }

        static void SaveSettings()
        {
           File.WriteAllText(settingsPath,  JsonUtility.ToJson(settings));
        }

        public void Init()
        {
            if (!File.Exists(Application.streamingAssetsPath + "/settings.txt"))
            {
                InitSettings();
            } else
            {
                PopulateSettings();
            }
        }

        #endregion

        #region Deinitialization

        private void OnDestroy()
        {
            settings = null;
        }

        #endregion

        #region Public Static Methods

        public static bool GetDisplayMode()
        {
            return settings.desktopDisplayMode;
        }

        public static float GetMusicVolume()
        {
            return settings.musicVolume;
        }

        public static float GetSoundFxVolume()
        {
            return settings.sfxVolume;
        }

        public static void SetSoundFxVolume(float volume)
        {
            settings.sfxVolume = volume;
            SaveSettings();
        }

        public static void SetMusicVolume(float volume)
        {
            settings.musicVolume = volume;
            SaveSettings();  
        }

        public static string GetSavedPlayerName()
        {
            return settings.playerName;
        }

        public static void SavePlayerName(string newPlayerName)
        {
            settings.playerName = newPlayerName;
            SaveSettings();
        }

        #endregion
    }
}