using UnityEngine;
#pragma warning disable 0649
namespace StellarArmada
{
    public class PlayerCamera : MonoBehaviour
    {
        public static PlayerCamera instance;
        [SerializeField] private Camera _camera;

        public Camera GetCamera() => _camera;

        void Awake() => instance = this;
    }
}