namespace SpaceCommander
{
    public interface IPlayerEntity
    {
        void CmdSetPlayer(uint playerID);
        uint GetId();
        IPlayer GetPlayer();

    }
}
