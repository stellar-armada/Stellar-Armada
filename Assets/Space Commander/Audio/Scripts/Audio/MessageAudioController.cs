using SpaceCommander.UI;
using UnityEngine;

namespace SpaceCommander.Audio
{


    public class MessageAudioController : MonoBehaviour
    {
        public static MessageAudioController instance; // singleton accessor
        
        [SerializeField] MessageData messageData;
        
        [SerializeField] AudioSource messageAudioSource;

        public void Init()
        {
            instance = this;
        }

        private void OnDestroy()
        {
            instance = null;
        }
        
        public void PlayOneShot(MessageType _type)
        {
            if (messageData.messages.ContainsKey(_type))
                messageAudioSource.PlayOneShot(messageData.messages[_type].audioClip);
            else
                Debug.LogError(_type.ToString() + " not found in MessageAudioLookup");
        }
    }
}