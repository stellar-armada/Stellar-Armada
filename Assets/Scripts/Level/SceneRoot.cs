using UnityEngine;

namespace StellarArmada.Levels
{
    public class SceneRoot : MonoBehaviour
    {
        // Singleton reference to the local player's "scene"
        public static SceneRoot instance;

        public delegate void SceneRootCreatedEvent();

        public static SceneRootCreatedEvent SceneRootCreated;

        void Awake()
        {
            instance = this;
            SceneRootCreated?.Invoke();
        }
    }
}