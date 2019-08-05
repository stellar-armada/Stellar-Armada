using UnityEngine;

namespace SpaceCommander
{
    public interface IPlayerEntity : IEntity
    {
        void CmdSetPlayer(uint playerId);
        IPlayer GetPlayer();
    }
}
