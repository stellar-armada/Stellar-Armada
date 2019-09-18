using System;
using System.Collections;
using Mirror;
using UnityEngine;

// remove 67 for production. 649 is just null checking serialized values
#pragma warning disable 0067
#pragma warning disable 0649
namespace StellarArmada.Match.Messaging
{
    /* Manager for Hud Messages (notifications at the bottom of player's screen) */

    public enum MatchMessageType
    {
        
        // Numbers
        CD_1 = 1,
        CD_2 = 2,
        CD_3 = 3,
        CD_4 = 4,
        CD_5 = 5,
        
        // Win condition
        VICTORY = 21,
        DEFEAT = 22,
        DRAW = 23,
        RED_TEAM_WINS = 24,
        BLUE_TEAM_WINS = 25,
        YOU_WIN = 26,
        YOU_LOSE = 27,

        // Player status
        MATCH_HAS_BEGUN = 33,
        
        // Counddown
        CD_FLEET_ARRIVING_IN_30 = 41,
        CD_MATCH_ENDING_IN_30 = 42,
        CD_PREPARE_TO_ENGAGE_IN = 44,
        CD_MATCH_ENDING_IN = 45,
        CD_SERVER_SHUTTING_DOWN_IN = 46,

        NONE = 100
        
    }

    public class MatchMessageManager : NetworkBehaviour
    {
        public static MatchMessageManager instance; // singleton accessor

        static MatchMessage currentMessage;

        [SerializeField] MatchMessageData matchMessageData;

        [SerializeField] MatchMessage MessageTemplate;
        
        public delegate void MessageRaisedDelegate(MatchMessageType t);
        
        [SyncEvent] public event MessageRaisedDelegate EventOnNewMessage;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.LogError("instance != this.");
            }

            instance = this;
        }


        private void OnDestroy()
        {
            instance = null;
            currentMessage = null;
        }

        public void HideCurrentMessage()
        {
            if(currentMessage != null) currentMessage.HideMessage();
        }

        public bool HasCurrentMessage()
        {
            if (currentMessage != null) return true;
            else return false;
        }

        public static bool CurrentMessageHasPriority()
        {
            if (currentMessage == null)
            {
                return false;
            }
            else if (currentMessage.hasPriority)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        [Command]
        public void CmdRaiseMessageToClients(MatchMessageType _t)
        {
            RaiseMessage(_t);
        }
        public static MatchMessage GetCurrentMessage()
        {
            return currentMessage;
        }

        public static void SetCurrentMessage(MatchMessage msg)
        {
            currentMessage = msg;
        }
        

        public static void ClearCurrentMessage()
        {
            currentMessage = null;
        }

        void RaiseMessage(MatchMessageType _type)
        {
            if (!matchMessageData.messages.ContainsKey(_type))
                Debug.LogError("Message Type " + _type + " not found in message lookup");
            else
                MessageTemplate.ShowMessage(_type,matchMessageData.messages[_type]);
        }
        
        public void RaiseMessageDelayedWithCondition(MatchMessageType _type, Func<bool> conditionToCheck,bool evaluatesToTrue, float delay )
        {
            StartCoroutine(iRaiseMessageDelayedWithCondition(_type, conditionToCheck,evaluatesToTrue, delay ));
        }
        
        public void RaiseMessageDelayedWithReturnLogic(Func<MatchMessageType> Method, float delay )
        {
            StartCoroutine(iRaiseMessageDelayedWithCondition(Method(), delay ));
        }
        
        IEnumerator iRaiseMessageDelayedWithCondition(MatchMessageType _type, float delay )
        {

            if (_type == MatchMessageType.NONE) yield break;
            
                yield return new WaitForSeconds(delay);

            if (!matchMessageData.messages.ContainsKey(_type))
                Debug.LogError("Message Type " + _type + " not found in message lookup");
            else
                MessageTemplate.ShowMessage(_type, matchMessageData.messages[_type]);

        }

        IEnumerator iRaiseMessageDelayedWithCondition(MatchMessageType _type, Func<bool> conditionToCheck,bool evaluatesToTrue, float delay )
        {
            yield return new WaitForSeconds(delay);

            if (conditionToCheck() != evaluatesToTrue) yield break;

            if (!matchMessageData.messages.ContainsKey(_type))
                Debug.LogError("Message Type " + _type + " not found in message lookup");
            else
                MessageTemplate.ShowMessage(_type, matchMessageData.messages[_type]);

        }
        
        
    }
}