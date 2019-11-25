using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using Mirror;
using UnityEngine.Serialization;

namespace NobleConnect.Mirror
{
    public class LobbyReadyToBeginMessage : MessageBase
    {
        public byte slotId;
        public bool readyState;

        public override void Deserialize(NetworkReader reader)
        {
            slotId = reader.ReadByte();
            readyState = reader.ReadBoolean();
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(slotId);
            writer.Write(readyState);


        }
    }

    [AddComponentMenu("Network/NobleNetworkLobbyManager")]
    public class NobleNetworkLobbyManager : NobleNetworkManager
    {
        public struct PendingPlayer
        {
            public NetworkConnection conn;
            public GameObject lobbyPlayer;
        }

        // configuration
        [Header("Lobby Settings")]
        [FormerlySerializedAs("m_ShowLobbyGUI")] [SerializeField] internal bool showLobbyGUI = true;
        [FormerlySerializedAs("m_MinPlayers")] [SerializeField] int minPlayers = 1;
        [FormerlySerializedAs("m_LobbyPlayerPrefab")] [SerializeField] NobleLobbyPlayer lobbyPlayerPrefab;

        [Scene]
        public string LobbyScene;

        [Scene]
        public string GameplayScene;

        // runtime data
        [FormerlySerializedAs("m_PendingPlayers")] public List<PendingPlayer> pendingPlayers = new List<PendingPlayer>();
        public List<NobleLobbyPlayer> lobbySlots = new List<NobleLobbyPlayer>();

        public bool allPlayersReady;

        public override void OnValidate()
        {
            // always >= 0
            maxConnections = Mathf.Max(maxConnections, 0);

            // always <= maxConnections
            minPlayers = Mathf.Min(minPlayers, maxConnections);

            // always >= 0
            minPlayers = Mathf.Max(minPlayers, 0);

            if (lobbyPlayerPrefab != null)
            {
                NetworkIdentity identity = lobbyPlayerPrefab.GetComponent<NetworkIdentity>();
                if (identity == null)
                {
                    lobbyPlayerPrefab = null;
                    Debug.LogError("LobbyPlayer prefab must have a NetworkIdentity component.");
                }
            }

            base.OnValidate();
        }

        internal void ReadyStatusChanged()
        {
            int CurrentPlayers = 0;
            int ReadyPlayers = 0;

            foreach (NobleLobbyPlayer item in lobbySlots)
            {
                if (item != null)
                {
                    CurrentPlayers++;
                    if (item.ReadyToBegin)
                        ReadyPlayers++;
                }
            }

            if (CurrentPlayers == ReadyPlayers)
                CheckReadyToBegin();
            else
                allPlayersReady = false;
        }

        public override void OnServerReady(NetworkConnection conn)
        {
            if (LogFilter.Debug) Debug.Log("NetworkLobbyManager OnServerReady");
            base.OnServerReady(conn);

            GameObject lobbyPlayer = conn?.playerController?.gameObject;

            // if null or not a lobby player, dont replace it
            if (lobbyPlayer?.GetComponent<NobleLobbyPlayer>() != null)
                SceneLoadedForPlayer(conn, lobbyPlayer);
        }

        void SceneLoadedForPlayer(NetworkConnection conn, GameObject lobbyPlayer)
        {
            if (LogFilter.Debug) Debug.LogFormat("NetworkLobby SceneLoadedForPlayer scene: {0} {1}", SceneManager.GetActiveScene().name, conn);

            if (SceneManager.GetActiveScene().name == LobbyScene)
            {
                // cant be ready in lobby, add to ready list
                PendingPlayer pending;
                pending.conn = conn;
                pending.lobbyPlayer = lobbyPlayer;
                pendingPlayers.Add(pending);
                return;
            }

            GameObject gamePlayer = OnLobbyServerCreateGamePlayer(conn);
            if (gamePlayer == null)
            {
                // get start position from base class
                Transform startPos = GetStartPosition();
                gamePlayer = startPos != null
                    ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
                    : Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
                gamePlayer.name = playerPrefab.name;
            }

            if (!OnLobbyServerSceneLoadedForPlayer(lobbyPlayer, gamePlayer))
                return;

            // replace lobby player with game player
            NetworkServer.ReplacePlayerForConnection(conn, gamePlayer);
        }

        public void CheckReadyToBegin()
        {
            if (SceneManager.GetActiveScene().name != LobbyScene) return;

            if (minPlayers > 0 && NetworkServer.connections.Count(conn => conn.Value != null && conn.Value.playerController.gameObject.GetComponent<NobleLobbyPlayer>().ReadyToBegin) < minPlayers)
            {
                allPlayersReady = false;
                return;
            }

            pendingPlayers.Clear();
            allPlayersReady = true;
            OnLobbyServerPlayersReady();
        }

        void CallOnClientEnterLobby()
        {
            OnLobbyClientEnter();
            foreach (NobleLobbyPlayer player in lobbySlots)
                player?.OnClientEnterLobby();
        }

        void CallOnClientExitLobby()
        {
            OnLobbyClientExit();
            foreach (NobleLobbyPlayer player in lobbySlots)
                player?.OnClientExitLobby();
        }

        #region server handlers

        public override void OnServerConnect(NetworkConnection conn)
        {
            if (numPlayers >= maxConnections)
            {
                conn.Disconnect();
                return;
            }

            // cannot join game in progress
            if (SceneManager.GetActiveScene().name != LobbyScene)
            {
                conn.Disconnect();
                return;
            }

            base.OnServerConnect(conn);
            OnLobbyServerConnect(conn);
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            if (conn.playerController != null)
            {
                NobleLobbyPlayer player = conn.playerController.GetComponent<NobleLobbyPlayer>();

                if (player != null)
                    lobbySlots.Remove(player);
            }

            allPlayersReady = false;

            foreach (NobleLobbyPlayer player in lobbySlots)
            {
                if (player != null)
                    player.GetComponent<NobleLobbyPlayer>().ReadyToBegin = false;
            }

            if (SceneManager.GetActiveScene().name == LobbyScene)
                RecalculateLobbyPlayerIndices();

            base.OnServerDisconnect(conn);
            OnLobbyServerDisconnect(conn);
        }

        public override void OnServerAddPlayer(NetworkConnection conn, AddPlayerMessage extraMessage)
        {
            if (SceneManager.GetActiveScene().name != LobbyScene) return;

            if (lobbySlots.Count == maxConnections) return;

            allPlayersReady = false;

            if (LogFilter.Debug) Debug.LogFormat("NetworkLobbyManager.OnServerAddPlayer playerPrefab:{0}", lobbyPlayerPrefab.name);

            GameObject newLobbyGameObject = OnLobbyServerCreateLobbyPlayer(conn);
            if (newLobbyGameObject == null)
                newLobbyGameObject = (GameObject)Instantiate(lobbyPlayerPrefab.gameObject, Vector3.zero, Quaternion.identity);

            NobleLobbyPlayer newLobbyPlayer = newLobbyGameObject.GetComponent<NobleLobbyPlayer>();

            lobbySlots.Add(newLobbyPlayer);

            RecalculateLobbyPlayerIndices();

            NetworkServer.AddPlayerForConnection(conn, newLobbyGameObject);
        }

        void RecalculateLobbyPlayerIndices()
        {
            if (lobbySlots.Count > 0)
            {
                for (int i = 0; i < lobbySlots.Count; i++)
                {
                    lobbySlots[i].Index = i;
                }
            }
        }

        public override void ServerChangeScene(string sceneName)
        {
            if (sceneName == LobbyScene)
            {
                foreach (NobleLobbyPlayer lobbyPlayer in lobbySlots)
                {
                    if (lobbyPlayer == null) continue;

                    // find the game-player object for this connection, and destroy it
                    NetworkIdentity identity = lobbyPlayer.GetComponent<NetworkIdentity>();

                    NetworkIdentity playerController = identity.connectionToClient.playerController;
                    NetworkServer.Destroy(playerController.gameObject);

                    if (NetworkServer.active)
                    {
                        // re-add the lobby object
                        lobbyPlayer.GetComponent<NobleLobbyPlayer>().ReadyToBegin = false;
                        NetworkServer.ReplacePlayerForConnection(identity.connectionToClient, lobbyPlayer.gameObject);
                    }
                }
            }
            else
            {
                if (dontDestroyOnLoad)
                {
                    foreach (NobleLobbyPlayer lobbyPlayer in lobbySlots)
                    {
                        if (lobbyPlayer != null)
                        {
                            lobbyPlayer.transform.SetParent(null);
                            DontDestroyOnLoad(lobbyPlayer);
                        }
                    }
                }
            }

            base.ServerChangeScene(sceneName);
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            if (sceneName != LobbyScene)
            {
                // call SceneLoadedForPlayer on any players that become ready while we were loading the scene.
                foreach (PendingPlayer pending in pendingPlayers)
                    SceneLoadedForPlayer(pending.conn, pending.lobbyPlayer);

                pendingPlayers.Clear();
            }

            OnLobbyServerSceneChanged(sceneName);
        }

        public override void OnStartServer()
        {
            if (string.IsNullOrEmpty(LobbyScene))
            {
                Debug.LogError("NetworkLobbyManager LobbyScene is empty. Set the LobbyScene in the inspector for the NetworkLobbyMangaer");
                return;
            }

            if (string.IsNullOrEmpty(GameplayScene))
            {
                Debug.LogError("NetworkLobbyManager PlayScene is empty. Set the PlayScene in the inspector for the NetworkLobbyMangaer");
                return;
            }

            base.OnStartServer();
            OnLobbyStartServer();
        }

        public override void OnStartHost()
        {
            base.OnStartHost();
            OnLobbyStartHost();
        }

        public override void OnStopServer()
        {
            lobbySlots.Clear();
            base.OnStopServer();
        }

        public override void OnStopHost()
        {
            OnLobbyStopHost();
        }

        #endregion

        #region client handlers

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (lobbyPlayerPrefab == null || lobbyPlayerPrefab.gameObject == null)
                Debug.LogError("NetworkLobbyManager no LobbyPlayer prefab is registered. Please add a LobbyPlayer prefab.");
            else
                ClientScene.RegisterPrefab(lobbyPlayerPrefab.gameObject);

            if (playerPrefab == null)
                Debug.LogError("NetworkLobbyManager no GamePlayer prefab is registered. Please add a GamePlayer prefab.");
            else
                ClientScene.RegisterPrefab(playerPrefab);

            OnLobbyStartClient();
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            OnLobbyClientConnect(conn);
            CallOnClientEnterLobby();
            base.OnClientConnect(conn);
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            OnLobbyClientDisconnect(conn);
            base.OnClientDisconnect(conn);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            OnLobbyStopClient();
            CallOnClientExitLobby();

            if (!string.IsNullOrEmpty(offlineScene))
            {
                // Move the LobbyManager from the virtual DontDestroyOnLoad scene to the Game scene.
                // This let's it be destroyed when client changes to the Offline scene.
                SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
            }
        }

        public override void OnClientChangeScene(string newSceneName)
        {
            if (LogFilter.Debug) Debug.LogFormat("OnClientChangeScene from {0} to {1}", SceneManager.GetActiveScene().name, newSceneName);

            if (SceneManager.GetActiveScene().name == LobbyScene && newSceneName == GameplayScene && dontDestroyOnLoad && NetworkClient.isConnected)
            {
                GameObject lobbyPlayer = NetworkClient.connection?.playerController?.gameObject;
                if (lobbyPlayer != null)
                {
                    lobbyPlayer.transform.SetParent(null);
                    DontDestroyOnLoad(lobbyPlayer);
                }
                else
                    Debug.LogWarningFormat("OnClientChangeScene: lobbyPlayer is null");
            }
            else
               if (LogFilter.Debug) Debug.LogFormat("OnClientChangeScene {0} {1}", dontDestroyOnLoad, NetworkClient.isConnected);
        }

        public override void OnClientSceneChanged(NetworkConnection conn)
        {
            if (SceneManager.GetActiveScene().name == LobbyScene)
            {
                if (NetworkClient.isConnected)
                    CallOnClientEnterLobby();
            }
            else
                CallOnClientExitLobby();

            base.OnClientSceneChanged(conn);
            OnLobbyClientSceneChanged(conn);
        }

        #endregion

        #region lobby server virtuals

        public virtual void OnLobbyStartHost() { }

        public virtual void OnLobbyStopHost() { }

        public virtual void OnLobbyStartServer() { }

        public virtual void OnLobbyServerConnect(NetworkConnection conn) { }

        public virtual void OnLobbyServerDisconnect(NetworkConnection conn) { }

        public virtual void OnLobbyServerSceneChanged(string sceneName) { }

        public virtual GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn)
        {
            return null;
        }

        public virtual GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn)
        {
            return null;
        }

        // for users to apply settings from their lobby player object to their in-game player object
        public virtual bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
        {
            return true;
        }

        public virtual void OnLobbyServerPlayersReady()
        {
            // all players are readyToBegin, start the game
            ServerChangeScene(GameplayScene);
        }

        #endregion

        #region lobby client virtuals

        public virtual void OnLobbyClientEnter() { }

        public virtual void OnLobbyClientExit() { }

        public virtual void OnLobbyClientConnect(NetworkConnection conn) { }

        public virtual void OnLobbyClientDisconnect(NetworkConnection conn) { }

        public virtual void OnLobbyStartClient() { }

        public virtual void OnLobbyStopClient() { }

        public virtual void OnLobbyClientSceneChanged(NetworkConnection conn) { }

        // for users to handle adding a player failed on the server
        public virtual void OnLobbyClientAddPlayerFailed() { }

        #endregion

        #region optional UI

        public virtual void OnGUI()
        {
            if (!showLobbyGUI)
                return;

            if (SceneManager.GetActiveScene().name != LobbyScene)
                return;

            GUI.Box(new Rect(10f, 180f, 520f, 150f), "PLAYERS");
        }

        #endregion
    }
}