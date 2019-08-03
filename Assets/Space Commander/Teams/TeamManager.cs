using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

namespace SpaceCommander.Teams
{
    [System.Serializable]
    public class TeamInfo
    {
        public Color color;
        public string name;
    }
    public class SyncListTeam : SyncList<Team> {}
    
    public class TeamManager : NetworkBehaviour
    {
        public static TeamManager instance; // singleton accessor
        
        public TeamInfo[] infos;
        
        [SerializeField] public string[] potentialTeamNames;

        [SerializeField] private bool useTeamNames;

        private static List<int> usedTeamNames = new List<int>();

        public SyncListTeam teams = new SyncListTeam();
        
        [Command]
        public void CmdCreateNewTeam()
        {
            TeamInfo info = infos[GetValue()];
            Team t = new Team();
            t.teamID = (uint)teams.Count;
            t.name = info.name;
            t.color = info.color;
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
        
        void PopulateTeamNames()
        {
            for (int i = 0; i < infos.Length; i++)
            {
                infos[i].name = potentialTeamNames[GetValue()]; // Recursively get team name until we find an unused one
            }
        }

        int GetValue() // Return a team name that hasn't been used yet (stored as an int)
        {
            int val = Random.Range(0, potentialTeamNames.Length - 1);
            if (!usedTeamNames.Contains(val))
            {
                usedTeamNames.Add(val);
                return val;
            }
            return GetValue();
        }
        
        public void Init()
        {
            usedTeamNames.Clear();
            instance = this;
            if(useTeamNames) PopulateTeamNames();
        }

        private void OnDestroy()
        {
            instance = null;
        }

    }
}