#if MIRROR
using System.Collections.Generic;
using UnityEngine;
using MatchUp;
using Mirror;

// Utilize the MatchUp Matchmaker to perform matchmaking
[RequireComponent(typeof(Matchmaker))]
public class ExampleMirrorMatchmaker : MonoBehaviour
{
    // A reference to the MatchUp Matchmaker component that will be used for matchmaking
    Matchmaker matchUp;

    // A list of matches returned by the MatchUp server
    // This is populated in the GetMatchList() method
    Match[] matches;

    bool isHost, isClient;

    string hostAddress;
    int hostPort;

    // Get a references to components we will use often
	void Awake()
    {
        matchUp = GetComponent<Matchmaker>();
	}

    // Display buttons for hosting, listing, joining, and leaving matches.
    void OnGUI()
    {
        if (!matchUp.IsReady || Matchmaker.externalIP == null) GUI.enabled = false;
        else GUI.enabled = true;

        if (!isHost && !isClient)
        {
            // Host a match
            if (GUI.Button(new Rect(10, 10, 150, 48), "Host"))
            {
                HostAMatch();
            }

            // List matches
            if (GUI.Button(new Rect(10, 85, 150, 48), "List matches"))
            {
                GetMatchList();
            }

            // Display the match list
            if (matches != null)
            {
                for (int i = 0; i < matches.Length; i++)
                {
                    DisplayJoinMatchButton(i, matches[i]);
                }
            }
        }
        else
        {
            GUI.Label(new Rect(10, 10, 250, 48), "Host Address: " + hostAddress);
            GUI.Label(new Rect(10, 35, 250, 48), "Host Port: " + hostPort);
            if (isClient)
            {
                GUI.Label(new Rect(10, 60, 250, 48), "Status: Match Joined");
            }
            else if (isHost)
            {
                GUI.Label(new Rect(10, 60, 250, 48), "Status: Match Created");
            }

            // Leave match
            if (GUI.Button(new Rect(10, 85, 150, 48), "Disconnect"))
            {
                Disconnect();
            }
        }
    }

    // Display a button to join a match
    void DisplayJoinMatchButton(int i, Match match)
    {
        // Grab some match data to display on the button
        var data = matches[i].matchData;

        // Join the match
        if (GUI.Button(new Rect(170, 10 + i * 26, 600, 25), data["Match name"]))
        {
            matchUp.JoinMatch(matches[i], OnJoinMatch);
        }
    }

    // Host a match
    void HostAMatch()
    {
        isHost = true;

        // Once you have the host's connection info, add it as match data and create the match
        hostAddress = NetworkManager.singleton.networkAddress;// Matchmaker.externalIP;
        if (Transport.activeTransport.GetType() == typeof(TelepathyTransport))
        {
            hostPort = ((TelepathyTransport)Transport.activeTransport).port;
        }
        else
        {
            // All you need to do to support other transports is get the host port similar to above.
            Debug.LogError("Unsupported Mirror Transport");
            return;
        }

        // Start hosting with Mirror
        NetworkManager.singleton.StartHost();

        // You can set MatchData when creating the match. (string, float, double, int, or long)
        var matchData = new Dictionary<string, MatchData>() {
            { "Match name", "Layla's Match" },
            { "Host Address", hostAddress },
            { "Host Port", hostPort }
        };

        // Create the Match with the associated MatchData
        matchUp.CreateMatch(10, matchData, OnMatchCreated);
    }
    
    // Called when a response is received from the CreateMatch request.
    void OnMatchCreated(bool success, Match match)
    {
        Debug.Log("Created match: " + match.matchData["Match name"]);
    }
    
    // Get a filtered list of matches
    void GetMatchList()
    {
        Debug.Log("Fetching match list");

        // Get the match list. The results will be received in OnMatchList()
        matchUp.GetMatchList(OnMatchListGot, 0, 10);
    }

    // Called when the match list is retreived via GetMatchList
    void OnMatchListGot(bool success, Match[] matches)
    {
        if (!success) return;

        Debug.Log("Received match list.");
        this.matches = matches;
    }

    // Called when a response is received from a JoinMatch request
    void OnJoinMatch(bool success, Match match)
    {
        if (!success) return;

        isClient = true;

        // Get the host's connection info
        hostAddress = match.matchData["Host Address"];
        hostPort = match.matchData["Host Port"];

        Debug.Log("Joined match: " + match.matchData["Match name"] + " " + hostAddress + ":" + hostPort);

        // Join the host using Mirror
        NetworkManager.singleton.networkAddress = hostAddress;
        if (Transport.activeTransport.GetType() == typeof(TelepathyTransport))
        {
            ((TelepathyTransport)Transport.activeTransport).port = (ushort)hostPort;
        }
        else
        {
            // All you need to do to support other transports is set the host port similar to above.
            Debug.LogError("Unsupported Mirror Transport");
            return;
        }
        NetworkManager.singleton.StartClient();
    }
    
    // Disconnect and leave the Match
    void Disconnect()
    {
        // Stop hosting and destroy the match
        if (isHost)
        {
            Debug.Log("Destroyed match");
            isHost = false;
            matchUp.DestroyMatch();

            // Stop hosting in mirror
            NetworkManager.singleton.StopHost();
        }

        // Disconnect from the host and leave the match
        else
        {
            Debug.Log("Left match");
            isClient = false;
            matchUp.LeaveMatch();

            // Disconnect client
            NetworkManager.singleton.StopClient();
        }
    }
}
#endif