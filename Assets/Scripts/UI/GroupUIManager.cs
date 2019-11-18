using System.Collections.Generic;
using System.Linq;
using StellarArmada.Entities;
using StellarArmada.Player;
using StellarArmada.Entities.Ships;
using StellarArmada.Teams;
using UnityEngine;
using UnityEngine.Serialization;

#pragma warning disable 0649
namespace StellarArmada.UI
{
    public class GroupUIManager : MonoBehaviour
    {
        // Local manager to handle population and selection of groups in the player's UI menu
        // Singleton object inside the MatchPlayer prefab

        public static GroupUIManager instance;
        
        [FormerlySerializedAs("humanPlayerController")] [SerializeField] private PlayerController playerController;

        private List<GameObject> ships = new List<GameObject>();
        [SerializeField] List<Transform> uiShipContainers = new List<Transform>();

        public bool IsAGroupContainer(Transform inTransform)
        {
            return uiShipContainers.Contains(inTransform);
        }
        
        private bool isInited; // catch race condition, can test
        
        void Awake()
        {
            humanPlayerController.EventOnPlayerTeamChange += UpdateGroupManager;
            instance = this;
        }

        public void UpdateGroupManager()
        {
            var groups = TeamManager.instance.GetTeamByID(playerController.GetTeamId()).groups;
            
            if (ships.Count > 0) // Delete any if there are any. Would only be if we had more ships added or something
            {
                foreach (GameObject go in ships)
                {
                    Destroy(go);
                }
                ships = new List<GameObject>();
            }

            for (int g = 0; g < groups.Count; g++)
            {
                for (int s = 0; s < groups[g].Count; s++)
                {
                    Ship ship = groups[g][s] as Ship;
                    ship.group = g;
                    ShipType type = ship.type; // Should do entity check but it's easier to do this
                    UIGroupShip newUiGroupShip = UIShipFactory.instance.CreateGroupShip(type).GetComponent<UIGroupShip>();
                    newUiGroupShip.ship = ship;
                    newUiGroupShip.transform.SetParent(uiShipContainers[g]);
                    ships.Add(newUiGroupShip.gameObject);
                }
            }
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

            ShipSelectionManager.instance.SetSelectionFromGroup(selectables);
        }

        public void MoveShipToGroup(UIGroupShip s, int newGroup)
        {

            // Set to the new group
            s.transform.SetParent(uiShipContainers[newGroup]);

            // Set ship to new group
            s.ship.group = newGroup;
        }
    }
}