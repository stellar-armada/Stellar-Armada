using StellarArmada.Entities.Ships;
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

        private GameObject newUIShip;
        
        void Awake()
        {
            instance = this;
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
    }
}