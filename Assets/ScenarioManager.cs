using System.Collections.Generic;
using UnityEngine;

public class ScenarioManager : MonoBehaviour
{
    public static ScenarioManager instance;
    [HideInInspector] public List<Scenario> scenarios = new List<Scenario>();
    
    void Awake()
    {
        instance = this;
        Object[] scenarioObjects = Resources.LoadAll("Scenarios", typeof(Scenario));
        foreach (Object scenario in scenarioObjects)
        {
            scenarios.Add((Scenario)scenario);
        }
    }
}
