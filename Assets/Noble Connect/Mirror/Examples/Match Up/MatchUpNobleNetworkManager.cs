using System.Collections.Generic;
using MatchUp;
using NobleConnect.Mirror;
using UnityEngine;
using Mirror;

// Example implementation of NobleNetworkManager utilizing Match Up for matchmaking
    // Look at ExampleMatchUpHUD for more information on how to use it. 
    public class MatchUpNobleNetworkManager : NobleNetworkManager
    {
        Matchmaker matchUp;
        
        // Networked prefab to instantiate for all players, containing all the networked managers (as opposed to Static managers)
        public GameObject matchManagerPrefab;
        
        public void CreateMatchManager()
        {
            GameObject matchManager = Instantiate(matchManagerPrefab);
            NetworkServer.Spawn(matchManager);
        }

        private void OnServerInitialized()
        {
        }

        override public void Start()
        {
            base.Start();
            matchUp = GetComponent<Matchmaker>();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn); }

        public override void OnServerConnect(NetworkConnection conn)
        {
            base.OnServerConnect(conn); }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn); }

        public override void OnStopHost()
        {
            base.OnStopHost(); 
            matchUp.DestroyMatch();
        }

        // OnServerPrepared is called when the host is listening and has received 
        // their HostEndPoint from the NobleConnect service.
        // This is a good time to create a match using Match Up.
        // The hostAddress and hostPort are stored as matchData for the newly created match.
        // Clients will select a match and retrieve the ip and port to connect to the host.
        // Look in ExampleMatchUpHUD.cs to see how that works.
        public override void OnServerPrepared(string hostAddress, ushort hostPort)
        {
            // Create the match data and add the host address and port
            var matchData = new Dictionary<string, MatchData>() {
                { "HostAddress", hostAddress },
                { "HostPort", (int)hostPort },
            };

            // Create the Match with the associated MatchData
            matchUp.CreateMatch(maxConnections + 1, matchData);
            Debug.Log("Creating match: " + hostAddress + ":" + hostPort);
            CreateMatchManager();

        }
    }