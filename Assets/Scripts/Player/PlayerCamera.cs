using UnityEngine;
#pragma warning disable 0649
namespace StellarArmada.Player
{
    public class PlayerCamera : MonoBehaviour
    {
        public static PlayerCamera instance;
        [SerializeField] private Camera _camera;

        public Camera GetCamera() => _camera;

        void Awake() => instance = this;
    }
}