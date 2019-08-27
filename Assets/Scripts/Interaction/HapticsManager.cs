using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Controller
{

    // Manage vibration and user haptics
    // TO-DO: Actually integrate this with UnityEngine.XR abstraction layer :)
    
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