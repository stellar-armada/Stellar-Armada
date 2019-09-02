using System.Collections;
using System.Collections.Generic;
using Mirror;
using StellarArmada.Match;
using UnityEngine;
using UnityEngine.UI;

public class MatchMenuStartButtonController : MonoBehaviour
{
    [SerializeField] private Button startMatchButton;

    void Awake()
    {
        startMatchButton.gameObject.SetActive(false);
    }

    // Check number of players in game... on player connect and disconnect...
    void Update()
    {
        if (MatchServerManager.instance != null && NetworkManager.singleton.isNetworkActive &&
            NetworkManager.singleton.numPlayers == MatchServerManager.instance.numberOfPlayerSlots)
        {
            startMatchButton.gameObject.SetActive(true);
        }
        else
        {
            startMatchButton.gameObject.SetActive(false);
        }
    }

}
