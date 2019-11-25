using System.Linq;
using StellarArmada.Match;
using StellarArmada.Player;
using StellarArmada.Teams;

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
        MatchStateManager.instance.CmdChangeMatchState(MatchState.Ended);
        
        foreach (Team team in TeamManager.instance.teams)
        {
            bool teamWins = (team.players.Where(p => p.isAlive).Count() > 0);
            
            if (teamWins)
            {
                foreach (PlayerController p in team.players)
                    p.HandleWin();
            }
            else
            {
                foreach (PlayerController p in team.players)
                    p.HandleLoss();
            }

        }
    }
}
