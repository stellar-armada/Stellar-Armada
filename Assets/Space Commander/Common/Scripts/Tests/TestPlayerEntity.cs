using SpaceCommander.Game;
using UnityEngine;

namespace SpaceCommander.Common.Tests
{
    public class TestPlayerEntity : MonoBehaviour, IPlayerEntity
    {
        private IPlayer testPlayer;

        public bool isAlive;

        private uint entityId = 0;

        public void SetPlayer(IPlayer player)
        {
            testPlayer = player;
        }

        public void CmdSetPlayer(uint playerId)
        {
            testPlayer = PlayerManager.GetPlayerByNetId(playerId);
        }

        public uint GetEntityId()
        {
            return entityId;
        }

        public void SetEntityId(uint id)
        {
            entityId = id;
        }

        public IPlayer GetPlayer()
        {
            return testPlayer;
        }

        public bool IsAlive()
        {
            return isAlive;
        }

        public void CmdDie()
        {
            isAlive = false;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }
    }
}