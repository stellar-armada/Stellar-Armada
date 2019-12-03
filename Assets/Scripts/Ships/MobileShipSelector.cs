using StellarArmada.Player;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Ships
{
    public class MobileShipSelector : ShipSelector
    {

        // Keep track of the current selected object
        ISelectable selectable;
        ISelectable highlightable;

        public string altButton = "Alt";

        private bool altButtonIsDown;

        private Camera cam;

        protected override void Awake()
        {
            base.Awake();
            cam = Camera.main;
        } 

        // Set whenever a raycast hits a ship
        RaycastHit hit;

        void Update()
        {
            // Debug -- add mobile controls -- select on mouse down
            if (Input.GetMouseButtonDown(0))
            {
                // Select ship
                Select(SelectionType.Selection);
            }
            // Select on the right mouse button down
            else if (Input.GetMouseButtonDown(1))
            {
                // Deselect ship
                Select(SelectionType.Deselection);
            }
        }

        private float doubleClickTime = .3f;

        private float lastClicktime = 0;

        private ISelectable lastSelected;
        
        // Handles selection and deselection of entities from the loop
        void Select(SelectionType selectionType)
        {
            // Is there a ship under our mouse?
            if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition),
                out hit, Mathf.Infinity, layerMask))
                return;

            // Get the selectable on the hit object
            selectable = hit.transform.GetComponent<ISelectable>();
            
            // If the selectable is not null, is selectable, and is from this player's team
            if (selectable != null && selectable.IsSelectable())
            {

                if (selectable.GetShip().shipWarp.isWarping) return;
                
                // Timestamp click
                float currentClicktime = Time.time;
                
                // If this click - lastclick < doubleClickTime
                if (currentClicktime - lastClicktime < doubleClickTime && lastSelected == selectable)
                {
                    // Deselect all ships and select this one and return
                    ShipSelectionManager.instance.SetSelection(selectable);
                }
                else
                    lastClicktime = currentClicktime;

                lastSelected = selectable;
                
                // Check if friendly
                if (selectable.GetShip().GetTeam() == PlayerController.localPlayer.GetTeam())
                {
                    switch (selectionType)
                    {
                        case SelectionType.Selection:
                            ShipSelectionManager.instance.AddToSelection(selectable);
                            break;
                        case SelectionType.Deselection:
                            ShipSelectionManager.instance.RemoveFromSelection(selectable);
                            break;
                    }
                }
                // Not friendly
                else
                    Debug.Log("Enemy hit");
            }
        }
    }
}