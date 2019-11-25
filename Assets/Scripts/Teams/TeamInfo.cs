using System;
using System.Collections.Generic;
using StellarArmada.Ships;

#pragma warning disable 0649
namespace StellarArmada
{
// Used inside of scenario scriptable object to describe teams that need to be created
    [Serializable]
    public class TeamInfo
    {
        public List<ShipType> availableShipTypes;
        public int pointsToSpend;
        public int numberOfPlayerSlots;
        public List<ShipIdDictionary> fleetBattleGroups = new List<ShipIdDictionary>();
    }
}