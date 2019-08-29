using System.Collections.Generic;
using System.Linq;
using Mirror;
using StellarArmada.Entities.Ships;
using StellarArmada.Levels;
using StellarArmada.Teams;
using StellarArmada.UI;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Match
{
    public class MatchServerManager : NetworkBehaviour
    {

        public static MatchServerManager instance;
        public int numberOfPlayerSlots = 0;

        private GameObject map;

        // Monolithic server initialization and management script
        // Handles the setup and selection of a random scenario, assignment of teams, creation of ships, everything players need
        // TO-DO: Add match clock and state setup and initialization!
        // TO-DO: Comment and clean up refs
        public void Start()
        {
            if (!isServer) return;
   
            // Get a random scenario
            MatchScenarioManager.instance.CmdChooseRandomScenario();

            Scenario currentScenario = MatchScenarioManager.instance.GetCurrentScenario();

            if (LevelRoot.instance == null)
            {
                Debug.LogError("No scene root found");
            }

            // Create map from scenario template
            map = Instantiate(currentScenario.levelPrefab, LevelRoot.instance.transform);

            NetworkServer.Spawn(map);

            // Create teams
            foreach (TeamInfo teamInfo in currentScenario.teamInfo)
            {
                TeamManager.instance.CmdCreateNewTeam(teamInfo.numberOfPlayerSlots);
                numberOfPlayerSlots += teamInfo.numberOfPlayerSlots;
            }
        }
        





        // Start prematch timer
    }
}