using StellarArmada.IO;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.UI
{
    public class PlayerCanvasManager : MonoBehaviour
    {
        void Start()
        {
            InputManager.instance.OnLeftThumbstickButton += (on) =>
            {
                if (on) AttachToLeftPoint();
                SetCanvasState(on);
            };
            InputManager.instance.OnRightThumbstickButton += (on) =>
            {
                if (on) AttachToRightPoint();
                SetCanvasState(on);
            };
            SetCanvasState(false);
        }

        [SerializeField] private Transform leftCanvasAttachPoint;
        [SerializeField] private Transform rightCanvasAttachPoint;

        public void AttachToLeftPoint()
        {
            transform.SetParent(leftCanvasAttachPoint);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        public void AttachToRightPoint()
        {
            transform.SetParent(rightCanvasAttachPoint);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        public void SetCanvasState(bool state)
        {
            gameObject.SetActive(state);
        }
    }
}