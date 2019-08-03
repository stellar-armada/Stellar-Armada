using UnityEngine;

namespace SpaceCommander.Player.Mobile
{
    public class HapticsController : MonoBehaviour
    {
#if UNITY_MOBILE
        // Start is called before the first frame update
        void Awake()
        {
            InputManager.Haptics_Shoot += HandleShoot;
            InputManager.Haptics_OutOfAmmo += HandleOutOfAmmo;
            InputManager.Haptics_Death += HandleDeath;
            InputManager.Haptics_Spawn += HandleSpawn;
            InputManager.Haptics_Kill += HandleKill;
            InputManager.Haptics_CalibrationSuccessful += HandleCalibrationSuccessful;
            InputManager.Haptics_ButtonPress += HandleButtonPress;
            InputManager.Haptics_Alert += HandleAlert;
            InputManager.Haptics_MatchStarted += HandleMatchStarted;
            InputManager.Haptics_MatchEnded += HandleMatchEnded;
            InputManager.Haptics_MatchCountdown += HandleMatchCountdown;
        }
        
        void HandleShoot()
        {
            if (!Application.isEditor)
                Handheld.Vibrate();
        }

        void HandleOutOfAmmo()
        {
            StartCoroutine(DoubleVibrate());
        }

        void HandleMatchCountdown()
        {
            StartCoroutine(VibrateLong());
        }

        void HandleDeath()
        {
            StartCoroutine(TripleVibrateLong());
        }

        void HandleKill()
        {
            StartCoroutine(DoubleVibrate());
        }

        void HandleSpawn()
        {
            StartCoroutine(DoubleVibrateLong());
        }

        void HandleCalibrationSuccessful()
        {
            StartCoroutine(DoubleVibrateLong());
        }

        void HandleButtonPress()
        {
            Handheld.Vibrate();
        }

        void HandleAlert()
        {
            StartCoroutine(VibrateLong());
        }

        void HandleMatchEnded()
        {
            StartCoroutine(TripleVibrateLong());
        }

        void HandleMatchStarted()
        {
            StartCoroutine(VibrateLong());
        }

        IEnumerator VibrateLong()
        {
            if (Application.isEditor)
                yield break;
            Handheld.Vibrate();
            yield return null;
            Handheld.Vibrate();
        }

        IEnumerator DoubleVibrateLong()
        {
            if (Application.isEditor)
                yield break;
            Handheld.Vibrate();
            yield return null;
            Handheld.Vibrate();
            yield return new WaitForSeconds(.1f);
            Handheld.Vibrate();
            yield return null;
            Handheld.Vibrate();
        }

        IEnumerator DoubleVibrate()
        {
            if (Application.isEditor)
                yield break;

            Handheld.Vibrate();
            yield return new WaitForSeconds(.1f);
            Handheld.Vibrate();
        }

        IEnumerator TripleVibrateLong()
        {
            if (Application.isEditor)
                yield break;
            Handheld.Vibrate();
            yield return null;
            Handheld.Vibrate();
            yield return new WaitForSeconds(.1f);
            Handheld.Vibrate();
            yield return null;
            Handheld.Vibrate();
        }
#endif
    }
}