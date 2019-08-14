using SpaceCommander.Teams;

#pragma warning disable 0649
namespace SpaceCommander
{
    public interface ITeamEntity : IEntity
    {
        void CmdSetTeam(uint teamId);
        Team GetTeam();
    }
}
