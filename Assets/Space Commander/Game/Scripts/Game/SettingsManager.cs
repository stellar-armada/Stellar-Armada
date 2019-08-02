using System.Collections.Generic;
using System.IO;
using SpaceCommander.Game.GameModes;
using UnityEngine;

namespace SpaceCommander.Game
{

    /* Serialize and store all player data here
     * This includes both player-specific data like name and weapon hand
     * As well as host options, so that when player goes to host a game they are offered the last options they used
     * Settings are stored as JSON, converted from their original type
     */

    public class PlayerSettings
    {
        public GameModeType gameMode;
        public float matchLength;
        public string level;
        public float musicVolume;
        public float sfxVolume;
        public string playerName;
    }

    public class SettingsManager : MonoBehaviour
    {
        private static PlayerSettings settings;
        private static string settingsPath;
        #region Initialization
        
        void InitSettings()
        {
            settings = new PlayerSettings
            {
                gameMode = GameModeType.FREE_PLAY,
                matchLength = 10 * 60,
                level = "NULL",
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
            settingsPath = Application.persistentDataPath + "/settings.txt";
            if (!File.Exists(Application.persistentDataPath + "/settings.txt"))
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
        public static GameModeType GetStoredHostGameMode()
        {
            return settings.gameMode;
        }

        public static void SaveHostGameMode(GameModeType gameMode)
        {
            settings.gameMode = gameMode;
            SaveSettings();
        }
/*
        public static void SaveAnchorPairs(AnchorPair[] _pairs)
        {
            settings.anchorPairs = _pairs;
            SaveSettings();
        }

        public static AnchorPair[] GetAnchorPairs()
        {
            return settings.anchorPairs;
        }
        */

        public static float GetMatchLength()
        {
            return settings.matchLength;
        }

        public static void SetMatchLength(float length)
        {
            settings.matchLength = length;
            SaveSettings();
        }

        public static string GetStoredHostLevel()
        {
            return settings.level;
        }

        public static void SetHostLevel(string levelName)
        {
            settings.level = levelName;
            SaveSettings();
            
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