using System.Collections.Generic;
using StellarArmada.Entities.Ships;
using UnityEngine;

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

        public int GetShipPrice(Ship ship)
        {
            return shipPriceDictionary[ship.type];
        }

        public int GetGroupPrice(List<Ship> ships)
        {
            int val = 0;
            foreach (Ship s in ships)
            {
                val += GetShipPrice(s);
            }

            return val;
        }
    }
}