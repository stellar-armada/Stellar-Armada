using System.Collections.Generic;
using System.Linq;
using Mirror;
using StellarArmada.Scenarios;
using StellarArmada.Ships;
using StellarArmada.Teams;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Match
{
    public class MatchServerManager : NetworkBehaviour
    {
        public void Start()
        {
            if (!isServer) return;

            // Get a random scenario
            MatchScenarioManager.instance.CmdChooseRandomScenario();

            Scenario scenario = MatchScenarioManager.instance.GetCurrentScenario();

            if (LevelRoot.instance == null)
            {
                Debug.LogError("No scene root found");
            }

            // Create map from scenario template
            GameObject m = Instantiate(scenario.levelPrefab, LevelRoot.instance.transform);

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
                TeamManager.instance.CmdCreateNewTeam(scenario.teamInfo[i]
                    .numberOfPlayerSlots); // Init with a fixed number of slots
                Team t = TeamManager.instance.GetTeamByID(i);

                // Create fleets and assign to teams

                // Iterate through battle groups
                for (int g = 0; g < scenario.teamInfo[i].fleetBattleGroups.Count; g++)
                {
                    //Store ships in a list for a second
                    List<Ship> ships = new List<Ship>();

                    foreach (var key in scenario.teamInfo[i].fleetBattleGroups[g])
                    {
                        bool hasFlagship = false;
                        for (int numShips = 0; numShips < key.Value; numShips++)
                        {
                            // For each ship, instantiate for current team
                            Ship s = ShipFactory.instance.CreateShipForTeam(i, g, key.Key);
                            ships.Add(s);
                        }
                    }

                    // get positions back for list of ships from formation manager
                    Dictionary<Ship, Vector3> shipPositions = ShipFormationManager.instance.GetFormationPositionsForShips(ships);

                    // Get list of warp vectors in level
                    var warpPoints = m.GetComponent<Level>().warpPoints;

                    Transform wp = warpPoints.Single(w => (w.teamIndex == i && w.groupNumber == g)).transform;
                    
                    foreach (Ship s in ships)
                    {
                        
                        Vector3 pos = wp.localPosition;
                        Quaternion rot = wp.rotation;

                        Matrix4x4 parentMatrix = Matrix4x4.TRS(Vector3.zero, rot, Vector3.one).inverse;

                        Vector3 newPos = parentMatrix.MultiplyPoint3x4(shipPositions[s]);

                        Debug.Log("Position: " + pos + " | Ship formation position: " + shipPositions[s] +
                                  " | Transformed position: " + pos + newPos);


                        s.shipWarp.InitWarp(pos + newPos, rot);
                    }
                }
            }


            // Start prematch timer
        }
    }
}