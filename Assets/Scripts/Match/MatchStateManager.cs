using Mirror;

#pragma warning disable 0649
namespace SpaceCommander.Match
{
    public enum MatchState
    {
        Lobby,
        Match,
        Ended
    }

    public class MatchStateManager : NetworkBehaviour
    {
        public static MatchStateManager instance; // private singleton with public GetCurrentMatch accessor

        // State

        public delegate void StateChangeDelegate();

        [SyncEvent]
        public event StateChangeDelegate EventOnMatchLobby, EventOnMatchStart, EventOnMatchEnded;

        [SyncVar] MatchState matchState = MatchState.Lobby;

        [Command]
        public void CmdChangeMatchState(MatchState newState)
        {
            matchState = newState;
            CmdSendMatchEvent(newState);
        }

        [Command]
        public void CmdSendMatchEvent(MatchState newState)
        {
            switch (newState)
            {
                case MatchState.Lobby:
                    EventOnMatchLobby?.Invoke();
                    break;
                case MatchState.Match:
                    EventOnMatchStart?.Invoke();
                    break;
                case MatchState.Ended:
                    EventOnMatchEnded?.Invoke();
                    break;
            }
        }

        [Command]
        public void CmdStartMatch()
        {
            // Initiate a countdown
            // Initiate matchclock that calls CmdStartSetupPhase
        }
        
        [Command]
        public void CmdStartBattlePhase()
        {
            // Init a matchclock that calls the battle start
            CmdChangeMatchState(MatchState.Match);
        }

        [Command]
        public void CmdFinishMatch()
        {
            // Initiate a countdown
            // Initiate a matchclock that 
        }

        public MatchState GetMatchState()
        {
            return matchState;
        }
        
        public bool IsStarted()
        {
            if (instance.matchState == MatchState.Match)
                return true;
            return false;
        }

        public bool InLobby()
        {
            if (instance != null && instance.matchState == MatchState.Lobby) return true;
            return false;
        }

        public bool IsFinished()
        {
            if (instance != null && instance.matchState == MatchState.Ended) return true;
            return false;
        }

    }
}
