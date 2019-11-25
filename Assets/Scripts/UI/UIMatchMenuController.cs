using StellarArmada.IO;
using StellarArmada.Match;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.UI {

    // Local player UI pointing handler
    // Inherits from the IUILaserPointer class which interacts with the Unity EventSystem for VR input
    // TO-DO: Move button-specific logic to input manager and abstract
    public class UIMatchMenuController : MonoBehaviour
    {
        [SerializeField] private InputManager inputManager;
        
        // State
        // TO-DO: All these bools look ugly, could use some refactor
        private bool leftMenuButtonActive;
        private bool rightMenuButtonActive;
        bool prevButtonState; // Store last frame state for triggering EventSystem.Process
        bool buttonChanged; // Called in the event system process loop if true, then falsed next frame

        void Start()
        {
            Initialize();
        }
        // Initialization is called from the base class Start
        protected void Initialize()
        {
            inputManager.OnButtonFour += HandleLeftMenuButtonActivated;
            inputManager.OnButtonTwo += HandleRightMenuButtonActivated;
        }

        // If the menu button is activated, enable the pointer (works until we need the pointer elsewhere!)
        void HandleLeftMenuButtonActivated(bool down)
        {
            if (MatchStateManager.instance == null || !MatchStateManager.instance.InMatch()) return; // Only use this functionality in the match state
            if (!HandSwitcher.instance.CurrentHandIsLeft()) return; // Only show menu on right hand if it's active
            if (rightMenuButtonActive) return; // If the button is being used on the other hand, ignore
            if (!down && !leftMenuButtonActive) return; // if button going up but down state was blocked by other side button, ignore action beyond this point
            leftMenuButtonActive = down; // filter ups from this side if race conditioned to other side
            MatchPlayerMenuManager.instance.SetMenuState(down); // If we're in the match, this will show the player's hotbar menu
        }

        void HandleRightMenuButtonActivated(bool down)
        {
            if (MatchStateManager.instance == null || !MatchStateManager.instance.InMatch()) return; // Only use this functionality in the match state
            if (!HandSwitcher.instance.CurrentHandIsRight()) return; // Only show menu on right hand if it's active
            if (leftMenuButtonActive) return; // If the button is being used on the other hand, ignore
            if (!down && !rightMenuButtonActive) return; // if button going up but down state was blocked by other side button, ignore action beyond this point
            rightMenuButtonActive = down;
            MatchPlayerMenuManager.instance.SetMenuState(down); // If we're in the match, this will show the player's hotbar menu
        }
    }

}