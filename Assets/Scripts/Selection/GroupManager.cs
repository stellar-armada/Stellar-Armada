using System.Collections.Generic;
using System.Text;
using SpaceCommander.Game;
using SpaceCommander.Teams;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceCommander.Selection
{
    public class GroupManager : MonoBehaviour
    {
        public static GroupManager instance;

        public Text[] groupTexts;

        void Awake()
        {
            instance = this;
        }

        public void UpdateGroupManager(uint updatedTeamId)
        {
            Debug.Log("Groups still need fixing!");

            return;
            // Get groups

            var groups = TeamManager.instance.GetTeamByID(updatedTeamId).groups;

            //foreach group, set groupuimanager.instance.groups[].entitydisplaypanel to current group

            for (int g = 0; g < groups.Count; g++)
            {
                StringBuilder stringBuilder = new StringBuilder();

                for (int s = 0; s < groups[g].Count; s++)
                {
                    stringBuilder.AppendLine(groups[g][s].GetGameObject().name);
                }
               // groupTexts[g].text = stringBuilder.ToString();
            }
        }

        public void SetSelectionToGroup(int groupNum)
        {
            uint playerTeamId = PlayerManager.localInstance.GetTeamId();
            var group = TeamManager.instance.GetTeamByID(playerTeamId).groups[groupNum];
            List<ISelectable> selectables = new List<ISelectable>();
            foreach (var entity in group)
            {
                selectables.Add(entity.GetSelectionHandler());
            }

            SelectionManager.instance.SetSelectionFromGroup(selectables);
            UpdateGroupManager(playerTeamId);
        }
    }
}