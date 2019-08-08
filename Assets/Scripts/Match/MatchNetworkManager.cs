using Mirror;
using UnityEngine;

namespace SpaceCommander.Networking
{
    [AddComponentMenu("Network/MatchNetworkManager")]
    public class MatchNetworkManager : NetworkManager
    {
        public GameObject matchManagerPrefab;
        
        // virtual so that inheriting classes' Awake() can call base.Awake() too
        public override void Awake()
        {
            base.Awake();
        }

        public override void Start()
        {
            base.Start();
        }

        // NetworkIdentity.UNetStaticUpdate is called from UnityEngine while LLAPI network is active.
        // if we want TCP then we need to call it manually. probably best from NetworkManager, although this means
        // that we can't use NetworkServer/NetworkClient without a NetworkManager invoking Update anymore.
        //
        // virtual so that inheriting classes' LateUpdate() can call base.LateUpdate() too
        public override void LateUpdate()
        {
            base.LateUpdate();
        }

        // When pressing Stop in the Editor, Unity keeps threads alive until we
        // press Start again (which might be a Unity bug).
        // Either way, we should disconnect client & server in OnApplicationQuit
        // so they don't keep running until we press Play again.
        // (this is not a problem in builds)
        public override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
        }

        public override void StartHost()
        {
            base.StartHost();
        }

        public override void ServerChangeScene(string newSceneName)
        {
            base.ServerChangeScene(newSceneName);
        }

        // virtual so that inheriting classes' OnDestroy() can call base.OnDestroy() too
        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        #region Server System Callbacks
        public override void OnServerConnect(NetworkConnection conn) {}

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
        }

        public override void OnServerReady(NetworkConnection conn)
        {
            base.OnServerReady(conn);
        }

        public override void OnServerAddPlayer(NetworkConnection conn, AddPlayerMessage extraMessage)
        {
            if (LogFilter.Debug) Debug.Log("NetworkManager.OnServerAddPlayer");

            if (conn.playerController != null)
            {
                Debug.LogError("There is already a player for this connections.");
                return;
            }

            GameObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            NetworkServer.AddPlayerForConnection(conn, player);
        }

        public override void OnServerRemovePlayer(NetworkConnection conn, NetworkIdentity player)
        {
            if (player.gameObject != null)
            {
                NetworkServer.Destroy(player.gameObject);
            }
        }

        public override void OnServerError(NetworkConnection conn, int errorCode) {}

        public override void OnServerSceneChanged(string sceneName) {}
        #endregion

        #region Client System Callbacks
        public override void OnClientConnect(NetworkConnection conn)
        {
            if (!clientLoadedScene)
            {
                // Ready/AddPlayer is usually triggered by a scene load completing. if no scene was loaded, then Ready/AddPlayer it here instead.
                ClientScene.Ready(conn);
                if (autoCreatePlayer)
                {
                    ClientScene.AddPlayer();
                }
            }
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            StopClient();
        }

        public override void OnClientError(NetworkConnection conn, int errorCode) {}

        public override void OnClientNotReady(NetworkConnection conn) {}

        // Called from ClientChangeScene immediately before SceneManager.LoadSceneAsync is executed
        // This allows client to do work / cleanup / prep before the scene changes.
        public override void OnClientChangeScene(string newSceneName) {}

        public override void OnClientSceneChanged(NetworkConnection conn)
        {
            base.OnClientSceneChanged(conn);
            // always become ready.
            ClientScene.Ready(conn);

            // vis2k: replaced all this weird code with something more simple
            if (autoCreatePlayer)
            {
                // add player if existing one is null
                if (ClientScene.localPlayer == null)
                {
                    ClientScene.AddPlayer();
                }
            }
        }
        #endregion

        #region Start & Stop callbacks
        // Since there are multiple versions of StartServer, StartClient and StartHost, to reliably customize
        // their functionality, users would need override all the versions. Instead these callbacks are invoked
        // from all versions, so users only need to implement this one case.

        public override void OnStartHost() {}

        public override void OnStartServer()
        {
            Debug.Log("Server started. Report to server list!");
            GameObject matchManager = Instantiate(matchManagerPrefab);
            NetworkServer.Spawn(matchManager);
        } 
        public override void OnStartClient()
        {
            base.OnStartClient();
        }

        public override void OnStopServer()
        {
            Debug.Log("Let the server know that the server has stopped");
        }
        public override void OnStopClient() {}
        public override void OnStopHost() {}
        #endregion
    }
}
