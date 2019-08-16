using UnityEngine;

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

        public InputButtonHandler OnLeftGrip;
        public InputButtonHandler OnRightGrip;
        
        public InputButtonHandler OnLeftTrigger;
        public InputButtonHandler OnRightTrigger;

        public InputButtonHandler OnLeftThumbstickButton;
        public InputButtonHandler OnRightThumbstickButton;

        public InputButtonHandler OnButtonOne;
        public InputButtonHandler OnButtonTwo;
        public InputButtonHandler OnButtonThree;
        public InputButtonHandler OnButtonFour;

        public InputButtonHandler OnButtonStart;
        
        void Awake()
        {
            instance = this;
        }
        public void LeftTrigger(bool on)
        {
            OnLeftTrigger?.Invoke(on);
        }

        public void RightTrigger(bool on)
        {
OnRightTrigger?.Invoke(on);
        }

        public void LeftSecondary(bool on)
        {
            OnLeftGrip?.Invoke(on);
        }

        public void RightSecondary(bool on)
        {
            OnRightGrip?.Invoke(on);
        }

        public void LeftThumbstick(Vector2 value)
        {

        }

        public void RightThumbstick(Vector2 value)
        {

        }

        public void LeftThumbstickButton(bool on)
        {
            OnLeftThumbstickButton?.Invoke(on);
        }

        public void RightThumbstickButton(bool on)
        {
            OnRightThumbstickButton?.Invoke(on);
        }

        public void ButtonOne(bool on)
        {
            OnButtonOne?.Invoke(on);
        }

        public void ButtonTwo(bool on)
        {
            OnButtonTwo?.Invoke(on);
        }
        
        public void ButtonThree(bool on)
        {
            OnButtonThree?.Invoke(on);
        }

        public void ButtonFour(bool on)
        {
            OnButtonFour?.Invoke(on);
        }
        
        public void ButtonStart(bool on)
        {
            OnButtonStart?.Invoke(on);
        }
        
    }
}