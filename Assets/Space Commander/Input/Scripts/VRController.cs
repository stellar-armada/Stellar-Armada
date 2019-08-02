using System.Collections.Generic;
using SpaceCommander.Game;
using SpaceCommander.UI;
using UnityEngine;
using UnityEngine.XR;

namespace InputHandling
{
    public class VRController : MonoBehaviour
    {
        private VRInputModule oculusInputModule;

        private List<InputDevice> inputDevices;

        private bool triggerButtonWasDown;
        private bool gripButtonWasDown;
        private bool menuButtonWasDown;
        bool triggerButton;
        bool gripButton;
        bool menuButton;

        private Canvas canvas;

        private bool isInited;
        private Camera cam;

        void Awake()
        {
            oculusInputModule = GetComponent<VRInputModule>();
            inputDevices = new List<InputDevice>();
            InputDevices.GetDevicesAtXRNode(XRNode.RightHand, inputDevices);

            Invoke(nameof(Init), 1f);
        }

        void Init()
        {
            isInited = true;
            canvas = VRCanvas.instance.canvas;
            canvas.worldCamera = oculusInputModule.uiCamera;
            cam = oculusInputModule.uiCamera;
        }



        // Update is called once per frame
        void Update()
        {

            // Forcing all this stuff into the update to avoid race conditions
            // Once everything works, we should refactor so that we're not throwing so many if statements in the update

            if (!isInited) return;

            if (canvas.worldCamera != cam)
            {
                canvas.worldCamera = cam;
            }

            if (inputDevices.Count < 1)
            {
                InputDevices.GetDevicesAtXRNode(XRNode.RightHand, inputDevices);
            }

            if (inputDevices.Count < 1) return;
            Vector2 dPad;
            if (inputDevices[0].TryGetFeatureValue(CommonUsages.dPad, out dPad))
            {

            }

            if (inputDevices[0].TryGetFeatureValue(CommonUsages.triggerButton, out triggerButton))
            {
                if (triggerButton && !triggerButtonWasDown)
                {

                    // do trigger down stuff
                    InputManager.HandleMainInputButtonPressed(true);
                    triggerButtonWasDown = true;
                    VRInputManager.SetIsControllerButtonPressed(true);
                }
                else if (!triggerButton && triggerButtonWasDown)
                {
                    // do trigger up stuff
                    InputManager.HandleMainInputButtonPressed(false);
                    triggerButtonWasDown = false;
                    VRInputManager.SetIsControllerButtonPressed(false);

                }
            }


            if (inputDevices[0].TryGetFeatureValue(CommonUsages.gripButton, out gripButton))
            {

                if (gripButton && !gripButtonWasDown)
                {
                    // do trigger down stuff
                    InputManager.HandleSecondaryInputButtonPressed(true);
                    gripButtonWasDown = true;
                }
                else if (!gripButton && gripButtonWasDown)
                {
                    // do trigger up stuff
                    InputManager.HandleSecondaryInputButtonPressed(false);
                    gripButtonWasDown = false;
                }
            }

            if (inputDevices[0].TryGetFeatureValue(CommonUsages.primaryButton, out menuButton))
            {

                if (menuButton && !menuButtonWasDown)
                {
                    // do trigger down stuff
                    InputManager.HandleMenuButtonPressed(true);
                    menuButtonWasDown = true;
                }
                else if (!menuButton && menuButtonWasDown)
                {
                    // do trigger up stuff
                    InputManager.HandleMenuButtonPressed(false);
                    menuButtonWasDown = false;
                }
            }

        }
    }
}
