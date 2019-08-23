using StellarArmada.Ships;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Scenarios
{
    [System.Serializable, CreateAssetMenu(fileName = nameof(Scenario), menuName = "Scenario", order = 56)]
    public class Scenario : ScriptableObject
    {
        public float setupTime = 1f;
        public float matchTime = 15f;
        public float postMatchTime = 1f;
        public string scenarioName;
        public int minimumNumberOfPlayers;
        public int maximumNumberOfPlayers;
        public string description;
        public TeamInfo[] teamInfo;
        public GameObject levelPrefab;
    }
}