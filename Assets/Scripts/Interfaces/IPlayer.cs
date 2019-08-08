using UnityEngine;
namespace SpaceCommander
{
    public interface IPlayer
    {
        bool IsLocalPlayer();
        bool IsServer();
        bool IsClient();
        void RegisterPlayer();
        void UnregisterPlayer();
        PlayerType GetPlayerType();
        GameObject GetGameObject();
        uint GetId();
        string GetName();
        bool IsEnemy(IPlayer player);
    }
}
