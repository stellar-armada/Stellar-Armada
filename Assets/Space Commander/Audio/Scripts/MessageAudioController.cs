using SpaceCommander.UI;
using UnityEngine;

namespace SpaceCommander.Audio
{


    public class MessageAudioController : MonoBehaviour
    {
        public static MessageAudioController instance; // singleton accessor
        
        [SerializeField] MatchMessageData matchMessageData;
        
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
            if (matchMessageData.messages.ContainsKey(_type))
                messageAudioSource.PlayOneShot(matchMessageData.messages[_type].audioClip);
            else
                Debug.LogError(_type.ToString() + " not found in MessageAudioLookup");
        }
    }
}