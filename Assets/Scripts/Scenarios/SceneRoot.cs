using UnityEngine;

namespace SpaceCommander.Scenarios
{
    // Instantiates the scene root indicator at start so we don't have to keep it in our scene
    public class SceneRoot : MonoBehaviour
    {
        public static SceneRoot instance;
        private void Awake()
        {
            if (instance == null)
                instance = this;
            
        }

    }
}
