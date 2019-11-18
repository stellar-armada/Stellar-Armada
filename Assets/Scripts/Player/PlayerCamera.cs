using UnityEngine;
#pragma warning disable 0649
namespace StellarArmada.Player
{
    public class PlayerCamera : MonoBehaviour
    {
        public static PlayerCamera instance;
        [SerializeField] private Camera cam;

        [SerializeField] private LayerMask matchLayers;
        [SerializeField] LayerMask purgatory;

        public void ShowMatchView()
        {
            if(cam != null)
            cam.cullingMask = matchLayers;
        }

        public void ShowPurgatoryView()
        {
            cam.cullingMask = purgatory;
        }
        
        public Camera GetCamera() => cam;

        void Awake() => instance = this;

        void Start()
        {
            ShowPurgatoryView(); // Initialize here
        }
    }
}