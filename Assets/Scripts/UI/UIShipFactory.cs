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
            newUIShip = Instantiate(shipyardShipPrefabs[type]);
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
        
        public List<UIShipyardShip> GetShipyardShips(uint teamId)
        {
            Scenario currentScenario = MatchScenarioManager.instance.GetCurrentScenario();
            //Store ships in a list for a second
            List<UIShipyardShip> ships = new List<UIShipyardShip>();
            for (int g = 0; g < currentScenario.teamInfo[teamId].fleetBattleGroups.Count; g++)
            {
                foreach (var shipKeyVal in currentScenario.teamInfo[teamId].fleetBattleGroups[g])
                {
                    for (int numShips = 0; numShips < shipKeyVal.Value; numShips++)
                    {
                        // For each ship, instantiate for current team
                        UIShipyardShip s = CreateShipyardShip(shipKeyVal.Key).GetComponent<UIShipyardShip>();
                        s.group = g;
                        ships.Add(s);
                    }
                }
            }

            return ships;
        }
    }
}