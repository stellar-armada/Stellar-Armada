using StellarArmada.Entities.Ships;
using StellarArmada.Match;
using StellarArmada.Networking;
using StellarArmada.Player;
using UnityEngine;

public enum MenuState
{
    None,
    MainMenu,
    Settings,
    HowToPlay,
    Connecting,
    InGame_WaitingForPlayers,
    InGame_Shipyard,
    InGame_Defeat,
    InGame_Victory
}

public class LocalMenuStateManager : MonoBehaviour
{
    [SerializeField] MenuStateDictionary menuStates = new MenuStateDictionary();

    [SerializeField] private Shipyard shipyard;
    
    private static MenuState menuState = MenuState.None; // passes across scenes to maintain state
    
    public static LocalMenuStateManager instance;

    public void GoToMainMenu()
    {
        ChangeMenuState(MenuState.MainMenu);
    }

    public void StartMatchmaking()
    {
        Debug.Log("Reinstate match connection manager");
        // MatchConnectionManager.instance.StartMatchmaking();
        ChangeMenuState(MenuState.Connecting);
    }

    public void GoToSettings()
    {
        ChangeMenuState(MenuState.Settings);
    }

    public void ShowHowToPlay()
    {
        ChangeMenuState(MenuState.HowToPlay);
    }

    public void InitializeMatchMenu()
    {
        ChangeMenuState(MenuState.InGame_WaitingForPlayers);
    }

    public void StartMatch()
    {
        // Call server to put all players on teams (i.e. initialize player)
        MatchServerManager.instance.InitializePlayers();
    }

    public void GoToShipyard()
    {
        ChangeMenuState(MenuState.InGame_Shipyard);
        shipyard.InitializeShipyard();
    }

    public void WarpIn()
    {
        HideMenu();
            
            // Format shipyard data and feed into createships for team
            ShipFactory.instance.CmdCreateShipsForTeam(HumanPlayerController.localPlayer.GetTeam().teamId);
            
            // create bridges for local player's flagship / instantiate bridge in at ship’s bridgeroot
            
            
            // parent sceneroot to ship bridge root
            
            // parent player, minimap, etc to sceneroot
            
            // rename sceneroot to bridgesceneroot
            
            // initialize warp and on-screen warp effects
            
            // on dewarp, hide warp effects and scale up minimap


    }

    public void HideMenu()
    {
        ChangeMenuState(MenuState.None);
    }

    public void ShowDefeatMenu()
    {
        ChangeMenuState(MenuState.InGame_Defeat);
    }

    public void ShowVictoryMenu()
    {
        ChangeMenuState(MenuState.InGame_Victory);
    }

    void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        if (menuState == MenuState.None)
        {
            ChangeMenuState(MenuState.MainMenu);
        }

        // Hide all menus, just in case one got left on in dev
        foreach (var view in menuStates)
        {
            if (view.Key != menuState)
            {
                view.Value.HideMenu();
            }
        }
    }

    void ChangeMenuState(MenuState newMenuState)
    {
        // Hide our current menu
        if (menuState != MenuState.None)
        {
            menuStates[menuState]?.HideMenu();
        }
        
        // Show the new one
        menuState = newMenuState;
        menuStates[menuState]?.ShowMenu();
    }
}
