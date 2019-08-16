using SpaceCommander.Ships;
using UnityEngine;

#pragma warning disable 0649
namespace SpaceCommander.Selection
{
    public class ShipSelectionHandler : MonoBehaviour, ISelectable
    {
        [SerializeField] Ship ship;

        [SerializeField] private Renderer selectionCube;

        public delegate void SelectionChangedHandler(bool selected);

        public SelectionChangedHandler OnSelectionChanged;
        
        public int selectionSetID;
        void Awake()
        {
            selectionCube.enabled = false;
            ship = GetComponent<Ship>();
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
            OnSelectionChanged?.Invoke(true);
        }

        public void Deselect()
        {
            selectionCube.enabled = false;
            OnSelectionChanged?.Invoke(false);
        }

        public IEntity GetOwningEntity()
        {
            return ship;
        }

        public bool IsSelectable()
        {
            //If is alive and has warped in
            if (ship.IsAlive() && ship.shipWarp.isWarpedIn) return true;
            return false;
        }
    }
}