using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpaceCommander.Ships;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SpaceCommander.Selection
{
    public class SelectionManager : MonoBehaviour
    {
        public static SelectionManager instance;

        private List<ISelectable> currentSelection = new List<ISelectable>();

        Dictionary<int, List<ISelectable>> selectionSets = new Dictionary<int, List<ISelectable>>();

        List<ISelectable> tempSelection;
        
        public GameObject uiShipPrefab;
        
        [SerializeField] private Transform[] selectionSetContainers;

        void Awake()
        {
            instance = this;
        }

        void UpdateSelectionSets(int selectionSet)
        {
            Transform[] children = selectionSetContainers[selectionSet].GetComponentsInChildren<Transform>();
            if (children.Length > 0)
            {
                foreach (Transform t in children)
                {
                    if (t == selectionSetContainers[selectionSet]) continue;
                    Destroy(t.gameObject);
                }
            }
                
                
                
                List<ISelectable> selection = GetSelectionSet(selectionSet);
                if (selection == null) return;
                foreach (ISelectable selectable in selection)
                {
                    GameObject uiShip = Instantiate(uiShipPrefab, selectionSetContainers[selectionSet]);// TO-DO -- refactor this to use pooling
                    // Set the ship (so it can sample hull and shields)
                    Ship s = (Ship)selectable.GetOwningEntity();
                    
                }
            }

        public List<ISelectable> GetCurrentSelection()
        {
            return currentSelection;
        }

        public List<ISelectable> GetSelectionSet(int selectionSetId)
        {
            if (selectionSets.ContainsKey(selectionSetId))
            {
                return selectionSets[selectionSetId];
            }

            Debug.Log("Selection is null");
            return null;
        }

        public void SelectAll(int teamId)
        {
            var ships = ShipManager.GetShips().Where(s => s.GetTeam().teamId == (uint)teamId);
            foreach (var ship in ships)
            {
                AddToSelection(ship.selectionHandler);
            }
        }
        
        public void CreateOrReplaceSelectionSet(int selectionSetId)
        {
            if (selectionSets.ContainsKey(selectionSetId))
            {
                selectionSets.Remove(selectionSetId);
            }

            List<ISelectable> newSelection = new List<ISelectable>();
            foreach(ISelectable selectable in currentSelection)
            {
                newSelection.Add(selectable);
            }
            Debug.Log("Added a selection with " + newSelection.Count + " members");
            selectionSets.Add(selectionSetId, newSelection);
            UpdateSelectionSets(selectionSetId);
        }

        public void RecallSelectionSet(int selectionSetId)
        {
            Debug.Log("Selection set id: " + selectionSetId);
            
            if (selectionSets.ContainsKey(selectionSetId))
            {

                List<ISelectable> selectionSet = selectionSets[selectionSetId];
                
                // For any object that is in current selection that isn't in selection set, Deselect
                foreach(ISelectable selectable in currentSelection)
                {
                    if (!selectionSet.Contains(selectable))
                    {
                        selectable.Deselect();
                    }
                }

                // For any object that is in the new selection that isn't in the current selection, Select
                // For any object that is in current selection that isn't in selection set, Deselect
                foreach(ISelectable selectable in selectionSet)
                {
                    if (!currentSelection.Contains(selectable))
                    {
                        selectable.Select();
                    }
                }
                
                
                currentSelection = selectionSets[selectionSetId];
            }
        }

        public void SetSelectionFromGroup(List<ISelectable> selectables)
        {

            foreach (var selectable in currentSelection)
            {
                if (!selectables.Contains(selectable)) selectable.Deselect();
            }
            
            foreach (ISelectable selectable in selectables)
            {
                if(!currentSelection.Contains(selectable)) selectable.Select();
            }

            currentSelection = selectables;
        }



        public void SetSelection(ISelectable selectable)
        {
            bool containsNewSelectable = false;

            if (currentSelection.Count > 0)
            {
                if (currentSelection.Contains(selectable))
                {
                    containsNewSelectable = true;
                    // Remove the selectable from the selection for a second
                    currentSelection.Remove(selectable);
                }

                foreach (ISelectable s in currentSelection)
                {
                    s.Deselect();
                }

                currentSelection.Clear();
            }
            // Add the selectable back to the selection
            currentSelection.Add(selectable);
            if (!containsNewSelectable)
            {
                
                selectable.Select();
                
            }
        }

        public void AddToSelection(ISelectable selectable)
        {
            if (!currentSelection.Contains(selectable))
            {
                Debug.Log("Added " + selectable.GetOwningEntity() + " to selection");
                currentSelection.Add(selectable);
                selectable.Select();
            }
        }

        public void ClearSelection()
        {
            foreach (ISelectable selectable in currentSelection)
            {
                selectable.Deselect();
            }

            currentSelection.Clear();
        }

        public void RemoveFromSelection(ISelectable selectable)
        {
            if (currentSelection.Contains(selectable))
            {
                Debug.Log("Removed " + selectable.GetOwningEntity() + " from selection");
                currentSelection.Remove(selectable);
                selectable.Deselect();
            }
        }
    }
}