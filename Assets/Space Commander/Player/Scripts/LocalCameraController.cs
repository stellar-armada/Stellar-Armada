using UnityEngine;

namespace SpaceCommander.Player
{
    /* Manages the layers the player sees
     * Mainly so that local player can't see the level and players until they're calibrated :)
     */
    public class LocalCameraController : MonoBehaviour
    {

        public static LocalCameraController instance;

        [SerializeField]  LayerMask cameraLayersNotCalibrated;
        [SerializeField] LayerMask cameraLayersCalibrated;

        Camera cam;

        public Transform uiRoot;

        void Awake()
        {
            instance = this;
            cam = GetComponent<Camera>();
            HideLevelAndPlayers();
        }

        public Camera GetCamera()
        {
            return cam;
        }

        public void ShowLevelAndPlayers()
        {
            cam.cullingMask = cameraLayersCalibrated;
        }

        public void HideLevelAndPlayers()
        {
            cam.cullingMask = cameraLayersNotCalibrated;

        }
    }
}