using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SpaceCommander.Selection
{
    public class SelectionManager : MonoBehaviour
    {
        public static SelectionManager instance;
        
        private List<ISelectable> currentSelection = new List<ISelectable>();
        
        Dictionary<int, List<ISelectable>> selectionLists = new Dictionary<int, List<ISelectable>>();
        
        List<ISelectable> tempSelection;

        public UnityEvent SelectionChanged;
        public UnityEvent SelectionSetsChanged;

        void Awake()
        {
            instance = this;
        }

        bool SelectionSetIsEmpty(int selectionSetId)
        {
            
            if (selectionLists.TryGetValue(selectionSetId, out tempSelection))
            {
                if (tempSelection.Count > 0)
                {
                    return false;
                }
                return true;
            }
            return true;
        }
        
        public void CreateOrReplaceSelectionSet(int selectionSetId)
        {
            if (!SelectionSetIsEmpty(selectionSetId))
            {
                selectionLists.Remove(selectionSetId);
            }
            selectionLists.Add(selectionSetId, currentSelection);
            SelectionSetsChanged.Invoke();
        }

        public void RecallSelectionSet(int selectionSetId)
        {
            if (selectionLists.TryGetValue(selectionSetId, out tempSelection))
            {
                currentSelection = tempSelection;
                SelectionSetsChanged.Invoke();
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
                SelectionChanged.Invoke();
                selectable.Deselect();
            }
        }
    }
}