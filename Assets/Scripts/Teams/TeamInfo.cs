using System;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
namespace SpaceCommander
{

    [Serializable]
    public class WarpVector
    {
        public Vector3 position;
        public Vector3 rotation;

    }
    [Serializable]
    public class TeamInfo
    {
         public List<FleetDictionary> fleetBattleGroups = new List<FleetDictionary>();
         public List<WarpVector> battleGroupWarpVectors;
         public int numberOfPlayerSlots;
    }
}