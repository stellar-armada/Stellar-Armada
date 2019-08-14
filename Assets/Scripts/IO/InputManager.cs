using UnityEngine;
using UnityEngine.Serialization;

#pragma warning disable 0649
namespace SpaceCommander.IO
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager instance;
        
        private Vector2 leftThumbstickValue;

        private Vector2 rightThumbstickValue;

        public Transform leftHand;
        public Transform rightHand;

        public delegate void InputButtonHandler(bool buttonState);

        public InputButtonHandler OnLeftSecondaryTrigger;
        public InputButtonHandler OnRightSecondaryTrigger;
        
        public InputButtonHandler OnLeftTrigger;
        public InputButtonHandler OnRightTrigger;

        void Awake()
        {
            instance = this;
        }
        public void LeftTrigger(bool on)
        {
            Debug.Log("LeftTrigger pulled: " + on);
            OnLeftTrigger?.Invoke(on);
        }

        public void RightTrigger(bool on)
        {
            Debug.Log("RightTrigger pulled: " + on);
OnRightTrigger?.Invoke(on);
        }

        public void LeftSecondary(bool on)
        {
            Debug.Log("LeftSecondary pulled: " + on);
            OnLeftSecondaryTrigger?.Invoke(on);
        }

        public void RightSecondary(bool on)
        {
            Debug.Log("RightSecondary pulled: " + on);
            OnRightSecondaryTrigger?.Invoke(on);
        }

        public void LeftThumbstick(Vector2 value)
        {
            Debug.Log("LeftThumbstick pulled: " + value);

        }

        public void RightThumbstick(Vector2 value)
        {
            Debug.Log("RightThumbstick pulled: " + value);

        }

        public void LeftThumbstickButton(bool on)
        {
            Debug.Log("LeftThumbstickButton pulled: " + on);

        }

        public void RightThumbstickButton(float on)
        {
            Debug.Log("RightThumbstickButton pulled: " + on);

        }

        public void ButtonOne(bool on)
        {
            Debug.Log("ButtonOne pulled: " + on);

        }

        public void ButtonTwo(bool on)
        {
            Debug.Log("ButtonTwo pulled: " + on);
        }
    }
}