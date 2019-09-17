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

        private bool isSelected = false;
        
        public void Select()
        {
            selectionCube.enabled = true;
            isSelected = true;
            OnSelectionChanged?.Invoke(true);
        }

        public void Deselect()
        {
            selectionCube.enabled = false;
            isSelected = false;
            OnSelectionChanged?.Invoke(false);
        }

        public NetworkEntity GetOwningEntity()
        {
            return entity;
        }

        public bool IsSelectable()
        {
            //If is alive and has warped inentity.shipWarp.isWarpedIn
            if (canSelect && entity.IsAlive()) return true;
            return false;
        }

        public void Highlight(Color highlightColor)
        {
            selectionCube.enabled = true;
            selectionCube.material.color = highlightColor;
        }

        public void Unhighlight()
        {
            selectionCube.material.color = ColorManager.instance.selectedColor;
            if(!isSelected) selectionCube.enabled = false;
        }
    }
}