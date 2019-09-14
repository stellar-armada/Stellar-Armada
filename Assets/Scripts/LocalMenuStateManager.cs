using Mirror;
using StellarArmada.Entities.Ships;
using StellarArmada.Levels;
using StellarArmada.Match;
using StellarArmada.Networking;
using StellarArmada.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

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
#pragma warning disable 0649
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
        MatchConnectionManager.instance.StartMatchmaking();
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
    
    public void GoToShipyard()
    {
        ChangeMenuState(MenuState.InGame_Shipyard);
    }

    public void WarpIn()
    {
        HideMenu();
            
            // Format shipyard data and feed into createships for team
           HumanPlayerController.localPlayer.CmdCreateShipsForTeam();
   
            // initialize warp and on-screen warp effects
            MiniMap.instance.transform.localScale = Vector3.zero; // Zero out the minimap on start
            
            // on dewarp, hide warp effects and scale up minimap
    }

    public void QuitMatch()
    {
        Debug.Log("Quitting match!");
        if(HumanPlayerController.localPlayer.isServer) NetworkServer.DisconnectAll();
        if(HumanPlayerController.localPlayer.isClient) NetworkClient.Disconnect();
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
            menuStates[menuState]?.HideMenu();

        // Show the new one
        menuState = newMenuState;
        if (menuState != MenuState.None)
            menuStates[menuState]?.ShowMenu();
    }
}
