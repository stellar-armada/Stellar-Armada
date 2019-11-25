using UnityEngine;
using UnityEngine.Serialization;

#pragma warning disable 0649
namespace StellarArmada.Ships
{
    public class ShipSelectionHandler : MonoBehaviour, ISelectable
    {
        [FormerlySerializedAs("entity")] [SerializeField] Ship ship;

        [SerializeField] private Renderer selectionCube;

        public delegate void SelectionChangedHandler(bool selected);

        public SelectionChangedHandler OnSelectionChanged;
        private bool canSelect = true;
        
        void Awake()
        {
            selectionCube.enabled = false;
            ship.OnEntityDead += HandleDeath;
        }

        void HandleDeath()
        {
            Deselect();
            selectionCube.enabled = false;
        }

        public bool isSelected = false;

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
            if (!isHighlighted)
            {
                selectionCube.enabled = false;
            }
            isSelected = false;
            OnSelectionChanged?.Invoke(false);
        }

        public Ship GetShip()
        {
            return ship;
        }

        public bool IsSelectable()
        {
            //If is alive and has warped inentity.shipWarp.isWarpedIn
            if (ship.IsAlive()) return true;
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
            if (isSelected)
            {
                selectionCube.enabled = true;
                selectionCube.material.SetColor("_BaseColor", ColorManager.instance.selectedColor);
            }
            else selectionCube.enabled = false;
        }
    }
}