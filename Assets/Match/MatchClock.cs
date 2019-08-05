using SpaceCommander.UI;
using UnityEngine;

namespace SpaceCommander.Match

{
    public class MatchClock: Clock
    {
        private float countdownTime = 5f;
        
        private MatchStateManager _currentMatchManager;

        private Timer PreMatchTimer;
        private Timer MatchTimer;
        private Timer PostMatchTimer;

        public MatchClock(MatchStateManager matchManager, float _preMatchLength, float _matchLength)
        {
            _currentMatchManager = matchManager;

            // Prematch setup
            PreMatchTimer = new Timer(this, countdownTime);
            
            // 30 second warning
            PreMatchTimer.SetTrigger(_preMatchLength - 30, () =>
            {
                MatchMessageManager.instance.CmdRaiseMessageToClients(MessageType.CD_FLEET_ARRIVING_IN_30);
            });
            
            // 5 second countdown
            PreMatchTimer.SetTrigger(_preMatchLength - 6, () =>
            {
                MatchCountdown preMatchCountDown = new MatchCountdown(this, new MatchCountDownSettings(
                    MessageType.CD_PREPARE_TO_ENGAGE_IN, CountdownType.PreMatch));
            });
            
            PreMatchTimer.OnTimerFinished += () =>
            {
                MatchTimer?.Start();
                _currentMatchManager.CmdChangeMatchState(MatchState.Match);
                currentTimer = MatchTimer;
            };

            
            // Match timer setup
            MatchTimer = new Timer(this, _matchLength);
            // 30 second warning to match ending
            MatchTimer.SetTrigger(_matchLength - 30, () =>
            {
                MatchMessageManager.instance.CmdRaiseMessageToClients(MessageType.CD_MATCH_ENDING_IN_30);
            });
            // 5 second countdown
            MatchTimer.SetTrigger(_matchLength - 6, () =>
            {
                MatchCountdown newCount = new MatchCountdown(this, new MatchCountDownSettings(
                    MessageType.CD_MATCH_ENDING_IN, CountdownType.Match ));
            });
            // 
            MatchTimer.OnTimerFinished += () =>
            {
                _currentMatchManager.CmdFinishMatch();
                PostMatchTimer.Start();
                currentTimer = PostMatchTimer;
            };

            PostMatchTimer = new Timer(this, 45);
            PostMatchTimer.SetTrigger(6, PostMatchCountdownEvent);
            PostMatchTimer.OnTimerFinished += OnPostMatchEnded;
        }

        public void Start()
        {
            MatchMessageManager.instance.CmdRaiseMessageToClients(MessageType.CD_FLEET_ARRIVING_IN_30);
            PreMatchTimer.Start();
            currentTimer = PreMatchTimer;
        }

        void PostMatchCountdownEvent()
        {
            MatchCountdown newCount = new MatchCountdown(this, new MatchCountDownSettings(
                MessageType.CD_SERVER_SHUTTING_DOWN_IN, CountdownType.PostMatch
            ));
        }


        void OnPostMatchEnded()
        {
            if (_currentMatchManager.isServer)
            {
                //Disconnect all the users and stop the server. Then restart the server scene
               Debug.LogError("Match ended, but handler hasn't been created");
            }
        }

        public float GetMatchTime()
        {
            return currentTimer?.currentTime ?? 0;
        }

    }
}