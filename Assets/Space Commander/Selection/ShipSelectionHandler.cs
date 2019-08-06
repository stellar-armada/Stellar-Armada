using UnityEngine;

namespace SpaceCommander.Selection
{
    public class ShipSelectionHandler : MonoBehaviour, ISelectable
    {
        private IPlayerEntity owningEntity;

        [SerializeField] private Renderer selectionCube;
        
        public int selectionSetID;

        void Awake()
        {
            selectionCube.enabled = false;
            owningEntity = GetComponent<IPlayerEntity>();
        }

        public int GetSelectionSetID()
        {
            return selectionSetID;
        }

        public void SetSelectionSetID(int id)
        {
            selectionSetID = id;
        }

        public void Select()
        {
            selectionCube.enabled = true;
        }

        public void Deselect()
        {
            selectionCube.enabled = false;
        }

        public IPlayerEntity GetOwningEntity()
        {
            return owningEntity;
        }

        public bool IsSelectable()
        {
            return true;
        }
    }
}