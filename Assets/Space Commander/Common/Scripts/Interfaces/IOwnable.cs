namespace SpaceCommander
{
    public interface IOwnable
    {
        void CmdSetPlayer(uint playerID);
        uint GetId();
        IPlayer GetPlayer();

    }
}
