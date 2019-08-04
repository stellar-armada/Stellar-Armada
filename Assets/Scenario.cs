using System.Collections;
using System.Collections.Generic;
using SpaceCommander;
using UnityEngine;
[System.Serializable, CreateAssetMenu(fileName = nameof(Scenario), menuName = "Scenario", order = 56)]    

public class Scenario : ScriptableObject
{
    public TeamInfo[] teamInfo;
    public MatchSettings matchSettings;
    public Map map;
}
