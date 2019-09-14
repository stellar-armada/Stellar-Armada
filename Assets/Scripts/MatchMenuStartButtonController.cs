using System.Collections;
using System.Collections.Generic;
using Mirror;
using StellarArmada.Match;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649
public class MatchMenuStartButtonController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI waitingForPlayersText;

    private bool isInitialized = false;

    void LateInitializePlayers()
    {
        MatchServerManager.instance.StartMatch();
    }

    // Check number of players in game... on player connect and disconnect...
    void Update()
    {
        if (MatchServerManager.instance != null && MatchServerManager.instance != null && NetworkManager.singleton != null && NetworkManager.singleton.isNetworkActive)
            if(NetworkManager.singleton.numPlayers == MatchServerManager.instance.numberOfPlayerSlots)
        {
            waitingForPlayersText.text = NetworkManager.singleton.numPlayers + " / " + MatchServerManager.instance.numberOfPlayerSlots + " in match";
            
            if (!isInitialized)
            {
                isInitialized = true;
                Debug.Log("Initializing players");
                Invoke(nameof(LateInitializePlayers),1f);
            }
        }
        else
        {
            waitingForPlayersText.text = "Waiting for players - " + NetworkManager.singleton.numPlayers + " / " + MatchServerManager.instance.numberOfPlayerSlots + " in match";
        }
    }

}
