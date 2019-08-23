using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Selection.Tests
{
    public class TestSelectionHandler : MonoBehaviour, ISelectable
    {
        
        private NetworkEntity _owningNetworkEntity;

        private Renderer ren;

        public int selectionSetID;

        void Awake()
        {
            ren = GetComponent<Renderer>();
            _owningNetworkEntity = GetComponent<NetworkEntity>();
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

        public NetworkEntity GetOwningEntity()
        {
            return _owningNetworkEntity;
        }

        public bool IsSelectable()
        {
            return true;
        }
    }
}