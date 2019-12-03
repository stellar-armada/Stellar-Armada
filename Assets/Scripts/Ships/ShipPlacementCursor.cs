using StellarArmada.Player;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Ships
{
    public abstract class ShipPlacementCursor : MonoBehaviour
    {
        public static ShipPlacementCursor instance;

        protected Transform t;
        
        protected bool isInitialized = false;

        [SerializeField] private Renderer ren;
        void Awake()
        {
            instance = this;
            t = GetComponent<Transform>();
            PlayerController.localPlayer.OnLocalPlayerInitialized += Initialize;
            Hide(); 
        }

        public void Show()
        {
            ren.enabled = true;
        }

        public void Hide()
        {
            ren.enabled = false;
        }

        protected abstract void Initialize();
        
    }
}