using System.Collections.Generic;
using SpaceCommander.Teams;
using UnityEngine;

#pragma warning disable 0649
namespace SpaceCommander
{
    public interface IEntity
    {
        uint GetEntityId();
        void SetEntityId(uint id);
        bool IsAlive();
        bool IsEnemy(IEntity otherEntity);
        void CmdDie();
        GameObject GetGameObject();
        ISelectable GetSelectionHandler();
        void CmdSetTeam(uint teamId);
        Team GetTeam();
    }
}