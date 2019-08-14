using UnityEngine;

#pragma warning disable 0649
namespace SpaceCommander.Scenarios
{
    // Instantiates the scene root indicator at start so we don't have to keep it in our scene
    public class MapParent : MonoBehaviour
    {
        public static MapParent instance;
        private void Awake()
        {
            if (instance == null)
                instance = this;
            
        }

    }
}
