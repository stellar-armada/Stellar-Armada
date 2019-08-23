using System.Collections.Generic;
using StellarArmada.Players;
using StellarArmada.Ships;
using StellarArmada.Teams;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.UI
{
    public class GroupUIManager : MonoBehaviour
    {
        [SerializeField] private HumanPlayerController humanPlayerController;

        private List<GameObject> ships = new List<GameObject>();
        [SerializeField] List<Transform> uiShipContainers = new List<Transform>();

        private bool isInited; // catch race condition, can test
        
        void Awake()
        {
            humanPlayerController.EventOnPlayerTeamChange += UpdateGroupManager;
        }

        void Start()
        {

                UpdateGroupManager();
        }

        public void UpdateGroupManager()
        {
            var groups = TeamManager.instance.GetTeamByID(humanPlayerController.GetTeamId()).groups;
            
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
            var group = TeamManager.instance.GetTeamByID(playerTeamId).groups[groupNum];
            List<ISelectable> selectables = new List<ISelectable>();
            foreach (var entity in group)
            {
                selectables.Add(entity.GetSelectionHandler());
            }

            SelectionUIManager.instance.SetSelectionFromGroup(selectables);
        }
    }
}