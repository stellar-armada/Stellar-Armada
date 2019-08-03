using System;
using System.Collections;
using UnityEngine;


namespace SpaceCommander.UI
{
    /* Manager for Hud Messages (notifications at the bottom of player's screen)
     * TO-DO: Make methods static so we don't need to call HudMessageManager.instance all over the place :)
     */

    public enum MessageType
    {
        
        // Numbers
        CD_1 = 1,
        CD_2 = 2,
        CD_3 = 3,
        CD_4 = 4,
        CD_5 = 5,
        
        // Game status
        YOU_ARE_HOST = 10,
        WAITING_FOR_HOST = 11,
        WAITING_FOR_CALIBRATION = 12,
        CALIBRATING = 13,
        CALIBRATION_SUCCESSFUL = 14,
        MATCH_PAUSED = 15,
        MATCH_RESTARTED = 16,
        
        // Win condition
        VICTORY = 21,
        DEFEAT = 22,
        DRAW = 23,
        RED_TEAM_WINS = 24,
        BLUE_TEAM_WINS = 25,
        YOU_WIN = 26,
        YOU_LOSE = 27,
        
        
        // Player status
        TAKING_DAMAGE_ENVIRONMENT = 31,
        TAKING_DAMAGE_PLAYER = 32,
        MATCH_HAS_BEGUN = 33,
        YOU_ARE_DEAD = 34,
        WARNING_SEVERE_DAMAGE = 35,
        WARNING_CRITICAL_DAMAGE = 36,
        SYSTEM_ONLINE = 37,
        SYSTEM_OFFLINE = 38,

        
        // Counddown
        CD_MATCH_STARTING_IN_30 = 41,
        CD_MATCH_ENDING_IN_30 = 42,
        CD_NEW_MATCH_IN_30 = 43,
        CD_MATCHSTARTING_IN = 44,
        CD_MATCHENDING_IN = 45,
        CD_MATCH_RESUMING_IN = 46,
        CD_NEWMATCH_IN = 47,
        CD_FIGHT = 48,
        
        NONE = 100
        
    }

    public class HudMessageManager : MonoBehaviour
    {
        public static HudMessageManager instance; // singleton accessor

        static HudMessage currentMessage;

        [SerializeField] MessageData messageData;

        [SerializeField] HudMessage GenericMessage;
        [SerializeField] CanvasGroup canvasGroup;

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
            //Debug.Log("Hiding message");
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

        public static HudMessage GetCurrentMessage()
        {
            return currentMessage;
        }

        public static void SetCurrentMessage(HudMessage msg)
        {
            currentMessage = msg;
        }
        
        public static void EnableHud()
        {
            instance.canvasGroup.alpha = 1.0f;
        }

        public static void DisableHud()
        {
            instance.canvasGroup.alpha = 0.0f;
        }

        public static void ClearCurrentMessage()
        {
            currentMessage = null;
        }

        public void RaiseMessage(MessageType _type)
        {
            if (!messageData.messages.ContainsKey(_type))
                Debug.LogError("Message Type " + _type + " not found in message lookup");
            else
                GenericMessage.ShowMessage(_type,messageData.messages[_type]);
        }
        
        public void RaiseMessageDelayedWithCondition(MessageType _type, Func<bool> conditionToCheck,bool evaluatesToTrue, float delay )
        {
            StartCoroutine(iRaiseMessageDelayedWithCondition(_type, conditionToCheck,evaluatesToTrue, delay ));
        }
        
        public void RaiseMessageDelayedWithReturnLogic(Func<MessageType> Method, float delay )
        {
            StartCoroutine(iRaiseMessageDelayedWithCondition(Method(), delay ));
        }
        
        IEnumerator iRaiseMessageDelayedWithCondition(MessageType _type, float delay )
        {

            if (_type == MessageType.NONE) yield break;
            
                yield return new WaitForSeconds(delay);

            if (!messageData.messages.ContainsKey(_type))
                Debug.LogError("Message Type " + _type + " not found in message lookup");
            else
                GenericMessage.ShowMessage(_type, messageData.messages[_type]);

        }

        IEnumerator iRaiseMessageDelayedWithCondition(MessageType _type, Func<bool> conditionToCheck,bool evaluatesToTrue, float delay )
        {
            yield return new WaitForSeconds(delay);

            if (conditionToCheck() != evaluatesToTrue) yield break;

            if (!messageData.messages.ContainsKey(_type))
                Debug.LogError("Message Type " + _type + " not found in message lookup");
            else
                GenericMessage.ShowMessage(_type, messageData.messages[_type]);

        }
        
        
    }
}