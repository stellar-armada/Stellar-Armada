using System.Collections.Generic;
using StellarArmada.Entities.Ships;
using StellarArmada.Levels;
using StellarArmada.Match;
using UnityEngine;

namespace StellarArmada.UI
{
    public class UIShipFactory : MonoBehaviour
    {
        // Local player singleton manager
        // Creates UI ships for the local player's menu
        public static UIShipFactory instance;
        public ShipDictionary groupShipPrefabs;
        public ShipDictionary selectionShipPrefabs;
        public ShipDictionary shipyardShipPrefabs;

        private GameObject newUIShip;
        
        void Awake()
        {
            instance = this;
        }

        public GameObject CreateShipyardShip(ShipType type)
        {
            newUIShip = Instantiate(groupShipPrefabs[type]);
            return newUIShip;
        }
        
        public GameObject CreateGroupShip(ShipType type)
        {
            newUIShip = Instantiate(groupShipPrefabs[type]);
            return newUIShip;
        }

        public GameObject CreateSelectionShip(ShipType type)
        {
            newUIShip = Instantiate(selectionShipPrefabs[type]);
            return newUIShip;
        }
        
        public void PopulateShipyard(int teamId)
        {
            Scenario currentScenario = MatchScenarioManager.instance.GetCurrentScenario();
            for (int g = 0; g < currentScenario.teamInfo[teamId].fleetBattleGroups.Count; g++)
            {
                //Store ships in a list for a second
                List<UIShipyardShip> ships = new List<UIShipyardShip>();

                foreach (var key in currentScenario.teamInfo[teamId].fleetBattleGroups[g])
                {
                    for (int numShips = 0; numShips < key.Value; numShips++)
                    {
                        // For each ship, instantiate for current team
                        UIShipyardShip s = UIShipFactory.instance.CreateShipyardShip(key.Key)
                            .GetComponent<UIShipyardShip>();
                        s.group = g;
                        ships.Add(s);
                    }
                }
            }
        }
    }
}