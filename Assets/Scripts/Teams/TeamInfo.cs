using System;
using System.Collections.Generic;

namespace SpaceCommander
{
    [Serializable]
    public class TeamInfo
    {
         public List<FleetDictionary> fleetBattleGroups = new List<FleetDictionary>();
         public int numberOfPlayerSlots;
    }
}