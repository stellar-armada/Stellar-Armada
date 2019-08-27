using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using StellarArmada.Levels;

#pragma warning disable 0649
namespace StellarArmada.Match
{
    // Handles the choosing of the scenario by the server, including resource loading, random level select, etc.
    
    public class MatchScenarioManager : NetworkBehaviour
    {
        public static MatchScenarioManager instance; // private singleton with public GetCurrentMatch accessor
        [HideInInspector] public List<Scenario> scenarios = new List<Scenario>();
        public delegate void ScenarioChangeDelegate(string scenarioName);

        // Sync event is called on server but fires on all clients
        [SyncEvent] public event ScenarioChangeDelegate EventScenarioChanged;
        
        [HideInInspector] public string currentScenarioName; // Hook for clients to listen for scenario change

        // Local reference variables
        Scenario currentScenario;
        
        void Awake()
        {
            instance = this;
            
            Object[] scenarioObjects = Resources.LoadAll("Scenarios", typeof(Scenario));
            foreach (Object scenario in scenarioObjects)
            {
                scenarios.Add((Scenario) scenario);
            }
            
            if (isClientOnly)
            {
                LoadScenario();
                EventScenarioChanged += LoadEventScenario;
            }
        }

        [Command]
        public void CmdChooseRandomScenario()
        {
            currentScenario = scenarios[Random.Range(0, scenarios.Count - 1)];
            currentScenarioName = currentScenario.name;
            EventScenarioChanged?.Invoke(currentScenarioName);
        }
        
        public void LoadScenario()
        {
            LoadEventScenario(currentScenarioName);
        }

        public void LoadEventScenario(string scenarioName)
        {
            currentScenario = scenarios.Single(s => s.name == scenarioName);
        }

        public Scenario GetCurrentScenario() => currentScenario;
    }
}