using System;
using System.Collections.Generic;
using SpaceCommander.Teams;
using UnityEngine.Serialization;

namespace SpaceCommander
{
    [Serializable]
    public class TeamInfo
    {
        public List<FleetDictionary> fleetBattleGroups = new List<FleetDictionary>();
        public Team team;
        public int maxNumberOfPlayers;
        public bool playersCanJoin;
    }
}