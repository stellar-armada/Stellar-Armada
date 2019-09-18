using StellarArmada.IO;
using StellarArmada.Match;
using UnityEngine;
using Wacki;

#pragma warning disable 0649
namespace StellarArmada.UI {

    // Local player UI pointing handler
    // Inherits from the IUILaserPointer class which interacts with the Unity EventSystem for VR input
    // TO-DO: Move button-specific logic to input manager and abstract
    public class UIPointer : IUIPointer
    {
        [SerializeField] private InputManager inputManager;

        public static UIPointer instance;
        
        // State
        // TO-DO: All these bools look ugly, could use some refactor
        private bool leftMenuButtonActive;
        private bool rightMenuButtonActive;
        public bool buttonState; // Store current button state
        bool prevButtonState; // Store last frame state for triggering EventSystem.Process
        bool buttonChanged; // Called in the event system process loop if true, then falsed next frame

        public Canvas uiPlacerCanvas;
        
        protected override void Update()
        {
            base.Update(); // Most of the magic happens in the base class

            // 
            if(buttonState == prevButtonState) {
                buttonChanged = false;
            } else {
                buttonChanged = true;
                prevButtonState = buttonState;
            }
        }

        void Awake()
        {
            instance = this;
            pointer.SetActive(false); // Hide on start
        }

        // Initialization is called from the base class Start
        protected override void Initialize()
        {
            inputManager.OnLeftTrigger += HandleButtonLeft;
            inputManager.OnRightTrigger += HandleButtonRight;

            inputManager.OnLeftGrip += HandleSecondaryLeft;
            inputManager.OnRightGrip += HandleSecondaryRight;
        }

        void HandleSecondaryRight(bool down)
        {
            if (!HandSwitcher.instance.CurrentHandIsRight()) return; // Only show menu on right hand if it's active
            if (leftMenuButtonActive) return; // If the button is being used on the other hand, ignore
            if (!down && !rightMenuButtonActive) return; // if button going up but down state was blocked by other side button, ignore action beyond this point

            if (down) HandleSecondary();
        }
        
        void HandleSecondaryLeft(bool down)
        {
            if (!HandSwitcher.instance.CurrentHandIsLeft()) return; // Only show menu on right hand if it's active
            if (rightMenuButtonActive) return; // If the button is being used on the other hand, ignore
            if (!down && !leftMenuButtonActive) return; // if button going up but down state was blocked by other side button, ignore action beyond this point
            
            if (down) HandleSecondary();
        }
        

        void HandleSecondary()
        {
            if (Rollover.currentRollover == null) return;
            Rollover.currentRollover.HandleSecondaryButtonPressed();
        }

        void HandleButtonRight(bool on)
        {
            if (!HandSwitcher.instance.CurrentHandIsRight()) return;
            buttonState = on;
        }
        
        void HandleButtonLeft(bool on)
        {
            if (!HandSwitcher.instance.CurrentHandIsLeft()) return;
            Debug.Log("HandleButtonLeft");
            buttonState = on;
            Debug.Log("ButtonLeft called");
        }

        public override bool ButtonDown()
        {
            return buttonChanged && buttonState;
        }

        public override bool ButtonUp()
        {
            return buttonChanged && !buttonState;
        }

        // Hooks to do stuff if we've entered or exited a canvas
        
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