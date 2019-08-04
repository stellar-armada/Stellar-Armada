using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

namespace SpaceCommander.Game
{
    public class PlayerManager : NetworkBehaviour
    {
        public static PlayerManager instance;
        
        public static List<IPlayer> players = new List<IPlayer>();
        
        public static IPlayer localInstance; // local player accessor

        void Awake()
        {
            instance = this;
        }

        public static IPlayer GetLocalNetworkPlayer()
        {
            return localInstance;
        }

        public static void SetLocalPlayer(IPlayer newLocalPlayer)
        {
            localInstance = newLocalPlayer;
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


        public static IPlayer GetPlayerByNetId(uint netID) => players.Single(p => p.GetId() == netID);
    }
}