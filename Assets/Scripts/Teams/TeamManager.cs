using System.Collections.Generic;
using System.Linq;
using Mirror;
using SpaceCommander.Game;
using SpaceCommander.Player;
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
        public void CmdCreateNewTeam(int numerOfPlayerSlots) // Would be great to pass full TeamInfo, but Cmds can only take basic types
        {
            TeamTemplate template = templates[newTeamIndex++];
            Team t = Instantiate(teamPrefab, transform).GetComponent<Team>();
            NetworkServer.Spawn(t.gameObject);
            t.teamId = (uint)teams.Count;
            t.name = template.name;
            t.color = template.color;
            t.insignia = template.insignia;
            t.playerSlots = numerOfPlayerSlots;
            teams.Add(t);
            Debug.Log("Added team " + t);
        }

        [Command]
        public void CmdJoinTeam(uint playerId)
        {
            // Find team with most slots available (slots avail - num players)

            List<Team> sortedTeams = new List<Team>(teams.OrderBy(t => (t.playerSlots - t.entities.Count)));

            // Set player teamId to team's
            sortedTeams[0].CmdAddPlayer(playerId);
            Debug.Log("Put player " + playerId + " on team " + sortedTeams[0].teamId);
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