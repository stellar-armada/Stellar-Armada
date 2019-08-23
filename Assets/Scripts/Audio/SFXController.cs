using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Audio
{
    // Manage all UI and non-weapon, non HUD message/announcement messages

    
    public enum SFXType
    {
        VOICECHAT_ENGAGED,
        KEY_PRESSED,
        KEY_BACKSPACE,
        BUTTON_ACCEPT,
        BUTTON_CANCEL,
        CANVAS_OPEN,
        CANVAS_CLOSED,
        BUTTON_HIGHLIGHT,
        PLAYERSPAWN,
        PLAYERDEATH,
        ENEMYHIT,
        FRIENDLYHEALED,
        ENEMYKILLED,
        HEADSHOT
    }


    public class SFXController : MonoBehaviour
    {
        public static SFXController instance; // singleton accessor

        [SerializeField] SfxAudioData SFXAudioLookup;

        [SerializeField] AudioSource sfxAudioSource;

        private AudioSource[] audioSources;

        [SerializeField] int numberOfAudioSources = 2;
        private int currentAudioSource;
        public void Init()
        {
            audioSources = new AudioSource[numberOfAudioSources];
            audioSources[0] = sfxAudioSource;
            for (int i = 1; i < numberOfAudioSources; i++)
            {
                AudioSource newAud = (AudioSource)CopyComponent(sfxAudioSource);
                audioSources[i] = newAud;
            }
            instance = this;
        }

        private void OnDestroy()
        {
            instance = null;
        }
        
        Component CopyComponent(Component original)
        {
            System.Type type = original.GetType();
            Component copy = original.gameObject.AddComponent(type);
            System.Reflection.FieldInfo[] fields = type.GetFields(); 
            foreach (System.Reflection.FieldInfo field in fields)
            {
                field.SetValue(copy, field.GetValue(original));
            }
            return copy;
        }

        public void PlayOneShot(SFXType _type)
        {
            if (SFXAudioLookup.audioClips.ContainsKey(_type))
                audioSources[++currentAudioSource%numberOfAudioSources].PlayOneShot(SFXAudioLookup.audioClips[_type]);
            else
                Debug.LogError(_type + " not found in SFXAudioLookup");
        }
    }
}