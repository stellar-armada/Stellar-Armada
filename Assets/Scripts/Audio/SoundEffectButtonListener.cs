using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace SpaceCommander.Audio
{
    // Add class to buttons for them to have UI sounds
    public class SoundEffectButtonListener : MonoBehaviour, ISelectHandler
    {
        [SerializeField] bool isCancelButton = false;

        Button button;

        void Awake()
        {
            button = GetComponent<Button>();
            if (isCancelButton)
            {
                button.onClick.AddListener(()=>SFXController.instance.PlayOneShot(SFXType.BUTTON_CANCEL));
            }
            else
            {
                button.onClick.AddListener(()=>SFXController.instance.PlayOneShot(SFXType.BUTTON_CANCEL));

            }

        }

        public void OnSelect(BaseEventData eventData)
        {
            SFXController.instance.PlayOneShot(SFXType.BUTTON_HIGHLIGHT);
        }

    }
}
