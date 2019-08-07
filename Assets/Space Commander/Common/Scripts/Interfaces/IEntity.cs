using System.Collections.Generic;
using SpaceCommander.Teams;
using UnityEngine;

namespace SpaceCommander
{
    public enum EntityType
    {
        TEAM,
        PLAYER
    }

    public interface IEntity
    {
        uint GetEntityId();
        void SetEntityId(uint id);
        (List<EntityType>, IEntity) GetEntityAndTypes();
        bool IsAlive();
        bool IsEnemy(IEntity otherEntity);
        void CmdDie();
        GameObject GetGameObject();
    }
}