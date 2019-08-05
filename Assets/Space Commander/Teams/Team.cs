using UnityEngine;

namespace SpaceCommander.Teams
{
    public struct Team
    {
        public uint teamID;
        public int pointsToSpend;
        public string name;
        public Color color;
        private bool playersCanJoin;
    }
}