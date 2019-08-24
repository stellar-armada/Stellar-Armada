using System.Collections.Generic;
using System.Linq;
using StellarArmada.Ships;
using StellarArmada.Teams;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.UI
{
    public class SelectionUIManager : MonoBehaviour
    {
        public static SelectionUIManager instance;

        [SerializeField] private PlayerController playerController;
        private List<ISelectable> currentSelection = new List<ISelectable>();

        List<List<ISelectable>> selectionSets = new List<List<ISelectable>>()
        {
            new List<ISelectable>(),
            new List<ISelectable>(),
            new List<ISelectable>(),
            new List<ISelectable>(),
        };

        List<ISelectable> tempSelection;
        
        [SerializeField] private Transform[] selectionSetContainers;

        private List<List<UISelectionShip>> uiShips = new List<List<UISelectionShip>>()
        {
            new List<UISelectionShip>(),
            new List<UISelectionShip>(),
            new List<UISelectionShip>(),
            new List<UISelectionShip>(),
        };

        public delegate void SelectionEvent();

        public SelectionEvent OnSelectionChanged;
        
        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            InitializeSelectionSets();
        }

        void InitializeSelectionSets()
        {
            ClearSelectionSets();
            
            // Get reference to all ships on player's team
            Team team = TeamManager.instance.GetTeamByID(playerController.teamId);
            
            //Foreach ship on team
            foreach (NetworkEntity entity in team.entities)
            {
                Ship s = entity as Ship;
                //foreach container
                for (int c = 0; c < selectionSets.Count; c++)
                {
                    // Create a UI ship
                    UISelectionShip selectionShip = UIShipFactory.instance.CreateSelectionShip(s.type)
                        .GetComponent<UISelectionShip>();
                    // Set UI ship to team ship ID
                    selectionShip.entityId = s.GetEntityId();
                    //Add to list
                    uiShips[c].Add(selectionShip);
                    selectionShip.transform.SetParent(selectionSetContainers[c]);
                    // Disable
                    selectionShip.gameObject.SetActive(false);
                }
            }
            OnSelectionChanged?.Invoke();
        }

        void ClearSelectionSets()
        {
            // if there are already ships (i.e. switch teams or something)
            if (uiShips.Count > 0)
                foreach (List<UISelectionShip> list in uiShips)
                foreach (UISelectionShip s in list)
                    Destroy(s);
            uiShips = new List<List<UISelectionShip>>()
            {
                new List<UISelectionShip>(),
                new List<UISelectionShip>(),
                new List<UISelectionShip>(),
                new List<UISelectionShip>()
            };
            
            // Any other cleanup we need? Might not even encounter this until mid-game team switching is possible...
        }

        void UpdateSelectionSet(int selectionSet)
        {
            // Disable all ships in selection set
            foreach (Transform child in selectionSetContainers[selectionSet])
            {
                child.gameObject.SetActive(false);
            }

            // foreach ship in current selection
            foreach (ISelectable selectable in currentSelection)
            {
                // Get uiship with linq where entity id matches current ship and activate
                uiShips[selectionSet].Single(s => s.entityId == selectable.GetOwningEntity().GetEntityId()).gameObject
                    .SetActive(true);
            }
        }

        public void RemoveSelectableFromSelectionSets(uint entityId)
        {
            for (int i = 0; i < selectionSets.Count; i++)
            {
                if (selectionSets[i].Exists(s => s.GetOwningEntity().GetEntityId() == entityId))
                {
                    var selectable = selectionSets[i].Single(s => s.GetOwningEntity().GetEntityId() == entityId);
                        selectionSets[i].Remove(selectable);
                    UpdateSelectionSet(i);
                    Destroy(selectable.GetOwningEntity().GetGameObject());
                }
            }
            OnSelectionChanged?.Invoke();
        }

        public List<ISelectable> GetCurrentSelection()
        {
            return currentSelection;
        }

        public List<ISelectable> GetSelectionSet(int selectionSetId)
        {
            return selectionSets[selectionSetId];
            return null;
        }

        public void SelectAll(int teamId)
        {
            var ships = EntityManager.GetEntities().Where(s => s.GetTeam().teamId == (uint) teamId);
            foreach (var ship in ships)
            {
                AddToSelection(ship.selectionHandler);
            }
            OnSelectionChanged?.Invoke();
        }

        public void CreateOrReplaceSelectionSet(int selectionSetId)
        {
            selectionSets[selectionSetId] = null;

            List<ISelectable> newSelection = new List<ISelectable>();
            foreach (ISelectable selectable in currentSelection)
            {
                newSelection.Add(selectable);
            }

            selectionSets[selectionSetId] = newSelection;
            UpdateSelectionSet(selectionSetId);
        }

        public void RecallSelectionSet(int selectionSetId)
        {
            List<ISelectable> selectionSet = selectionSets[selectionSetId];

                // For any object that is in current selection that isn't in selection set, Deselect
                foreach (ISelectable selectable in currentSelection)
                {
                    if (!selectionSet.Contains(selectable))
                    {
                        selectable.Deselect();
                    }
                }

                // For any object that is in the new selection that isn't in the current selection, Select
                // For any object that is in current selection that isn't in selection set, Deselect
                foreach (ISelectable selectable in selectionSet)
                {
                    if (!currentSelection.Contains(selectable))
                    {
                        selectable.Select();
                    }
                }

                currentSelection = selectionSets[selectionSetId];
                OnSelectionChanged?.Invoke();
        }

        public void SetSelectionFromGroup(List<ISelectable> selectables)
        {
            foreach (var selectable in currentSelection)
            {
                if (!selectables.Contains(selectable)) selectable.Deselect();
            }

            foreach (ISelectable selectable in selectables)
            {
                if (!currentSelection.Contains(selectable)) selectable.Select();
            }

            currentSelection = selectables;
            OnSelectionChanged?.Invoke();
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
            OnSelectionChanged?.Invoke();
        }

        public void AddToSelection(ISelectable selectable)
        {
            if (!currentSelection.Contains(selectable))
            {
                currentSelection.Add(selectable);
                selectable.Select();
            }
            OnSelectionChanged?.Invoke();
        }

        public void ClearSelection()
        {
            foreach (ISelectable selectable in currentSelection)
            {
                selectable.Deselect();
            }

            currentSelection.Clear();
            OnSelectionChanged?.Invoke();
        }

        public void RemoveFromSelection(ISelectable selectable)
        {
            if (currentSelection.Contains(selectable))
            {
                currentSelection.Remove(selectable);
                selectable.Deselect();
            }
            OnSelectionChanged?.Invoke();
        }
    }
}