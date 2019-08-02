using SpaceCommander.Game;

namespace SpaceCommander
{
    public interface IDamager
    {
        IPlayer GetPlayer();
        void SetPlayer(IPlayer player);
    }
}