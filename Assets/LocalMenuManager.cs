using System.Collections.Generic;
using Mirror;
using StellarArmada.Networking;
using UnityEngine;

public enum MenuState
{
    None,
    MainMenu,
    Settings,
    HowToPlay,
    Connecting,
    InGame_WaitingForPlayers,
    InGame_ReadyTo_Start,
    InGame_FleetSelection,
    InGame_Defeat,
    InGame_Victory
}

public class LocalMenuManager : MonoBehaviour
{
    [SerializeField] MenuStateDictionary menuStates = new MenuStateDictionary();
    
    public MenuState startMenuState = MenuState.MainMenu;

    public MenuState networkStartMenuState = MenuState.InGame_WaitingForPlayers;
    
    private MenuState menuState = MenuState.None;
    
    public static LocalMenuManager instance;

    public void GoToMainMenu()
    {
        ChangeMenuState(MenuState.MainMenu);
    }

    public void StartMatchmaking()
    {
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

    public void ShowGameIsReadyToStartMenu()
    {
        ChangeMenuState(MenuState.InGame_ReadyTo_Start);
    }

    public void ShowFleetSelectionMenu()
    {
        ChangeMenuState(MenuState.InGame_FleetSelection);
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
        MenuState m;
        if (NetworkManager.singleton.isNetworkActive)
        {
            m = networkStartMenuState;
        }
        else
        {
            m = startMenuState;
        }
        ChangeMenuState(m);
        // Hide all menus, just in case one got left on in dev
        foreach (var view in menuStates)
        {
            if (view.Key != m)
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
