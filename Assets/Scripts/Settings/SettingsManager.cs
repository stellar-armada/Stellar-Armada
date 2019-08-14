using System.IO;
using UnityEngine;

#pragma warning disable 0649
namespace SpaceCommander.Game
{

    /* Serialize and store all player data here
     * This includes both player-specific data like name and weapon hand
     * As well as host options, so that when player goes to host a game they are offered the last options they used
     * Settings are stored as JSON, converted from their original type
     */

    public class PlayerSettings
    {
        public float musicVolume;
        public float sfxVolume;
        public string playerName;
    }

    public class SettingsManager : MonoBehaviour
    {
        private static PlayerSettings settings;
        private static string settingsPath;
        #region Initialization

        void Awake()
        {
            Init();
        }
        
        void InitSettings()
        {
            settings = new PlayerSettings
            {
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
            settingsPath = Application.streamingAssetsPath + "/settings.txt";
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