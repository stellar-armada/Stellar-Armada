using System.Collections.Generic;
using System.Linq;
using Mirror;
using SpaceCommander.Scenarios;
using SpaceCommander.Ships;
using SpaceCommander.Teams;
using UnityEngine;

#pragma warning disable 0649
namespace SpaceCommander.Match
{
    public class MatchServerManager : NetworkBehaviour
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
                        for (int numShips = 0; numShips < key.Value; numShips++)
                        {
                            // For each ship, instantiate for current team
                            Ship s = ShipFactory.instance.CreateShipForTeam(i, g, key.Key);
                            ships.Add(s);
                        }
                    }

                    // for each battlegroup, make a new transform with vector 
                    GameObject go = new GameObject();
                    go.name = "Group Position: " + g;

                    // get positions back for list of ships from formation manager
                    Dictionary<Ship, Vector3> shipPositions = ShipFormationManager.GetFormationPositionsForShips(ships);

                    // Get list of warp vectors in level
                    var warpPoints = m.GetComponent<Level>().warpPoints;

                    foreach (Ship s in ships)
                    {
                        Transform wp = warpPoints.Single(w => (w.teamIndex == i && w.groupNumber == g)).transform;
                        Vector3 pos = wp.position;
                        Quaternion rot = wp.rotation;

                        Matrix4x4 parentMatrix = Matrix4x4.TRS(pos, rot, Vector3.one).inverse;

                        Vector3 newPos = parentMatrix.MultiplyPoint3x4(shipPositions[s]);

                        Debug.Log("Position: " + pos + " | Ship formation position: " + shipPositions[s] +
                                  " | Transformed position: " + newPos);


                        s.shipWarp.InitWarp(newPos, Quaternion.Inverse(rot));
                    }
                }
            }


            // Start prematch timer
        }
    }
}