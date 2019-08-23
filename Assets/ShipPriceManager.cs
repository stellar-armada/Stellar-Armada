using System.Collections.Generic;
using StellarArmada.Ships;
using UnityEngine;

public class ShipPriceManager : MonoBehaviour
{
    public ShipPriceManager instance;
    public ShipIdDictionary shipPricedictionary;

    public void Awake()
    {
        instance = this;
    }

    public int GetShipPrice(Ship ship)
    {
        return shipPricedictionary[ship.type];
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
