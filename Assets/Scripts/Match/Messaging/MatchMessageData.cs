using UnityEngine;

#pragma warning disable 0649
namespace SpaceCommander.Match.Messaging {
    
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
    public class MessageDictionary : SerializableDictionary<MatchMessageType, SerializedMessage>
    {
    }

    [CreateAssetMenu(fileName = nameof(MatchMessageData), menuName = "Message Data", order = 51)]
    public class MatchMessageData : ScriptableObject
    {
        public MessageDictionary messages;
    }
}