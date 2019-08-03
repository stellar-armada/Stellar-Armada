using UnityEngine;

namespace SpaceCommander.UI {
    
    [System.Serializable]
    public class SerializedMessage
    {
        public AudioClip audioClip;
        public string messageContents;
        public string subMessageContents;
        public bool holdMessage;
        public float holdTime;
        public bool hapticAlert;
    }

    [System.Serializable]
    public class MessageDictionary : SerializableDictionary<MessageType, SerializedMessage>
    {
    }

    [CreateAssetMenu(fileName = nameof(MessageData), menuName = "Message Data", order = 51)]
    public class MessageData : ScriptableObject
    {
        public MessageDictionary messages;
    }
}