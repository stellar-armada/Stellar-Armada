using System.Collections.Generic;
using System.Linq;
using StellarArmada.Teams;
using StellarArmada.UI;
using StellarArmada.Player;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Entities.Ships
{
    // Local manager to handle population and selection of custom selection sets in the player's UI menu
    // Singleton object inside the MatchPlayer prefab
    // TO-DO: class could have less white space with cleaner initializations
    public class ShipSelectionManager : MonoBehaviour
    {
        public static ShipSelectionManager instance;

        [SerializeField] private HumanPlayerController playerController;
        private List<ISelectable> currentSelection = new List<ISelectable>();
        private bool initialized = false;
        
        public bool IsInitialized() => initialized;

        // 4 selection sets by default
        List<List<ISelectable>> selectionSets = new List<List<ISelectable>>()
        {
            new List<ISelectable>(),
            new List<ISelectable>(),
            new List<ISelectable>(),    
            new List<ISelectable>(),
        };

        List<ISelectable> tempSelection;
        
        [SerializeField] private Transform[] selectionSetContainers;

        // 4 selection sets by default
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

        public void InitializeSelectionSets()
        {
            if (initialized) return;
            initialized = true;
            
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
                    selectionShip.transform.localPosition = Vector3.zero;
                    selectionShip.transform.localScale = Vector3.one;
                    selectionShip.transform.localRotation = Quaternion.identity;
                    // Disable
                    selectionShip.gameObject.SetActive(false);
                }
            }
            // OnSelectionChanged?.Invoke();
        }

        void ClearSelectionSets()
        {
            // if there are already ships (i.e. switch teams or something)
            if (uiShips.Count > 0)
                foreach (List<UISelectionShip> list in uiShips)
                foreach (UISelectionShip s in list)
                    Destroy(s);
            
            uiShips = new List<List<UISelectionShip>>();
            for (int i = 0; i < 4; i++) // magic number = hardcoded number of custom selection groups
                uiShips.Add(new List<UISelectionShip>());

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
        }

        public void SelectAll()
        {
            var ships = EntityManager.GetEntities().Where(s => s.GetTeam().teamId == HumanPlayerController.localPlayer.teamId && s.GetType() == typeof(Ship));
            foreach (var ship in ships)
            {
                AddToSelection(((Ship)ship).shipSelectionHandler);
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
        
        
        public void ClearSelectionSet(int selectionSetId)
        {
            selectionSets[selectionSetId] = null;
            UpdateSelectionSet(selectionSetId);
        }

        public void AddGroupToSelection(int groupNum)
        {
            uint playerTeamId = HumanPlayerController.localPlayer.GetTeamId();
            var group = TeamManager.instance.GetTeamByID(playerTeamId).groups[groupNum].Where(s => s.GetType() == typeof(Ship));
            
            List<ISelectable> selectables = new List<ISelectable>();
            
            foreach (var entity in group)
            {
                selectables.Add(((Ship)entity).GetSelectionHandler());
            }
            
            foreach (var selectable in selectables)
            {
                if (!currentSelection.Contains(selectable))
                {
                    currentSelection.Add(selectable);
                    selectable.Select();
                }
            }
            
            OnSelectionChanged?.Invoke();
        }
        
        public void RemoveGroupFromSelection(int groupNum)
        {
            uint playerTeamId = HumanPlayerController.localPlayer.GetTeamId();
            var group = TeamManager.instance.GetTeamByID(playerTeamId).groups[groupNum].Where(s => s.GetType() == typeof(Ship));
            
            List<ISelectable> selectables = new List<ISelectable>();
            
            foreach (var entity in group)
            {
                selectables.Add(((Ship)entity).GetSelectionHandler());
            }
            
            foreach (var selectable in selectables)
            {
                if (currentSelection.Contains(selectable))
                {
                    currentSelection.Remove(selectable);
                    selectable.Deselect();
                }
            }
            
            OnSelectionChanged?.Invoke();
        }
        
        
        public void SetSelectionToGroup(int groupNum)
        {
            uint playerTeamId = HumanPlayerController.localPlayer.GetTeamId();
            var group = TeamManager.instance.GetTeamByID(playerTeamId).groups[groupNum].Where(s => s.GetType() == typeof(Ship));
            List<ISelectable> selectables = new List<ISelectable>();
            foreach (var entity in group)
            {
                selectables.Add(((Ship)entity).GetSelectionHandler());
            }

            // Deselect any from currect selection
            foreach (var selectable in currentSelection)
            {
                if (!selectables.Contains(selectable)) selectable.Deselect();
            }
            
            // Select any not in current selection
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
                OnSelectionChanged?.Invoke();
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
            Debug.Log("Attempting to add to selection. Current selection length: " + currentSelection.Count);
            if (!currentSelection.Contains(selectable))
            {
                Debug.Log("<color=green>SELECTION </color> success!");
                currentSelection.Add(selectable);
                selectable.Select();
                OnSelectionChanged?.Invoke();
            }
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
                OnSelectionChanged?.Invoke();
            }
            
        }
    }
}