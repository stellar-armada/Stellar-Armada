using System;
using UnityEngine;

#pragma warning disable 0649
namespace SpaceCommander.Selection.Tests
{
    public class SelectorTest : MonoBehaviour, ISelector
    {
        private Ray cameraRay;
        private RaycastHit hit;

        [SerializeField] LayerMask layerMask;

        private bool isSelecting;
        private bool isDeselecting;

        [SerializeField] uint teamId;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SelectionManager.instance.ClearSelection();
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
            Debug.Log("Selector test needs updating no main camera?");
            return;
            cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(cameraRay, out hit, 1000, layerMask))
            {
                ICollidable collidable = hit.transform.GetComponent<ICollidable>();
                if (collidable != null)
                {
                    ISelectable selectable = collidable.GetSelectable();
                    if (!selectable.IsSelectable()) return;
                    var entityAndType = selectable.GetOwningEntity().GetEntityAndTypes();
                    bool canSelect = false;
                    
                    // Item 1 = list of entity types (ships could be player AND team, for example)
                    if (entityAndType.Item1.Contains(EntityType.TEAM))
                    {
                        canSelect = ((ITeamEntity) entityAndType.Item2).GetTeam().teamId == teamId;
                    } else if (entityAndType.Item1.Contains(EntityType.PLAYER))
                    {
                        throw new NotImplementedException(); // When we add player functionality we'll need to work this out
                    }
                    else
                    {
                        Debug.Log("Couldn't discern entity type");
                    }

                    if (!canSelect) return;
                    
                    switch (selectionType)
                    {
                        case SelectionType.Selection:
                            SelectionManager.instance.AddToSelection(selectable);
                            break;
                        case SelectionType.Deselection:
                            SelectionManager.instance.RemoveFromSelection(selectable);
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