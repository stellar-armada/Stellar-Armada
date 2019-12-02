using Mirror;
using StellarArmada.Levels;
using StellarArmada.Player;
using StellarArmada.Teams;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Match
{
    public class MatchServerManager : NetworkBehaviour
    {
        
        public static MatchServerManager instance;
        public int numberOfPlayerSlots = 0;

        [SerializeField] private MatchStateManager matchStateManager;
        
        private GameObject map;
        private Scenario currentScenario;
        
        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            if (isServer)
            {
                Invoke(nameof(Initialize),2f);
            }
        }

        public Scenario GetCurrentScenario()
        {
            return currentScenario;
        }
        
        // Monolithic server initialization and management script
        // Handles the setup and selection of a random scenario, assignment of teams, creation of ships, everything players need
        // TO-DO: Add match clock and state setup and initialization!
        // TO-DO: Comment and clean up refs
        public void Initialize()
        {
            Debug.Log("Initing");

            MatchScenarioManager.instance.EventScenarioChanged += HandleScenarioChange;
            
            // Get a random scenario
            MatchScenarioManager.instance.CmdChooseRandomScenario();
        }

        void HandleScenarioChange(string scenarioname){
Debug.Log("Scenario changed to " + scenarioname);
        currentScenario = MatchScenarioManager.instance.GetCurrentScenario();
                numberOfPlayerSlots = currentScenario.numberOfHumanPlayers;
                if (LevelRoot.instance == null)
                {
                    Debug.LogError("No scene root found");
                }

                // Create map from scenario template
                map = Instantiate(currentScenario.levelPrefab, LevelRoot.instance.transform);

                NetworkServer.Spawn(map);

                // Create teams
                foreach (TeamInfo teamInfo in currentScenario.teamInfo)
                {
                    TeamManager.instance.CreateNewTeam(teamInfo);
                }

                matchStateManager.Initialize();
                
                RpcInitializeMenu();
        }

        [ClientRpc]
        void RpcInitializeMenu()
        {
            if (NetworkClient.active)
            {
                LocalMenuStateManager.instance.InitializeMatchMenu();
            }
        }
        
        public void StartMatch()
        {
            
            foreach (var player in PlayerManager.players)
            {
                ((PlayerController)player).CmdInitialize();
            }
        }


    }
}