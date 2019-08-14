using System.Collections;
using System.Collections.Generic;
using SpaceCommander.Ships;
using UnityEngine;

namespace SpaceCommander.UI
{
    public class UIShipFactory : MonoBehaviour
    {
        public static UIShipFactory instance;
        public ShipDictionary groupShipPrefabs;
        public ShipDictionary selectionShipPrefabs;

        void Awake()
        {
            instance = this;
        }
        public GameObject CreateGroupShip(ShipType type)
        {
            GameObject newUIShip = Instantiate(groupShipPrefabs[type]);
            return newUIShip;
        }

        public GameObject CreateSelectionShip(ShipType type)
        {
            GameObject newUIShip = Instantiate(selectionShipPrefabs[type]);
            return newUIShip;
        }
    }
}