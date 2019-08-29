using System.Collections.Generic;
using StellarArmada.Entities.Ships;
using StellarArmada.UI;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Entities.Ships
{
    public class ShipPriceManager : MonoBehaviour
    {
        public static ShipPriceManager instance;
        public ShipIdDictionary shipPriceDictionary;

        public void Awake()
        {
            instance = this;
        }

        public int GetShipPrice(UIShipyardShip ship)
        {
            return shipPriceDictionary[ship.shipType];
        }

        public int GetGroupPrice(List<UIShipyardShip> ships)
        {
            int val = 0;
            foreach (UIShipyardShip s in ships)
            {
                val += GetShipPrice(s);
            }

            return val;
        }
    }
}