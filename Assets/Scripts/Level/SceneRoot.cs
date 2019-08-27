using UnityEngine;

namespace StellarArmada.Levels
{
    public class SceneRoot : MonoBehaviour
    {
        // Singleton reference to the local player's "scene"
        // This is what the player is locked to
        // This is instantiated for the local player as part of their bridge when they captain a ship

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