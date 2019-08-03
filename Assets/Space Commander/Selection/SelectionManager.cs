using System.Collections.Generic;
using UnityEngine;

namespace SpaceCommander.Selection
{
    public class SelectionManager : MonoBehaviour
    {
        public static SelectionManager instance;
        
        private List<ISelectable> currentSelection = new List<ISelectable>();


        void Awake()
        {
            instance = this;
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