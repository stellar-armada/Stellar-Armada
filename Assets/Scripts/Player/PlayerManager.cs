using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

#pragma warning disable 0649
namespace SpaceCommander.Game
{
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager instance;
        
        public static List<IPlayer> players = new List<IPlayer>();
        
        void Awake()
        {
            instance = this;
        }

        public void RegisterPlayer(IPlayer player)
        {
            //Debug.Log("Registering player");
            if (!players.Contains(player))
            {
                players.Add(player);
            }
            else
            {
                Debug.LogError("Player list already contains player");
            }
        }

        public void UnregisterPlayer(IPlayer player)
        {
            if (!players.Contains(player))
            {
                players.Remove(player);
            }
            else
            {
                Debug.LogError("Player list does not contain player");

            }
        }
        
        public static List<IPlayer> GetPlayers() => players;


        public static IPlayer GetPlayerById(uint netID) => players.Single(p => p.GetId() == netID);
    }
}