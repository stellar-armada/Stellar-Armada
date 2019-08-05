using SpaceCommander.Teams;

namespace SpaceCommander
{
    public interface ITeamEntity : IEntity
    {
        void CmdSetTeam(uint teamId);
        Team GetTeam();
    }
}
