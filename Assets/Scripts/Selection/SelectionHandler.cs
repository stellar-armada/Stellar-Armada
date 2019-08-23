using StellarArmada.Ships;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Selection
{
    public class SelectionHandler : MonoBehaviour, ISelectable
    {
        [SerializeField] NetworkEntity entity;

        [SerializeField] private Renderer selectionCube;

        public delegate void SelectionChangedHandler(bool selected);

        public SelectionChangedHandler OnSelectionChanged;
        
        void Awake()
        {
            selectionCube.enabled = false;
        }

        public void Select()
        {
            Debug.Log("Selected!");
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
            Debug.Log("Checking if is selectable: " + (entity.IsAlive() && entity.movement.controlEnabled));
            //If is alive and has warped inentity.shipWarp.isWarpedIn
            if (entity.IsAlive()) return true;
            return false;
        }
    }
}