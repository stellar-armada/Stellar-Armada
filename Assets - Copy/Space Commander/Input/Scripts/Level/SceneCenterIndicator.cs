using UnityEngine;

namespace SpaceCommander.Level
{

    // Makes a neat little animated object appear and animate at the center of the map once, directly over the calibration target

    public class SceneCenterIndicator : MonoBehaviour
    {

        public static SceneCenterIndicator instance;

        void Awake()
        {
            instance = this;
        }

        private void OnDestroy()
        {
            instance = null;
        }

        public void ShowIndicator()
        {
            BroadcastMessage("Activate");
        }

        public void HideIndicator()
        {
            BroadcastMessage("Deactivate");

        }

    }
}