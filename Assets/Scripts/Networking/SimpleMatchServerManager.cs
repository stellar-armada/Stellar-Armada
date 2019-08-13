using Mirror;
using SpaceCommander.Scenarios;
using SpaceCommander.Teams;
using UnityEngine;
namespace SpaceCommander.Match
{
    public class SimpleMatchServerManager : NetworkBehaviour
    {
        public void Start()
        {
            if (!isServer) return;
            
            // Get a random scenario
            MatchScenarioManager.instance.CmdChooseRandomScenario();
            
            Scenario scenario = MatchScenarioManager.instance.GetCurrentScenario();

            if (MapParent.instance == null)
            {
                Debug.LogError("No scene root found");
            }
            
            // Create map from scenario template
            GameObject m = Instantiate(scenario.levelPrefab, MapParent.instance.transform);
            
            NetworkServer.Spawn(m);
            
            // Create teams
            foreach (TeamInfo teamInfo in scenario.teamInfo)
            {
                TeamManager.instance.CmdCreateNewTeam(teamInfo.numberOfPlayerSlots);
            }

            // Create ships
            for (uint i = 0; i < scenario.teamInfo.Length; i++)
            {
                // Create teams from scenario template
                TeamManager.instance.CmdCreateNewTeam(scenario.teamInfo[i].numberOfPlayerSlots); // Init with a fixed number of slots
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
                            ShipFactory.instance.CmdCreateShipForTeam(i, g, key.Key, Random.onUnitSphere * 10f, Quaternion.identity);
                        }
                    }
                }
            }

            // Start prematch timer
            
            
        }
    }
}