using UnityEngine;
using Wacki;

#pragma warning disable 0649
namespace SpaceCommander.IO {

    public class LaserPointer : IUILaserPointer
    {
        [SerializeField] private InputManager inputManager;
        public bool buttonState = false;
        bool _prevButtonState = false;
        bool _buttonChanged = false;

        protected override void Update()
        {
            base.Update();

            if(buttonState == _prevButtonState) {
                _buttonChanged = false;
            } else {
                _buttonChanged = true;
                _prevButtonState = buttonState;
            }

            if(ButtonDown())
                Debug.Log("Button down!");
            if(ButtonUp())
                Debug.Log("Button up!");
        }

        protected override void Initialize()
        {
            inputManager.OnRightTrigger += HandleButtonRight;
            inputManager.OnLeftTrigger += HandleButtonLeft;
            Debug.Log("Initialized");
        }

        void HandleButtonRight(bool on)
        {
            Debug.Log("HandleButtonRight called");
            if (!HandSwitcher.instance.CurrentHandIsRight()) return;
            buttonState = on;
        }
        
        void HandleButtonLeft(bool on)
        {
            Debug.Log("HandleButtonLeft called");
            if (!HandSwitcher.instance.CurrentHandIsLeft()) return;
            buttonState = on;
        }

        public override bool ButtonDown()
        {
            Debug.Log("ButtonDown called");

            return _buttonChanged && buttonState;
        }

        public override bool ButtonUp()
        {
            return _buttonChanged && !buttonState;
        }

        public override void OnEnterControl(GameObject control)
        {
            Debug.Log("OnEnterControl " + control.name);
        }

        public override void OnExitControl(GameObject control)
        {
            Debug.Log("OnExitControl " + control.name);
        }
    }

}