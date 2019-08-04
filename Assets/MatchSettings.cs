using System.Collections;
using System.Collections.Generic;
using SpaceCommander;
using UnityEngine;
using UnityEngine.XR;

[System.Serializable, CreateAssetMenu(fileName = nameof(MatchSettings), menuName = "MatchSettings", order = 56)]    

public class MatchSettings : ScriptableObject
{
   public float setupDuration;

   public float battleDuration;
   // public GameMode gameMode;
}
