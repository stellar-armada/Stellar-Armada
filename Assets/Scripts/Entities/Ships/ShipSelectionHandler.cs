using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Entities.Ships
{
    public class ShipSelectionHandler : MonoBehaviour, ISelectable
    {
        [SerializeField] NetworkEntity entity;

        [SerializeField] private Renderer selectionCube;

        public delegate void SelectionChangedHandler(bool selected);

        public SelectionChangedHandler OnSelectionChanged;
        private bool canSelect = true;
        
        void Awake()
        {
            selectionCube.enabled = false;
            entity.OnEntityDead += HandleDeath;
        }

        void HandleDeath()
        {
            Deselect();
            selectionCube.enabled = false;
        }

        public void Select()
        {
            selectionCube.enabled = true;
            OnSelectionChanged?.Invoke(true);
        }

        public void Deselect()
        {
            selectionCube.enabled = false;
            OnSelectionChanged?.Invoke(false);
        }

        public NetworkEntity GetOwningEntity()
        {
            return entity;
        }

        public bool IsSelectable()
        {
            //If is alive and has warped inentity.shipWarp.isWarpedIn
            if (entity.IsAlive()) return true;
            return false;
        }
    }
}