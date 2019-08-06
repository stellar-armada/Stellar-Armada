using Mirror;
using SpaceCommander.Game;
using SpaceCommander.Ships;
using SpaceCommander.Teams;
using UnityEngine;
namespace SpaceCommander.Match
{
    public class SimpleMatchServerManager : NetworkBehaviour
    {
        public void Start()
        {
            if (!isServer) return;
            
            Debug.Log("Starting server initialization routine");
            
            // Get a random scenario
            Debug.Log("Fetching random scenario");
            MatchScenarioManager.instance.CmdChooseRandomScenario();
            
            Debug.Log("Getting current scenario");            
            Scenario scenario = MatchScenarioManager.instance.GetCurrentScenario();

            if (SceneRoot.instance == null)
            {
                Debug.LogError("No scene root found");
            }
            
            // Create map from scenario template
            Debug.Log("Creating map");
            GameObject m = Instantiate(scenario.levelPrefab, SceneRoot.instance.transform);
            
            NetworkServer.Spawn(m);
            
            // Create teams
            foreach (TeamInfo teamInfo in scenario.teamInfo)
            {
                TeamManager.instance.CmdCreateNewTeam();
            }

            // Create ships
            for (uint i = 0; i < scenario.teamInfo.Length; i++)
            {
                Debug.Log("Created a team new team at ID " + i);
                // Create teams from scenario template
                TeamManager.instance.CmdCreateNewTeam();
                Team t = TeamManager.instance.GetTeamByID(i);
                
                // Create fleets and assign to teams
                
                // Iterate through battle groups
                for (int g = 0; g < scenario.teamInfo[i].fleetBattleGroups.Count; g++)
                {
                    // Iterate through ship type
                    foreach (var key in scenario.teamInfo[i].fleetBattleGroups[g])
                    {
                        for (int numShips = 0; numShips < key.Value; numShips++)
                        {
                            // For each ship, instantiate for current team
                            ShipFactory.instance.CmdCreateShipForTeam(i, g, key.Key, Vector3.zero, Quaternion.identity);
                            Debug.Log("Gave a " + key.Key + " to team " + i);
                        }
                    }
                }
            }

            // Start prematch timer
            
            
        }
    }
}