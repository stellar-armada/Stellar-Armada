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

        private bool isHighlighted;
        
        public void Select()
        {
            selectionCube.enabled = true;
            selectionCube.material.SetColor("_BaseColor", ColorManager.instance.selectedColor);
            isSelected = true;
            OnSelectionChanged?.Invoke(true);
        }

        public void Deselect()
        {
            if(!isHighlighted) selectionCube.enabled = false;
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
            if (entity.IsAlive()) return true;
            Debug.Log("Entity is not selectable because entity is not alive");
            return false;
        }

        public void Highlight(Color highlightColor)
        {
            selectionCube.enabled = true;
            selectionCube.material.SetColor("_BaseColor", highlightColor);
            isHighlighted = true;
        }

        public void Unhighlight()
        {
            isHighlighted = false;
            if (isSelected) selectionCube.enabled = true;
            else selectionCube.enabled = false;
        }
    }
}