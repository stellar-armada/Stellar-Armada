using UnityEngine;

namespace SpaceCommander.Selection.Tests
{
    public class TestSelectionHandler : MonoBehaviour, ISelectable
    {
        
        private IPlayerEntity owningEntity;

        private Renderer ren;

        public int selectionSetID;

        void Awake()
        {
            ren = GetComponent<Renderer>();
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
            ren.material.SetColor("_BaseColor", Color.green);
        }

        public void Deselect()
        {
            ren.material.SetColor("_BaseColor", Color.white);
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