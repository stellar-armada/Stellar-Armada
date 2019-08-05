using UnityEngine;

namespace SpaceCommander
{
    public interface IEntity
    {
        uint GetEntityId();
        bool IsAlive();
        void CmdDie();
        GameObject GetGameObject();
    }
}
