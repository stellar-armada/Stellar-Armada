using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Match
{
    // Manage the current state of the match
    // TO-DO: Integrate!
    
    public enum MatchState
    {
        Lobby,
        Match,
        Ended
    }

    public class MatchStateManager : NetworkBehaviour
    {
        public static MatchStateManager instance; // private singleton with public GetCurrentMatch accessor

        [SerializeField] private MatchServerManager matchServerManager;
        
        List<WinCondition> winConditions;

        public void Initialize() // Called by the match server manager when ready
        {
            List<WinCondition> newWinConditions = new List<WinCondition>();
                
            // Add win conditions from the scenario to the match state manager and initialize them
                
            foreach (var w in matchServerManager.GetCurrentScenario().WinConditions)
                newWinConditions.Add(gameObject.AddComponent(WinConditionManager.instance.winConditionDictionary[w].GetType()) as WinCondition);

            winConditions = newWinConditions;
            foreach (var winCondition in winConditions)
            {
                winCondition.SetupWinCondition();
            }
        }
        
        
        

        // ** CODE BELOW THIS POINT HAS NOT BEEN INTEGRATED
        
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
