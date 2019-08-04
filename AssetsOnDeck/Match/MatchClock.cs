using SpaceCommander.UI;
using UnityEngine;

namespace SpaceCommander.Game

{
    public class MatchClock: Clock
    {

        private float countdownTime = 5f;
        
        private Match currentMatch;

        private Timer PreMatchTimer;
        private Timer MatchTimer;
        private Timer PostMatchTimer;

        public MatchClock(Match _match, float _matchLength)
        {
            currentMatch = _match;

            PreMatchTimer = new Timer(this, countdownTime);
            PreMatchTimer.SetTrigger(24, PreMatchCountDownEvent);
            PreMatchTimer.OnTimerFinished += StartMatchEvent;

            MatchTimer = new Timer(this, _matchLength);
            MatchTimer.SetTrigger(_matchLength - 30, MatchEndingWarningEvent);
            MatchTimer.SetTrigger(_matchLength - 6, MatchEndingCountdownEvent);
            MatchTimer.OnTimerFinished += EndMatchEvent;

            PostMatchTimer = new Timer(this, 45);
            PostMatchTimer.SetTrigger(15, PostMatchWarningEvent);
            PostMatchTimer.SetTrigger(6, PostMatchCountdownEvent);
            PostMatchTimer.OnTimerFinished += OnPostMatchEnded;
        }

        public void Start()
        {
            currentMatch.RaiseMessageToClients(MessageType.CD_MATCH_STARTING_IN_30);
            GameManager.instance.EventOnMatchStarted?.Invoke();
            PreMatchTimer.Start();
            currentTimer = PreMatchTimer;
        }

        private void PreMatchCountDownEvent()
        {
            Countdown preMatchCountDown = new Countdown(this, new CountDownSettings(
                MessageType.CD_MATCHSTARTING_IN, CountdownType.Prematch));
        }

        void StartMatchEvent()
        {
            Debug.Log("Starting match proper!");
            MatchTimer?.Start();
            currentMatch.SetState(MatchState.Started);
            currentTimer = MatchTimer;
        }

        void MatchEndingWarningEvent()
        {
            currentMatch.RaiseMessageToClients(MessageType.CD_MATCH_ENDING_IN_30);
        }

        void MatchEndingCountdownEvent()
        {
            Countdown newCount = new Countdown(this, new CountDownSettings(
                MessageType.CD_MATCHENDING_IN,
                CountdownType.Match
            ));
        }

        void EndMatchEvent()
        {
            currentMatch.EndMatch();
            PostMatchTimer.Start();
            currentTimer = PostMatchTimer;
        }

        void PostMatchWarningEvent()
        {
            currentMatch.RaiseMessageToClients(MessageType.CD_NEW_MATCH_IN_30);
        }

        void PostMatchCountdownEvent()
        {
            Countdown newCount = new Countdown(this, new CountDownSettings(
                MessageType.CD_NEWMATCH_IN, CountdownType.Postmatch
            ));
        }


        void OnPostMatchEnded()
        {
            if (currentMatch.isServer)
            {
                MatchManager.instance.CmdCreateNewMatch();
            }
        }

        public float GetMatchTime()
        {
            return currentTimer?.currentTime ?? 0;
        }

    }
}