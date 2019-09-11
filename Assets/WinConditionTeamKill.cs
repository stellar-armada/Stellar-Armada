using StellarArmada.Match;
using StellarArmada.Player;
using StellarArmada.Teams;
using UnityEngine;

public class WinConditionTeamKill : WinCondition
{
    private bool winConditionMet = false;
    
    public override void SetupWinCondition()
    {
        foreach (PlayerController pc in PlayerManager.players)
        {
            pc.OnPlayerControllerDeath += CheckWinCondition;
        }

        PlayerManager.instance.OnPlayerRegistered += RegisterWinCondition;
    }

    void RegisterWinCondition(PlayerController playerController)
    {
        playerController.OnPlayerControllerDeath += CheckWinCondition;
    }

    void CheckWinCondition(PlayerController playerController)
    {
        if (winConditionMet) return;
        
        Team team = playerController.GetTeam();

        winConditionMet = false;
        
        foreach (PlayerController p in team.players)
        {
            if (p.isAlive) // win condition failed automatically, so set to false and return
            {
                winConditionMet = false;
                return;
            }
                winConditionMet = true;
        }

        if (winConditionMet) HandleWinConditionMet();

    }

    void HandleWinConditionMet()
    {
        
        foreach (Team team in TeamManager.instance.teams)
        {
            bool teamWins = false;
            
            // If anyone is alive on the team, they win!
            foreach (PlayerController p in team.players)
            {
                if (p.isAlive)
                {
                    teamWins = true;
                    return;
                }
                teamWins = false;
            }

            if (teamWins)
            {
                foreach (PlayerController p in team.players)
                {
                    p.HandleWin();
                }
            }
            else
            {
                foreach (PlayerController p in team.players)
                {
                    p.HandleLoss();
                }
            }

        }
    }
}
