using SpaceCommander.Teams;
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
        Team GetTeam();
        uint GetId();
        string GetName();
        bool IsEnemy(IPlayer player);

    }
}
