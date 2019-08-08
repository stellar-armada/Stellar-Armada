using SpaceCommander.Ships;
using UnityEngine;

namespace SpaceCommander.Selection
{
    public class ShipSelectionHandler : MonoBehaviour, ISelectable
    {
        [SerializeField] Ship ship;

        [SerializeField] private Renderer selectionCube;
        
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
        }

        public void Deselect()
        {
            selectionCube.enabled = false;
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