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

        private bool leftMenuButtonActive;
        private bool rightMenuButtonActive;

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

        void Awake()
        {
            pointer.SetActive(false);
        }

        protected override void Initialize()
        {
            inputManager.OnRightTrigger += HandleButtonRight;
            inputManager.OnLeftTrigger += HandleButtonLeft;
            inputManager.OnLeftThumbstickButton += HandleLeftMenuButtonActivated;
            inputManager.OnRightThumbstickButton += HandleRightMenuButtonActivated;
        }

        void HandleLeftMenuButtonActivated(bool down)
        {
            if (rightMenuButtonActive) return;
            if (!down && !leftMenuButtonActive) return; // if button going up but down state was blocked by other side button, ignore action beyond this point
            leftMenuButtonActive = down; // filter ups from this side if race conditioned to other side
            pointer.SetActive(down);
        }

        void HandleRightMenuButtonActivated(bool down)
        {
            if (leftMenuButtonActive) return;
            if (!down && !rightMenuButtonActive) return; // if button going up but down state was blocked by other side button, ignore action beyond this point
            rightMenuButtonActive = down;
            pointer.SetActive(down);
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