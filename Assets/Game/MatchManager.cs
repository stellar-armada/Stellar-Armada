using System.Linq;
using UnityEngine;
using Mirror;


namespace SpaceCommander.Match
{
    public class MatchManager : NetworkBehaviour
    {
        public static MatchManager instance; // private singleton with public GetCurrentMatch accessor

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(instance.gameObject);
            }

            instance = this;
        }


        Scenario currentScenario;


        [HideInInspector] [SyncVar(hook = nameof(HandleScenarioChange))]
        public string currentScenarioName; // Hook for clients to listen for scenario change

        [Command]
        public void CmdChooseRandomScenario()
        {
            currentScenario =
                ScenarioManager.instance.scenarios[Random.Range(0, ScenarioManager.instance.scenarios.Count - 1)];
            currentScenarioName = currentScenario.name;
        }

        void HandleScenarioChange(string scenarioName)
        {
            currentScenario = ScenarioManager.instance.scenarios.Single(s => s.name == scenarioName);
        }

        public Scenario GetCurrentScenario() => currentScenario;

    }
}