using System.Collections.Generic;
using System.Linq;
using StellarArmada.IO;
using UnityEngine;
using StellarArmada.Player;
using StellarArmada.UI;

#pragma warning disable 0649
namespace StellarArmada.Entities.Ships
{
    public class MobileShipSelector : ShipSelector
    {

        // Keep track of the current selected object
        ISelectable selectable;
        ISelectable highlightable;

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
        
        // Handles selection and deselection of entities from the loop
        void Select(SelectionType selectionType)
        {
            // Is there a ship under our mouse?
            if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition),
                out hit, Mathf.Infinity, layerMask)) return;

            // Get the selectable on the hit object
            selectable = hit.transform.GetComponent<ISelectable>();
            
            // If the selectable is not null, is selectable, and is from this player's team
            if (selectable != null && selectable.IsSelectable())
            {
                // Check if friendly
                if(selectable.GetOwningEntity().GetTeam() == playerController.GetTeam())
                    switch (selectionType)
                    {
                        case SelectionType.Selection:
                            ShipSelectionManager.instance.AddToSelection(selectable);
                            break;
                        case SelectionType.Deselection:
                            ShipSelectionManager.instance.RemoveFromSelection(selectable);
                            break;
                    }
                // Not friendly
                else
                    Debug.Log("Enemy hit");
            }
        }
    }
}