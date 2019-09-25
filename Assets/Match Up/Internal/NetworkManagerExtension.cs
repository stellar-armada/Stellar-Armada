#if UNITY_5 || UNITY_6 || UNITY_2017 || UNITY_2018
using MatchUp;

namespace UnityEngine.Networking
{
    /// <summary>Some useful extensions to the NetworkManager class</summary>
    public static class NetworkManagerExtension
    {
        /// <summary>Adds a version of StartClient to the NetworkManager that accepts a Match.</summary>
        /// <param name="manager">The NetworkManager instance</param>
        /// <param name="match">The Match whose host we should connect to</param>
        public static void StartClient(this NetworkManager manager, MatchUp.Match match)
        {
            // Get the connection info from the Match's MatchData
            string externalIP, internalIP;
            externalIP = match.matchData["externalIP"];
            internalIP = match.matchData["internalIP"];

            manager.networkPort = match.matchData["port"];
            manager.networkAddress = Matchmaker.PickCorrectAddressToConnectTo(externalIP, internalIP);
            manager.StartClient();
        }

    }
}
#endif