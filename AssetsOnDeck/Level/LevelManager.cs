using UnityEngine;
using SpaceCommander.Game;
using SpaceCommander.Player;

namespace SpaceCommander.Level
{
    // IN DEVELOPMENT -- replace level prefabs with a fully modular level editor system so people can make their own for their space :)


    /* Levels in Laserbots tend to be very small and light, and so are loaded as prefabs instead of scenes
     * The only time the scene reloads is when the player disconnects from a game -- this reloads everything (except for static vars, so watch out)
     * Lots of variables that could be accessed by GetComponent are serialized to avoid any additional CPU load time
     * Just make sure all your values are set properly on your LevelPrefab objects!
     * Add levels by creating new prefabs and adding them to the LevelManager GameObject
     */

    [System.Serializable]
    public class LevelDictionary : SerializableDictionary<string, GameObject>
    {
    }

    public class LevelManager : MonoBehaviour
    {
        private bool calibrationReady;
        private bool matchReady;

        // Public static fields
        public static LevelManager instance; // singleton accessor
        public static LevelObject currentLevel;

        // Public instance fields
        public Transform sceneRoot;

        // Serialized private fields
        [SerializeField] LevelDictionary availableLevels;

        #region Initialization and Dinitialization

        public void Init()
        {
            instance = this;
            currentLevel = null;

                InitLevel();
                Match.OnMatchInitialized += () =>
            {
                matchReady = true;
                InitLevel();
            };
        }

        void InitLevel()
        {
            if (!matchReady || !calibrationReady)
                return;
            CreateLevel(Match.GetCurrentMatch().levelName);
        }

        private void OnDestroy()
        {
            instance = null;
        }

        #endregion

        #region Public Static Methods

        public static GameObject GetPrefabFromLevelName(string name)
        {
            GameObject prefab;
            instance.availableLevels.TryGetValue(name, out prefab);
            if (prefab != null) return prefab;
            return null;
        }

        public static void ActivateLevel()
        {
            if (currentLevel.IsLevelActive() == false)
            {
                currentLevel.gameObject.SetActive(true);
                currentLevel.Materialize();
            }
        }

        public static void CreateLevel(string mapName)
        {
            DestroyLevel(); // If a level already exists, destroy it

            GameObject prefab; // dictionary output ref
            // Get prefab from dictionary
            prefab = GetPrefabFromLevelName(SettingsManager.GetStoredHostLevel());

                bool success =instance.availableLevels.TryGetValue(mapName, out prefab);
                if (!success) return;

                GameObject levelGO = Instantiate(prefab, instance.sceneRoot);
                levelGO.transform.localPosition = Vector3.zero;
                levelGO.transform.localRotation = Quaternion.identity;
                currentLevel = levelGO.GetComponent<LevelObject>();


                currentLevel.gameObject.SetActive(true);

            ActivateLevel();
        }

        #endregion

        #region Static Private Methods

        static void DestroyLevel()
        {
            if (currentLevel != null)
            {
                DestroyImmediate(currentLevel.gameObject);
            }
        }

        #endregion
    }
}