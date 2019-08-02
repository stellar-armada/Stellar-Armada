using System.Collections.Generic;
using SpaceCommander.Player;
using UnityEngine;

namespace SpaceCommander.Game.GameModes
{
    public enum GameModeType
    {
        BATTLE_ROYALE,
        FREE_PLAY
    }

    public abstract class GameMode
    {
        public string Name;
        public string Description;

        public static GameMode CreateGameMode(GameModeType _type)
        {
            switch (_type)
            {
                case GameModeType.BATTLE_ROYALE:
                    return new BattleRoyale();
                case GameModeType.FREE_PLAY:
                    return new FreePlay();
                default:
                    Debug.LogError(_type + " not implemented yet!");
                    return null;
            }
        }

        public abstract void AssignTeam(IPlayer _player);

        public delegate List<IPlayer> GameModeWinEvent();

        public List<Team> Teams = new List<Team>();

        public GameModeWinEvent OnGameModeWin;

        public Dictionary<Team, int> TeamScores;

        public GameObject GameModeLevelPrefab;

        public abstract void AddScore(int amount, Team _team);

        public abstract void AddToTeam(IPlayer _player, Team _team);

        public abstract void AddToTeam(IPlayer _player);
    }
}