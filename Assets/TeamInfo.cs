using System;
using SpaceCommander.Teams;

namespace SpaceCommander
{
    [Serializable]
    public class TeamInfo
    {
        public int pointsToSpend;
        public FleetDictionary fleet = new FleetDictionary();
        public Team team;
        public int maxNumberOfPlayers;
        public bool playersCanJoin;
    }
}