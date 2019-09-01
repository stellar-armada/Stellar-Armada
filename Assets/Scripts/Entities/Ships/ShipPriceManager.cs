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

        public int GetShipPrice(ShipType shipType)
        {
            return shipPriceDictionary[shipType];
        }

        public int GetGroupPrice(List<ShipPrototype> protos)
        {
            int price = 0;
            foreach (var proto in protos)
            {
                price += GetShipPrice(proto.shipType);
            }

            return price;
        }
        
    }
}