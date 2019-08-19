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
         public List<ShipIdDictionary> fleetBattleGroups = new List<ShipIdDictionary>();
         public int numberOfPlayerSlots;
    }
}