using System.Collections;
using UnityEngine;
using SpaceCommander.Audio;
using SpaceCommander.Game;
using SpaceCommander.Haptics;
using TMPro;

namespace SpaceCommander.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class MatchMessage : MonoBehaviour
    {
        public bool hasPriority = false;
        public bool isActive = false;
        private MessageType messageType;
        [SerializeField] float fadeTime = .5f;
        [SerializeField] private TextMeshProUGUI textSingle;
        [SerializeField] private TextMeshProUGUI textDoubleHeader;
        [SerializeField] private TextMeshProUGUI textDoubleHeaderSecondary;


        bool holdMessage = true;
        float holdTime = 3f;
        bool hapticAlert = false;

        CanvasGroup group;
        bool fading = false;
        float lastShown = 0f;

        #region Events // Subscribe to these for special message-specific logic and scripts

        public event GameManager.EventHandler
            EventOnMessageShown; // GameManager's void EventHandler is fine for our event purposes

        public event GameManager.EventHandler EventOnMessageHidden;

        #endregion

        #region Initialization and Deinitialization

        private void Awake()
        {
            group = GetComponent<CanvasGroup>();
        }

        void Init()
        {
            group = GetComponent<CanvasGroup>();
        }

        private void OnDisable()
        {
            group.alpha = 0f;
        }

        #endregion

        public void ShowMessage(MessageType _type, SerializedMessage _message)
        {
            //Debug.Log("Showing message of type " + _type.ToString());
            messageType = _type;

            if (_message.subMessageContents == "")
            {
                textSingle.text = _message.messageContents;
                textDoubleHeader.text = "";
                textDoubleHeaderSecondary.text = "";
            }
            else
            {
                textSingle.text = "";
                textDoubleHeader.text = _message.messageContents;
                textDoubleHeaderSecondary.text = _message.subMessageContents;
            }
            
            holdMessage = _message.holdMessage;
            holdTime = _message.holdTime;
            hapticAlert = _message.hapticAlert;
            ShowMessage();
        }

        public void ShowMessage()
        {
            if (MatchMessageManager.instance.HasCurrentMessage())
            {
                MatchMessageManager.GetCurrentMessage().HideMessage();
            }

            lastShown = Time.time;
            gameObject.SetActive(true);
            MatchMessageManager.SetCurrentMessage(this);

            if (fading == true)
            {
                StopAllCoroutines();
            }

            Init();
            
            if ((int)messageType < 5)
            {
                isActive = true;
                group.alpha = 1;
            }
            else
                StartCoroutine(FadeIn());

            MessageAudioController.instance.PlayOneShot(messageType);

            if (hapticAlert)
            {
                HapticsManager.HandleHaptics_Alert();
            }

            EventOnMessageShown?.Invoke();
        }

        public void HideMessage()
        {
            if (!isActive) return;
            if (fading == true)
            {
                StopAllCoroutines();
            }

            if (MatchMessageManager.GetCurrentMessage() == this)
            {
                MatchMessageManager.ClearCurrentMessage();
            }

            gameObject.SetActive(false);
            isActive = false;
            EventOnMessageHidden?.Invoke();
        }

        #region Private Methods

        void FadeAndHideMessage()
        {
            if (!isActive) return;
            if (fading == true)
            {
                StopAllCoroutines();
            }

            StartCoroutine(FadeOut(.25f));


            isActive = false;
            EventOnMessageHidden?.Invoke();
        }

        IEnumerator FadeIn()
        {
            isActive = true;
            group.alpha = 0;
            float timer = 0;
            fading = true;
            float startAlpha = group.alpha;

            do
            {
                timer += Time.deltaTime;
                group.alpha = Mathf.Lerp(group.alpha, 1f, timer / fadeTime);
                yield return null;
            } while (timer <= fadeTime);

            fading = false;
        }

        IEnumerator FadeOut(float delay)
        {
            yield return new WaitForSeconds(delay);
            isActive = false;
            float timer = 0;
            fading = true;
            float startAlpha = group.alpha;
            do
            {
                timer += Time.deltaTime;
                group.alpha = Mathf.Lerp(group.alpha, 1f, timer / fadeTime);
                yield return null;
            } while (timer <= fadeTime);

            fading = false;
            group.alpha = 0;

            gameObject.SetActive(false);
        }

        void Update()
        {
            if (holdMessage && Time.time > lastShown + holdTime && isActive)
            {
                FadeAndHideMessage();
            }
        }

        #endregion
    }
}