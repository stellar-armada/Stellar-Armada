using System;
using System.Collections.Generic;
using SpaceCommander.Teams;

namespace SpaceCommander
{
    [Serializable]
    public class TeamInfo
    {
         public List<FleetDictionary> fleetBattleGroups = new List<FleetDictionary>();
         public int numberOfPlayerSlots;
    }
}