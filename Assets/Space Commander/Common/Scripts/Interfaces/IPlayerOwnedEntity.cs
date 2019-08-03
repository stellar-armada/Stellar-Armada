namespace SpaceCommander
{
    public interface IPlayerOwnedEntity
    {
        void CmdSetPlayer(uint playerID);
        uint GetId();
        IPlayer GetPlayer();

    }
}
