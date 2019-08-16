using UnityEngine;
using Wacki;

#pragma warning disable 0649
namespace SpaceCommander.IO {

    public class UIPointer : IUILaserPointer
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
        }

        protected override void Initialize()
        {
            inputManager.OnRightTrigger += HandleButtonRight;
            inputManager.OnLeftTrigger += HandleButtonLeft;
        }

        void HandleButtonRight(bool on)
        {
            if (!HandSwitcher.instance.CurrentHandIsRight()) return;
            buttonState = on;
        }
        
        void HandleButtonLeft(bool on)
        {
            if (!HandSwitcher.instance.CurrentHandIsLeft()) return;
            buttonState = on;
        }

        public override bool ButtonDown()
        {
            return _buttonChanged && buttonState;
        }

        public override bool ButtonUp()
        {
            return _buttonChanged && !buttonState;
        }

        public override void OnEnterControl(GameObject control)
        {
            base.OnEnterControl(control);
        }

        public override void OnExitControl(GameObject control)
        {
            base.OnExitControl(control);
        }
    }

}