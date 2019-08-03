using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SpaceCommander.Audio;
using SpaceCommander.Game;
using SpaceCommander.Player;
using SpaceCommander.UI;

namespace InputHandling
{
    /* InputManager handles all input from various devices.
     * Instantiation of the ML controller prefab object happens here (if your GameManager devicetype is Magic Leap)
     * Other devices such as the ML Controller can trigger InputManager events to fire by calling OnPrimaryButton?.Invoke() (for example)
     */
    public class InputManager : MonoBehaviour
    {
        public static InputManager instance;
        [SerializeField] GameObject magicLeapControllerPrefab;
        [SerializeField] private GameObject oculusRiftControllerPrefab;
        [SerializeField] private GameObject hololensControllerPrefab;
        [SerializeField] GameObject uiPointerPrefab;
        static Pointer uiPointer;
        public static GameObject controller;

        static bool triggerIsDown = false;
        
        static bool inputIsBlocked;

        public delegate void EventHandler();
        
        static PlayerController localPlayerController;
        
        static bool localPlayerIsInited;
        
        public static EventHandler Haptics_ButtonPress;
        public static EventHandler Haptics_Alert;
        public static EventHandler Haptics_MatchStarted;
        public static EventHandler Haptics_MatchEnded;
        public static EventHandler Haptics_MatchCountdown;

        private static bool pushToTalkIsDown;

        static GameObject go;
        
        static PlayerController localPlayer;

        public void Init()
        {
            instance = this;

            // reset static variables
            controller = null;
            uiPointer = null;
            inputIsBlocked = false;
        }

        public static void HandleHaptics_ButtonPress()
        {
            Haptics_ButtonPress?.Invoke();
        }

        public static void HandleHaptics_Alert()
        {
            Haptics_Alert?.Invoke();
        }

        public static void HandleHaptics_MatchStarted()
        {
            Haptics_MatchStarted?.Invoke();
        }

        public static void HandleHaptics_MatchEnded()
        {
            Haptics_MatchEnded?.Invoke();
        }

        public static void HandleHaptics_MatchCountdownTick()
        {
            Haptics_MatchCountdown?.Invoke();
        }
        

        public static void HandleSecondaryInputButtonPressed(bool buttonIsDown)
        {
            if (!localPlayerIsInited) return;


            if (Match.IsStarted())
            {

                Debug.Log("Secondary input here");
            }
        }

        public static void PushToTalkPressed(bool buttonIsDown)
        {
            pushToTalkIsDown = buttonIsDown;
            if (buttonIsDown) SFXController.instance.PlayOneShot(SFXType.VOICECHAT_ENGAGED);
            PlayerCanvasController.instance.ToggleMicrophoneUIImage(buttonIsDown);   
        }

        public static Pointer GetUIPointer()
        {
            return uiPointer;
        }

        static void PointerUp()
        {
        }


        static void PointerDown()
        {
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                go = EventSystem.current.currentSelectedGameObject;
                if (go != null && go.GetComponent<Selectable>() != null)
                {
                    HandleHaptics_ButtonPress();
                }
            }
        }

        public static void HandleMenuButtonPressed(bool buttonIsDown)
        {
            if (inputIsBlocked)
            {
                Debug.Log("Can't bump, input is blocked");
                return;
            }

            if (!buttonIsDown) return;
            PlayerCanvasController.instance.ToggleControllerCanvas();
        }


        public static void HandleMainInputButtonPressed(bool triggerDown)
        {
            Debug.Log("Shooting");
            if (inputIsBlocked)
            {
                Debug.Log("Can't shoot, input is blocked");
                return;
            }
            
            if (triggerDown)
                    PointerDown();
                else
                    PointerUp();
                return;

                if (!localPlayerIsInited)
            {
                Debug.Log("Can't shoot, local player isn't inited");
                return; // if we're not in UI and there's no local player, then there's no weapon and no reason to shoot
            }

            Match currentMatch = Match.GetCurrentMatch();
        }

        private void OnDestroy()
        {
            instance = null;
            localPlayerIsInited = false;
            localPlayerController = null;
            //  Null all static events
            Haptics_ButtonPress = Haptics_Alert = Haptics_MatchStarted = Haptics_MatchEnded = Haptics_MatchCountdown = null;
        }


        public static void CreatePointer()
        {
            if (uiPointer != null)
            {
                Debug.Log("Can't create a uiPointer, already exists!");
                return;
            }

            GameObject uiPointerObj = null;

            uiPointerObj = Instantiate(instance.uiPointerPrefab, controller.transform);
            uiPointer = uiPointerObj.GetComponent<Pointer>();
        }

        public static void CreateController()
        {
            if (controller != null)
            {
                Debug.Log("Can't create a controller, already exists!");
                return;
            }
            
            controller = Instantiate(instance.oculusRiftControllerPrefab, LocalRig.instance.transform);
        }
    }
}