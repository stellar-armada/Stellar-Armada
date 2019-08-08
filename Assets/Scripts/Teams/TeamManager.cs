using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

namespace SpaceCommander.Teams
{
    [System.Serializable]
    public class TeamTemplate
    {
        public Color color;
        public string name;
        public Texture insignia;
    }
    public class TeamManager : NetworkBehaviour
    {
        public static TeamManager instance; // singleton accessor
        
        public TeamTemplate[] templates;

        public GameObject teamPrefab;

        public List<Team> teams = new List<Team>();

        private int newTeamIndex;
        
        public delegate void TeamEventSelectionHandler(uint teamId);

        [SyncEvent] public event TeamEventSelectionHandler EventTeamShipsUpdated;
        

        void Awake()
        {
            instance = this;
        }
        
        [Command]
        public void CmdCreateNewTeam()
        {
            TeamTemplate template = templates[newTeamIndex++];
            Team t = Instantiate(teamPrefab, transform).GetComponent<Team>();
            NetworkServer.Spawn(t.gameObject);
            t.teamId = (uint)teams.Count;
            t.name = template.name;
            t.color = template.color;
            t.insignia = template.insignia;
            teams.Add(t);
            Debug.Log("Added team " + t);
        }

        public Team GetTeamByID(uint teamID)
        {
            return teams.Single(t => t.teamId == teamID);
        }

        private void OnDestroy()
        {
            instance = null;
        }

    }
}