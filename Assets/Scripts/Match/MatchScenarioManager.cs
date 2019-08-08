using System.Linq;
using UnityEngine;
using Mirror;
using SpaceCommander.Scenarios;

namespace SpaceCommander.Match
{
    public class MatchScenarioManager : NetworkBehaviour
    {
        public static MatchScenarioManager instance; // private singleton with public GetCurrentMatch accessor

        public delegate void ScenarioChangeDelegate(string scenarioName);

        [SyncEvent] public event ScenarioChangeDelegate EventScenarioChanged;

        Scenario currentScenario;

        [HideInInspector] public string currentScenarioName; // Hook for clients to listen for scenario change

        [Command]
        public void CmdChooseRandomScenario()
        {
            currentScenario =
                ScenarioManager.instance.scenarios[Random.Range(0, ScenarioManager.instance.scenarios.Count - 1)];
            currentScenarioName = currentScenario.name;
            EventScenarioChanged?.Invoke(currentScenarioName);
        }

        void Awake()
        {
            instance = this;
            if (isClientOnly)
            {
                LoadScenario();
                EventScenarioChanged += LoadEventScenario;
            }
        }

        public void LoadScenario()
        {
            LoadEventScenario(currentScenarioName);
        }

        public void LoadEventScenario(string scenarioName)
        {
            currentScenario = ScenarioManager.instance.scenarios.Single(s => s.name == scenarioName);
        }

        public Scenario GetCurrentScenario() => currentScenario;
    }
}