using System.Collections.Generic;
using SpaceCommander.Game;
using UnityEngine;

namespace SpaceCommander.Common.Tests
{
    public class TestPlayerEntity : MonoBehaviour, IPlayerEntity
    {
        private IPlayer testPlayer;

        public bool isAlive;

        private uint entityId = 0;

        private List<EntityType> entityTypes = new List<EntityType> {EntityType.TEAM};

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
        
        public (List<EntityType>, IEntity) GetEntityAndTypes()
        {
            return (entityTypes, this);
        }

        public IPlayer GetPlayer()
        {
            return testPlayer;
        }

        public bool IsAlive()
        {
            return isAlive;
        }
        
// This only tests player ships, not team ships
        public bool IsEnemy(IEntity otherEntity)
        {
            var entityKey = otherEntity.GetEntityAndTypes();
            var entityTypes = entityKey.Item1;
            if (entityTypes.Contains(EntityType.PLAYER))
            {
                if (((IPlayerEntity)otherEntity).GetPlayer() != GetPlayer()) return true; // For now, players will all shoot each other FFA if they are controller "player ships"
            }

            return false;
        }

        public void CmdDie()
        {
            isAlive = false;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public ISelectable GetSelectionHandler()
        {
            throw new System.NotImplementedException();
        }
    }
}