namespace SpaceCommander
{
    public interface IPlayerEntity
    {
        void CmdSetPlayer(uint playerId);
        uint GetEntityId();
        IPlayer GetPlayer();

    }
}
