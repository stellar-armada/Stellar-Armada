using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Player
{
    // Manages the list of current players for easy access
    // Players register on spawn and re-register on despawn
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager instance;
        
        public static List<PlayerController> players = new List<PlayerController>();
        
        void Awake()
        {
            instance = this;
        }

        public void RegisterPlayer(PlayerController playerController)
        {
            //Debug.Log("Registering player");
            if (!players.Contains(playerController))
            {
                players.Add(playerController);
            }
            else
            {
                Debug.LogError("Player list already contains player");
            }
        }

        public void UnregisterPlayer(PlayerController playerController)
        {
            if (!players.Contains(playerController))
            {
                players.Remove(playerController);
            }
            else
            {
                Debug.LogError("Player list does not contain player");

            }
        }
        
        public static List<PlayerController> GetPlayers() => players;


        public static PlayerController GetPlayerById(uint netID) => players.Single(p => p.GetId() == netID);
    }
}