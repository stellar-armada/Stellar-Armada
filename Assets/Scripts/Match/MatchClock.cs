using StellarArmada.Match.Messaging;
using UnityEngine;
using UnityEngine.Events;

#pragma warning disable 0649
namespace StellarArmada.Match

{
    public class MatchClock
    {
        
        public MatchTimer currentTimer;
        public UnityEvent OnUpdate;

        public void Update()
        {
            OnUpdate?.Invoke();
        }

        private float countdownTime = 5f;
        
        private MatchStateManager _currentMatchManager;

        private MatchTimer PreMatchTimer;
        private MatchTimer DuringMatchTimer;
        private MatchTimer PostMatchTimer;

        public MatchClock(MatchStateManager matchManager, float _preMatchLength, float _matchLength)
        {
            _currentMatchManager = matchManager;

            // Prematch setup
            PreMatchTimer = new MatchTimer(this, countdownTime);
            
            // 30 second warning
            PreMatchTimer.SetTrigger(_preMatchLength - 30, () =>
            {
                MatchMessageManager.instance.CmdRaiseMessageToClients(MatchMessageType.CD_FLEET_ARRIVING_IN_30);
            });
            
            // 5 second countdown
            PreMatchTimer.SetTrigger(_preMatchLength - 6, () =>
            {
                MatchCountdown preMatchCountDown = new MatchCountdown(this, new MatchCountDownSettings(
                    MatchMessageType.CD_PREPARE_TO_ENGAGE_IN, CountdownType.PreMatch));
            });
            
            PreMatchTimer.OnTimerFinished += () =>
            {
                DuringMatchTimer?.Start();
                _currentMatchManager.CmdChangeMatchState(MatchState.Match);
                currentTimer = DuringMatchTimer;
            };

            
            // Match timer setup
            DuringMatchTimer = new MatchTimer(this, _matchLength);
            // 30 second warning to match ending
            DuringMatchTimer.SetTrigger(_matchLength - 30, () =>
            {
                MatchMessageManager.instance.CmdRaiseMessageToClients(MatchMessageType.CD_MATCH_ENDING_IN_30);
            });
            // 5 second countdown
            DuringMatchTimer.SetTrigger(_matchLength - 6, () =>
            {
                MatchCountdown newCount = new MatchCountdown(this, new MatchCountDownSettings(
                    MatchMessageType.CD_MATCH_ENDING_IN, CountdownType.Match ));
            });
            // 
            DuringMatchTimer.OnTimerFinished += () =>
            {
                _currentMatchManager.CmdFinishMatch();
                PostMatchTimer.Start();
                currentTimer = PostMatchTimer;
            };

            PostMatchTimer = new MatchTimer(this, 45);
            PostMatchTimer.SetTrigger(6, PostMatchCountdownEvent);
            PostMatchTimer.OnTimerFinished += OnPostMatchEnded;
        }

        public void Start()
        {
            MatchMessageManager.instance.CmdRaiseMessageToClients(MatchMessageType.CD_FLEET_ARRIVING_IN_30);
            PreMatchTimer.Start();
            currentTimer = PreMatchTimer;
        }

        void PostMatchCountdownEvent()
        {
            MatchCountdown newCount = new MatchCountdown(this, new MatchCountDownSettings(
                MatchMessageType.CD_SERVER_SHUTTING_DOWN_IN, CountdownType.PostMatch
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