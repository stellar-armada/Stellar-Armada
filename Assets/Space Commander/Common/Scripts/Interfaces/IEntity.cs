using UnityEngine;

namespace SpaceCommander
{
    public interface IEntity
    {
        uint GetEntityId();
        void SetEntityId(uint id);
        bool IsAlive();
        void CmdDie();
        GameObject GetGameObject();
    }
}
