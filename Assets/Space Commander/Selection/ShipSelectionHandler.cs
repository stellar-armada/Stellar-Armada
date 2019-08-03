using UnityEngine;

namespace SpaceCommander.Selection
{
    public class ShipSelectionHandler : MonoBehaviour, ISelectable
    {
        private IPlayerEntity owningEntity;

        public int GetSelectionSetID()
        {
            throw new System.NotImplementedException();
        }

        public void SetSelectionSetID(int id)
        {
            throw new System.NotImplementedException();
        }

        public void Select()
        {
            throw new System.NotImplementedException();
        }

        public void Deselect()
        {
            throw new System.NotImplementedException();
        }

        public IPlayerEntity GetOwningEntity()
        {
            return owningEntity;
        }

        public bool IsSelectable()
        {
            throw new System.NotImplementedException();
        }
    }
}