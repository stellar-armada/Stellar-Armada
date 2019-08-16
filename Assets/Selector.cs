using System;
using SpaceCommander.UI;
using UnityEngine;

#pragma warning disable 0649
namespace SpaceCommander.Selection
{
    public class Selector : MonoBehaviour, ISelector
    {
        [SerializeField] Transform selectorStartPoint;

        public float selectorRadius;
        
        [SerializeField] LayerMask layerMask;

        private bool isSelecting;
        private bool isDeselecting;

        [SerializeField] private PlayerController playerController;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SelectionUIManager.instance.ClearSelection();
            }
            if (Input.GetMouseButtonDown(0))
            {
                StartSelection();
            }

            else if (Input.GetMouseButtonUp(0))
            {
                EndSelection();
            }
            
            if (Input.GetMouseButtonDown(1))
            {
                StartDeselection();
            }

            else if (Input.GetMouseButtonUp(1))
            {
                EndDeselection();
            }

            if (isSelecting)
            {
                Select(SelectionType.Selection);
            } else if (isDeselecting)
            {
                Select(SelectionType.Deselection);
            }
        }

        public void Select(SelectionType selectionType)
        {            
            Collider[] hitColliders = Physics.OverlapSphere(selectorStartPoint.position, selectorRadius, layerMask);
            foreach(Collider collider in hitColliders)
            {
                ICollidable collidable = collider.GetComponent<ICollidable>();
                if (collidable != null)
                {
                    ISelectable selectable = collidable.GetSelectable();
                    if (!selectable.IsSelectable()) return;
                    if (selectable.GetOwningEntity().GetTeam().teamId != playerController.teamId) return;
                    
                    switch (selectionType)
                    {
                        case SelectionType.Selection:
                            SelectionUIManager.instance.AddToSelection(selectable);
                            break;
                        case SelectionType.Deselection:
                            SelectionUIManager.instance.RemoveFromSelection(selectable);
                            break;
                    }
                }
            } 
        }

        public void StartSelection()
        {
            isSelecting = true;
        }

        public void EndSelection()
        {
            isSelecting = false;
        }

        public void StartDeselection()
        {
            isDeselecting = true;
        }

        public void EndDeselection()
        {
            isDeselecting = false;
        }
    }
}