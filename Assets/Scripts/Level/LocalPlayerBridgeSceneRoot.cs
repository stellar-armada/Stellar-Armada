using UnityEngine;

namespace StellarArmada.Levels
{
    public class LocalPlayerBridgeSceneRoot : MonoBehaviour
    {
        // Singleton reference to the local player's "scene"
        public static LocalPlayerBridgeSceneRoot instance;

        public delegate void SceneRootCreatedEvent();

        public static SceneRootCreatedEvent SceneRootCreated;

        void Awake()
        {
            instance = this;
            SceneRootCreated?.Invoke();
        }
    }
}