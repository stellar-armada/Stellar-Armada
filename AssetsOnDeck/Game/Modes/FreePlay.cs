using SpaceCommander.Teams;

namespace SpaceCommander.Game.GameModes
{
    public class FreePlay : GameMode
    {
        private int teamInd = 0;
        public override void AssignTeam(IPlayer _player)
        {
            /*
            var emptyTeam = Teams.Find(_e => _e.GetMembers().Count == 0);
            if (emptyTeam != null)
            {
                emptyTeam.AddMember(_player);
                _player.CmdSetTeam(teamInd);
                return;
            }
            
            while(teamInd >= Teams.Count)
                Teams.Add(new Team());
            
            Teams[teamInd].AddMember(_player);
            _player.CmdSetTeam(teamInd);
            
            teamInd++;
            Debug.Log("teamInd increased to " + teamInd);
            */
        }

        public override void AddScore(int amount, Team _team)
        {
        }

        public override void AddToTeam(IPlayer _player, Team _team)
        {
            
        }

        public override void AddToTeam(IPlayer _player)
        {
        }
    }
}