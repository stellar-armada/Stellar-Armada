using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

namespace SpaceCommander.Teams
{
    [System.Serializable]
    public class TeamTemplate
    {
        public Color color;
        public string name;
    }
    public class SyncListTeam : SyncList<Team> {}
    
    public class TeamManager : NetworkBehaviour
    {
        public static TeamManager instance; // singleton accessor
        
        public TeamTemplate[] templates;
        
        public SyncListTeam teams = new SyncListTeam();

        private int team;

        void Awake()
        {
            instance = this;
        }
        
        [Command]
        public void CmdCreateNewTeam()
        {
            TeamTemplate template = templates[team++];
            Team t = new Team();
            t.teamID = (uint)teams.Count;
            t.name = template.name;
            t.color = template.color;
            teams.Add(t);
            Debug.Log("Added team " + t);
        }

        public Team GetTeamByID(uint teamID)
        {
            return teams.Single(t => t.teamID == teamID);
        }

        public void DestroyAllTeams()
        {
            teams = new SyncListTeam();
        }

        private void OnDestroy()
        {
            instance = null;
        }

    }
}