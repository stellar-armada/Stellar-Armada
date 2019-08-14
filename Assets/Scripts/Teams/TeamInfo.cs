using System;
using System.Collections.Generic;

#pragma warning disable 0649
namespace SpaceCommander
{
    [Serializable]
    public class TeamInfo
    {
         public List<FleetDictionary> fleetBattleGroups = new List<FleetDictionary>();
         public int numberOfPlayerSlots;
    }
}