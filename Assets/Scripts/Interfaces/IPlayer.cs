using UnityEngine;
#pragma warning disable 0649
namespace SpaceCommander
{
    public interface IPlayer
    {
        bool IsLocalPlayer();
        bool IsServer();
        bool IsClient();
        PlayerType GetPlayerType();
        GameObject GetGameObject();
        uint GetId();
        string GetName();
        bool IsEnemy(IPlayer player);
        uint GetTeamId();
        void SetTeamId(uint teamId);
    }
}
