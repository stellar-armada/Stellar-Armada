using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Levels
{
    // Describes the set up of the level, teams, etc for the current match, used to instantiate the level and prototype settings
    [System.Serializable, CreateAssetMenu(fileName = nameof(Scenario), menuName = "Scenario", order = 56)]
    public class Scenario : ScriptableObject
    {
        public float setupTime = 1f;
        public float matchTime = 15f;
        public float postMatchTime = 1f;
        public string scenarioName;
        public int numberOfHumanPlayers;
        public string description;
        public TeamInfo[] teamInfo;
        public GameObject levelPrefab;
    }
}