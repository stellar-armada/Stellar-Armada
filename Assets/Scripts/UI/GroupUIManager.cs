using System.Collections.Generic;
using StellarArmada.Ships;
using StellarArmada.Player;
using StellarArmada.Teams;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.UI
{
    public class GroupUIManager : MonoBehaviour
    {
        // Local manager to handle population and selection of groups in the player's UI menu
        // Singleton object inside the MatchPlayer prefab

        public static GroupUIManager instance;
        
        [SerializeField] private PlayerController playerController;

        private List<UIGroupShip> ships = new List<UIGroupShip>();
        [SerializeField] List<Transform> uiShipContainers = new List<Transform>();

        private bool groupShipsLocked = true;
        
        // Private ref vars for reuse
        private UIGroupShip newUiGroupShip;
        private Transform t;
        private Ship ship;
        private ShipType type;
        
        public void LockGroupShips()
        {
            groupShipsLocked = true;
            foreach (UIGroupShip s in ships)
            {
                s.DisableControl();
            }
        }
        
        public void UnlockGroupShips()
        {
            groupShipsLocked = false;
            foreach (UIGroupShip s in ships)
            {
                s.EnableControl();
            }
        }

        public bool GroupShipsLocked() => groupShipsLocked;
        
        public bool IsAGroupContainer(Transform inTransform) =>  uiShipContainers.Contains(inTransform);

        private bool isInited; // catch race condition, can test
        
        void Awake()
        {
            instance = this;
        }

        public void UpdateGroupManager()
        {
            var groups = TeamManager.instance.GetTeamByID(playerController.GetTeamId()).groups;
            
            if (ships.Count > 0) // Delete any if there are any. Would only be if we had more ships added or something
            {
                foreach (UIGroupShip gs in ships)
                {
                    Destroy(gs.gameObject);
                }
                ships = new List<UIGroupShip>();
            }

            for (int g = 0; g < groups.Count; g++)
            {
                for (int s = 0; s < groups[g].Count; s++)
                {
                    ship = groups[g][s] as Ship;
                    ship.group = g;
                    type = ship.type; // Should do entity check but it's easier to do this
                    newUiGroupShip = UIShipFactory.instance.CreateGroupShip(type).GetComponent<UIGroupShip>();
                    newUiGroupShip.ship = ship;
                    t = newUiGroupShip.transform;
                    t.SetParent(uiShipContainers[g]);
                    t.localScale = Vector3.one;
                    t.localPosition = Vector3.zero;
                    t.localRotation = Quaternion.identity;
                    ships.Add(newUiGroupShip);
                }
            }
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