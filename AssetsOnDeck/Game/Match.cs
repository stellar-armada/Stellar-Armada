using UnityEngine;
using Mirror;
using SpaceCommander.Game.GameModes;
using SpaceCommander.Player;
using SpaceCommander.UI;

namespace SpaceCommander.Game
{
    public enum MatchState
    {
        Lobby,
        Started,
        Paused,
        Finished
    }

    public class Match : NetworkBehaviour
    {
        static Match currentMatch; // private singleton with public GetCurrentMatch accessor
        [SyncVar] public string levelName;
        public MatchClock clock;
        public GameMode gameMode;
        [SyncVar] private float matchLength;

        public bool environmentDamagesPlayers = false;

        [SyncVar(hook = nameof(HandleMatchStateChange))]
        public MatchState matchState;

        [SyncVar] public bool autoStartNextMatch = true;

        public static GameManager.EventHandler OnMatchInitialized;
        public static event GameManager.EventHandler OnMatchLobby, OnMatchStarted, OnMatchPaused, OnMatchFinished;
        [SyncVar] private float CurrentTime;

        private void HandleMatchStateChange(MatchState _newState)
        {
            Debug.Log("<color=red>*** HANDLE PAUSE LOGIC HERE</color>");
          //      Debug.Log("State changed to: " + _newState);
            // if (matchState == MatchState.Paused && _newState == MatchState.Started)
            //  if (isLocalPlayer)
            //    HudMessageManager.instance.RaiseMessage(MessageType.MATCH_RESUMING);
            switch (_newState)
            {
                case MatchState.Lobby:
                    OnMatchLobby?.Invoke();
                    break;
                case MatchState.Started:
                    OnMatchStarted?.Invoke();
                    break;
                case MatchState.Paused:
                    HudMessageManager.instance.RaiseMessage(MessageType.MATCH_PAUSED);
                    OnMatchPaused?.Invoke();
                    if (isLocalPlayer)
                        HudMessageManager.instance.RaiseMessage(MessageType.MATCH_PAUSED);
                    break;
                case MatchState.Finished:
                    OnMatchFinished?.Invoke();
                    break;
            }
        }
        
        public static bool IsStarted()
        {
            if (currentMatch != null && currentMatch.matchState == MatchState.Started)
                return true;
            else
                return false;
        }
        
        public static bool InLobby()
        {
            if (currentMatch != null && currentMatch.matchState == MatchState.Lobby) return true;
            return false;
        }
        
        public static bool IsFinished()
        {
            if (currentMatch != null && currentMatch.matchState == MatchState.Finished) return true;
            return false;
        }
        
        public static bool IsPaused()
        {
            if (currentMatch != null && currentMatch.matchState == MatchState.Paused) return true;
            return false;
        }

        public static void ClearDelegates()
        {
            OnMatchInitialized = OnMatchLobby = OnMatchStarted = OnMatchPaused = OnMatchFinished = null;
        }

        private void Awake()
        {

            if(isServer) InitLocalMatchWithStoredSettings();
            
            if (currentMatch != null && currentMatch != this)
            {
                Destroy(currentMatch.gameObject);
            }

            currentMatch = this;
            
            OnMatchInitialized?.Invoke();
        }

        public static Match GetCurrentMatch()
        {
            return currentMatch;
        }

        public void StartMatch()
        {
            SetState(MatchState.Started);
            Debug.Log("Starting Match!");
            clock.Start();
        }

        public void Pause()
        {
            GetCurrentMatch().SetState(MatchState.Paused);
        }

        public void SetState(MatchState _newState)
        {
            if (isServer)
                matchState = _newState;
        }

        public void Resume()
        {
            GetCurrentMatch().SetState(MatchState.Started);
        }

        public void EndMatch()
        {
            if (isServer)
            {
                GetCurrentMatch().SetState(MatchState.Finished);
            }
            GameManager.instance.EventOnMatchEnded?.Invoke();
        }

        public void ResetMatch()
        {
            GameManager.instance.CmdCreateNewMatch();
            clock = new MatchClock(this, matchLength);
            HudMessageManager.instance.RaiseMessage(MessageType.MATCH_RESTARTED);
            ScoreboardController.instance.GetScoreboardPanel().SetActive(true);
            ScoreboardController.instance.PopulateScoreboard();
            GetCurrentMatch().RpcHandleMatchRestarted();
            PlayerCanvasController.instance.HideCanvas();
        }

        [ClientRpc] // Callback for clients to update their UI
        void RpcHandleMatchRestarted() // Private, but since this is called from ResetMatch lets keep it here
        {
            if (isLocalPlayer && !isServer)
            {
                HudMessageManager.instance.RaiseMessage(MessageType.MATCH_RESTARTED);
                ScoreboardController.instance.PopulateScoreboard();
            }
        }

        public void InitLocalMatchWithStoredSettings() // Server calls this
        {
            // Initializes the match using stored settings-- settings are stored whenever changing host options so we can give the player the options for game as they last used 
            InitMatch(SettingsManager.GetStoredHostGameMode(), SettingsManager.GetMatchLength(), SettingsManager.GetStoredHostLevel());
        }

        private void Update()
        {
            if (isServer)
            {
                clock.Update();
                CurrentTime = clock.GetMatchTime();
            }
        }

        void InitMatch(GameModeType newGameMode, float newMatchLength, string newLevel)
        {
            gameMode = new FreePlay();
            if (isServer)
                foreach (IPlayer player in PlayerManager.GetPlayers())
                {
                    gameMode.AssignTeam(player);
                }

            levelName = newLevel;

            SetState(MatchState.Lobby);
            matchLength = newMatchLength;
            clock = new MatchClock(this, newMatchLength);
            
            NetworkServer.Spawn(gameObject);

            GameManager.instance.EventOnNewMatchCreated?.Invoke();
        }
        

        public void AddPlayerToTeam(IPlayer player)
        {
            if (isServer)
                gameMode.AssignTeam(player);
        }

        public void RaiseMessageToClients(MessageType _t)
        {
            if (!isServer) return;
                HudMessageManager.instance.RaiseMessage(_t);
                RpcRaiseMessageToClients(_t);
        }

        
        [ClientRpc]
        public void RpcRaiseMessageToClients(MessageType _t)
        {
            if(!isServer)
            HudMessageManager.instance.RaiseMessage(_t);
        }
    }
}