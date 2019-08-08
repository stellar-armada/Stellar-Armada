using UnityEngine;

namespace SpaceCommander.Controller
{
    /* InputManager handles all input from various devices.
     * Instantiation of the ML controller prefab object happens here (if your GameManager devicetype is Magic Leap)
     * Other devices such as the ML Controller can trigger InputManager events to fire by calling OnPrimaryButton?.Invoke() (for example)
     */
    public class HapticsManager : MonoBehaviour
    {
        public static HapticsManager instance;

        public delegate void EventHandler();

        public static EventHandler Haptics_ButtonPress;
        public static EventHandler Haptics_Alert;
        public static EventHandler Haptics_MatchStarted;
        public static EventHandler Haptics_MatchEnded;
        public static EventHandler Haptics_MatchCountdown;

        public void Awake()
        {
            instance = this;
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
        
    }
}