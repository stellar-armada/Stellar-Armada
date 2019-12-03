using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

#pragma warning disable 0649
#pragma warning disable 0067
namespace StellarArmada.Teams
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
        // Network manager that manages the creation of teams, and autojoins players to teams with empty slots
        public static TeamManager instance; // singleton accessor
        
        public TeamTemplate[] templates;

        public GameObject teamPrefab;

        public List<Team> teams = new List<Team>();

        private int newTeamIndex;
        
        public delegate void TeamEventSelectionHandler(uint teamId);

        [SyncEvent] public event TeamEventSelectionHandler EventTeamShipsUpdated;

        static int teamIncrementer = 0;

        void Awake()
        {
            instance = this;
        }
        
        [Server]
        public void CreateNewTeam(TeamInfo teamInfo) // Would be great to pass full TeamInfo, but Cmds can only take basic types
        {
            TeamTemplate template = templates[newTeamIndex++];
            Team t = Instantiate(teamPrefab, transform).GetComponent<Team>();
            t.teamId = (uint)teamIncrementer++;
            t.teamName = template.name;
            t.color = template.color;
            t.insignia = template.insignia;
            t.playerSlots = teamInfo.numberOfPlayerSlots;
            t.pointsToSpend = teamInfo.pointsToSpend;

            foreach (var shipType in teamInfo.availableShipTypes)
            {
                t.availableShipTypes.Add(shipType);
            }
            
            // Populate prototypes
            for (int group = 0; group < teamInfo.fleetBattleGroups.Count; group++) 
                foreach (var ship in teamInfo.fleetBattleGroups[group])
                    for (int i = 0; i < ship.Value; i++)
                    {
                        // add one for each 
                        ShipPrototype p = new ShipPrototype();
                        p.shipType = ship.Key;
                        p.group = group;
                        p.id = ShipPrototype.prototypeEntityIncrement++;
                        t.prototypes.Add(p);
                    }
            
            NetworkServer.Spawn(t.gameObject);
        }


        [Server]
        public void ServerJoinTeam(uint playerId)
        {
            // Find team with most slots available (slots avail - num players)

            List<Team> sortedTeams = new List<Team>(teams.OrderByDescending(t => (t.playerSlots - t.players.Count)));
            
            // Set player teamId to team's
            sortedTeams[0].CmdAddPlayer(playerId);
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