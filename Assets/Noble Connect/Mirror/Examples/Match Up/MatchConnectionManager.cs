#if MATCH_UP
using MatchUp;
using NobleConnect.Mirror;
using UnityEngine;
using Mirror;

namespace StellarArmada.Networking
{
    public class MatchConnectionManager : MonoBehaviour
    {
        public static MatchConnectionManager instance;
        
        // The NetworkManager controller by the HUD
        NobleNetworkManager networkManager;

        // A reference to the Matchmaker component
        Matchmaker matchUp;

        // Used to determine which GUI to display
        bool isHost, isClient;

        // Get a reference to the NetworkManager
        public void Awake()
        {
            instance = this;
            // Cast from Unity's network manager to a punchthrough network manager.
            networkManager = GetComponent<NobleNetworkManager>();
            matchUp = GetComponent<Matchmaker>();
        }
        
        public void StartMatchmaking(){
            // Get a list of matches. OnMatchList is called with the results when they are received.
            matchUp.GetMatchList(OnMatchList, 0, 10, null, true);
        }
        
        
        // Join the first match in the list
        // The HostAddress and HostPort are retrieved from the associated match data
        // Connecting to the host works just like Mirror: Set the networkAddress and networkPort and call StartClient()
        private void OnMatchList(bool success, MatchUp.Match[] matches)
        {
            if (success && matches != null)
            {

                if (matches.Length != 0)
                {
                    Debug.Log("Found matches, connecting as client");
                    var matchData = matches[0].matchData;
                    isClient = true;
                    networkManager.networkAddress = matchData["HostAddress"];
                    networkManager.networkPort = matchData["HostPort"];
                    networkManager.StartClient();
                }
                else
                {
                    Debug.Log("No matches found, starting as host");
                    isHost = true;
                    isClient = false;
                    networkManager.StartHost();
                }
                LocalMenuManager.instance.InitializeMatchMenu();
            }
            else
            {
                Debug.LogError("Failed to get matches");
                LocalMenuManager.instance.GoToMainMenu();
            }
        }

        public void Disconnect()
        {
            if(isHost) StopHost();
            else if(isClient) StopClient();
        }


        void StopHost()
        {
            networkManager.StopHost();
            isHost = false;
        }

        void StopClient()
        {
            if (networkManager.client.isConnected)
            {
                // If we are already connected it is best to quit gracefully by sending
                // a disconnect message to the host.
                networkManager.client.connection.Disconnect();
                var msgID = MessagePacker.GetId<DisconnectMessage>();
                networkManager.client.handlers[msgID](new NetworkMessage());
            }
            else
            {
                // If the connection is still in progress StopClient will cancel it
                networkManager.StopClient();
            }

            isClient = false;
        }

    }
}
#endif