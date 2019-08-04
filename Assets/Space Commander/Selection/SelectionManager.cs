using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SpaceCommander.Selection
{
    public class SelectionManager : MonoBehaviour
    {
        public static SelectionManager instance;

        private List<ISelectable> currentSelection = new List<ISelectable>();

        Dictionary<int, List<ISelectable>> selectionSets = new Dictionary<int, List<ISelectable>>();

        List<ISelectable> tempSelection;

        public UnityEvent SelectionChanged;
        public UnityEvent SelectionSetsChanged;

        void Awake()
        {
            instance = this;
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
            SelectionSetsChanged.Invoke();
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
                SelectionChanged.Invoke();
            }
        }

        public void SetSelection(ISelectable selectable)
        {
            bool containsNewSelectable = false;

            if (currentSelection.Count > 0)
            {
                if (currentSelection.Contains(selectable))
                {
                    containsNewSelectable = true;
                    currentSelection.Remove(selectable);
                }

                foreach (ISelectable s in currentSelection)
                {
                    s.Deselect();
                }

                currentSelection.Clear();
            }

            currentSelection.Add(selectable);
            if (!containsNewSelectable)
            {
                selectable.Select();
                SelectionChanged.Invoke();
            }
        }

        public void AddToSelection(ISelectable selectable)
        {
            if (!currentSelection.Contains(selectable))
            {
                Debug.Log("Added " + selectable.GetOwningEntity() + " to selection");
                currentSelection.Add(selectable);
                selectable.Select();
                SelectionChanged.Invoke();
            }
        }

        public void ClearSelection()
        {
            foreach (ISelectable selectable in currentSelection)
            {
                selectable.Deselect();
            }

            currentSelection.Clear();
            SelectionChanged.Invoke();
        }

        public void RemoveFromSelection(ISelectable selectable)
        {
            if (currentSelection.Contains(selectable))
            {
                Debug.Log("Removed " + selectable.GetOwningEntity() + " from selection");
                currentSelection.Remove(selectable);
                selectable.Deselect();
                SelectionChanged.Invoke();
            }
        }
    }
}