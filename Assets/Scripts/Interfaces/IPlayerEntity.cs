#pragma warning disable 0649
namespace SpaceCommander
{
    public interface IPlayerEntity : IEntity
    {
        void CmdSetPlayer(uint playerId);
        IPlayer GetPlayer();
    }
}
