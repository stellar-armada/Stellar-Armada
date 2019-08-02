using Mirror;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceCommander.Networking
{

    public class NetworkDiscoveryController : MonoBehaviour
    {
        public static NetworkDiscoveryController instance;

        [Header("Listen Server Connection")]
        public string listServerIp = "127.0.0.1";
        public ushort gameServerToListenPort = 8887;
        public ushort clientToListenPort = 8888;
        public string gameServerTitle = "Deathmatch";

        Telepathy.Client gameServerToListenConnection = new Telepathy.Client();
        Telepathy.Client clientToListenConnection = new Telepathy.Client();

        int connectingDots = 0;

        Button currentButton;

        private bool useInternetConnection;

        Dictionary<string, ServerStatus> serverlist = new Dictionary<string, ServerStatus>();

        List<ServerStatus> localServers = new List<ServerStatus>();

        private void Awake()
        {
            instance = this;
        }

        void OnEnable()
        {
            LocalNetworkDiscovery.EventOnReceivedServerResponse += OnDiscoveredServer;

        }

        void OnDisable()
        {
            LocalNetworkDiscovery.EventOnReceivedServerResponse -= OnDiscoveredServer;
        }

        void RefreshLocalServers()
        {
            foreach(ServerStatus status in localServers.ToList())
            {
                if (status.timeStamp < Time.deltaTime - 10f)
                {
                    localServers.Remove(status); // remove old entries
                    serverlist.Remove(status.ip);
                }
            }

            LocalNetworkDiscovery.SendBroadcast();
        }

        void OnDiscoveredServer(LocalNetworkDiscovery.DiscoveryInfo info)
        {

            if (info.serverStatusObject.signature != LocalNetworkDiscovery.GetSignature())
            {
                return;
            }

            if (localServers.Any(server => server.ip == info.serverStatusObject.ip))
                {
                localServers.First(server => server.ip == info.serverStatusObject.ip).timeStamp = Time.time; // refresh time
                }
                else
                {

                    localServers.Add(info.serverStatusObject);
                    serverlist.Add(info.serverStatusObject.ip, info.serverStatusObject);
                  
                }

        }

        private void OnDestroy()
        {
            if (instance == this) instance = null;
        }

        void Start()
        {

            InvokeRepeating(nameof(Tick), 0, 1);
        }

        bool IsConnecting() => NetworkClient.active && !ClientScene.ready;
        bool FullyConnected() => NetworkClient.active && ClientScene.ready;

        // should we use the client to listen connection?
        bool UseClientToListen()
        {
            return (!NetworkManager.IsHeadless() && !NetworkServer.active && !FullyConnected() && useInternetConnection);
        }

        // should we use the game server to listen connection?
        bool UseGameServerToListen()
        {
            return (NetworkServer.active && useInternetConnection);
        }

        void Tick()
        {
            TickGameServer();
            TickClient();


        }

        // send server status to list server
        void SendStatus()
        {
            BinaryWriter writer = new BinaryWriter(new MemoryStream());

            // create message
            // NOTE: you can send anything that you want, as long as you also
            //       receive it in ParseMessage

            IPAddress[] ipv4Addresses = Array.FindAll(Dns.GetHostEntry(string.Empty).AddressList, a => a.AddressFamily == AddressFamily.InterNetwork);
            IPAddress addr = ipv4Addresses[0];
            char[] ipAddress = addr.ToString().ToCharArray();

            char[] titleChars = gameServerTitle.ToCharArray();
            writer.Write((ushort)titleChars.Length);
            writer.Write(titleChars);
            writer.Write((ushort)NetworkServer.connections.Count);
            writer.Write((ushort)NetworkManager.singleton.maxConnections);
            writer.Write((ushort)NetworkManager.singleton.maxConnections);
            writer.Write((ushort)ipAddress.Length);
            writer.Write(ipAddress);

            // send it
            writer.Flush();
            gameServerToListenConnection.Send(((MemoryStream)writer.BaseStream).ToArray());
        }

        void TickGameServer()
        {
            // send server data to listen
            if (UseGameServerToListen())
            {
                // connected yet?
                if (gameServerToListenConnection.Connected)
                {
                    SendStatus();
                }
                // otherwise try to connect
                // (we may have just started the game)
                else if (!gameServerToListenConnection.Connecting)
                {
                    Debug.Log("Establishing game server to listen connection...");
                    gameServerToListenConnection.Connect(listServerIp, gameServerToListenPort);
                }
            }
            // shouldn't use game server, but still using it?
            else if (gameServerToListenConnection.Connected)
            {
                gameServerToListenConnection.Disconnect();
            }
        }

        void ParseMessage(byte[] bytes)
        {

            // use binary reader because our NetworkReader uses custom string reading with bools
            // => we don't use ReadString here because the listen server doesn't
            //    know C#'s '7-bit-length + utf8' encoding for strings
            BinaryReader reader = new BinaryReader(new MemoryStream(bytes, false), Encoding.UTF8);
            ushort ipLength = reader.ReadUInt16();
            string ip = new string(reader.ReadChars(ipLength));
            //ushort port = reader.ReadUInt16(); <- not all Transports use a port. assume default.
            ushort titleLength = reader.ReadUInt16();
            string title = new string(reader.ReadChars(titleLength));
            ushort players = reader.ReadUInt16();
            ushort capacity = reader.ReadUInt16();

            ushort localIPLength = reader.ReadUInt16();
            string localIP = new string(reader.ReadChars(localIPLength));
            //Debug.Log("PARSED: ip=" + ip + /*" port=" + port +*/ " title=" + title + " players=" + players + " capacity= " + capacity);

            // build key
            string key = ip;

            // find existing or create new one
            ServerStatus server;
            if (serverlist.TryGetValue(key, out server))
            {
                // refresh
                server.title = title;
                server.players = players;
                server.capacity = capacity;
                server.connectionType = ServerStatus.ConnectionType.Internet;
                server.localip = localIP;
            }
            else
            {
                // create
                server = new ServerStatus(ip, localIP, title, players, capacity, ServerStatus.ConnectionType.Internet);
            }

            // save
            serverlist[key] = server;
        }



        void TickClient()
        {
            
            RefreshLocalServers();
            
            // receive client data from listen
            if (UseClientToListen())
            {
                // connected yet?
                if (clientToListenConnection.Connected)
                {

                    // receive latest game server info
                    while (clientToListenConnection.GetNextMessage(out Telepathy.Message message))
                    {
                        // data message?
                        if (message.eventType == Telepathy.EventType.Data)
                        {
                            ParseMessage(message.data);

                        }
                    }

                    // ping again if previous ping finished
                    foreach (ServerStatus server in serverlist.Values)
                    {
                        if (server.ping == null)
                        {
                            server.ping = new Ping(server.ip);

                        }
                        else if (server.ping.isDone)
                        {
                            server.lastLatency = server.ping.time;
                            server.ping = new Ping(server.ip);
                        }
                    }
                }
                // otherwise try to connect
                // (we may have just joined the menu/disconnect from game server)
                else if (!clientToListenConnection.Connecting)
                {
                    Debug.Log("Establishing client to listen connection...");
                    clientToListenConnection.Connect(listServerIp, clientToListenPort);
                }
            }
            // shouldn't use client, but still using it? (e.g. after joining)
            else if (clientToListenConnection.Connected)
            {
                clientToListenConnection.Disconnect();
                serverlist.Clear();
            }

            if (NetworkDiscoveryUIController.instance == null) return; // Our UI controller doesn't exist yet, so don't do UI

            // refresh UI afterwards
            OnUI();
        }

        // instantiate/remove enough prefabs to match amount
        public static void BalancePrefabs(GameObject prefab, int amount, Transform parent)
        {
            // instantiate until amount
            for (int i = parent.childCount; i < amount; ++i)
            {
                GameObject go = GameObject.Instantiate(prefab);
                go.transform.SetParent(parent, false);
            }

            // delete everything that's too much
            // (backwards loop because Destroy changes childCount)
            for (int i = parent.childCount - 1; i >= amount; --i)
                GameObject.Destroy(parent.GetChild(i).gameObject);
        }

        public void Join()
        {
            NetworkManager.singleton.StartClient();
        }

        void OnUI()
        {
            // only show while client not connected and server not started
            if (!NetworkManager.singleton.isNetworkActive)
            {
                // mainPanel.SetActive(true);

                // instantiate/destroy enough slots
                BalancePrefabs(NetworkDiscoveryUIController.instance.slotPrefab.gameObject, serverlist.Values.Count, NetworkDiscoveryUIController.instance.content);

                if (currentButton == null) NetworkDiscoveryUIController.instance.joinButton.interactable = false;

                // refresh all members
                for (int i = 0; i < serverlist.Values.Count; ++i)
                {
                    ServerStatusSlot slot = NetworkDiscoveryUIController.instance.content.GetChild(i).GetComponent<ServerStatusSlot>();
                    ServerStatus server = serverlist.Values.ToList()[i];

                    if (server.ip == server.localip && server.connectionType == ServerStatus.ConnectionType.Internet) continue; // Hide internet entries if they match a local IP entry, since user can't join by internet IP

                    slot.serverNameText.text = server.title;
                    if (server.connectionType == ServerStatus.ConnectionType.Local)
                    {
                        slot.serverNameText.text += "<color=#00ffffff> (LAN)</color>";
                        slot.transform.SetAsFirstSibling();
                    }

                    slot.playersText.text = server.players + "/" + server.capacity;

                    slot.latencyText.text = server.lastLatency != -1 ? server.lastLatency.ToString() : "...";
                    slot.addressText.text = server.ip;
                    slot.localAddress = server.localip;

                    slot.selectObjectButton.interactable = !IsConnecting();
                    slot.selectObjectButton.gameObject.SetActive(server.players < server.capacity);
                    slot.selectObjectButton.onClick.RemoveAllListeners();

                    if (NetworkManager.singleton.networkAddress == server.ip)
                    {
                        slot.selectObjectButton.interactable = false;
                    }

                    slot.selectObjectButton.onClick.AddListener(() =>
                    {


                        NetworkManager.singleton.networkAddress = server.ip;

                        if (currentButton != null)
                        {
                            currentButton.interactable = true;
                        }
                        slot.selectObjectButton.interactable = false;
                        currentButton = slot.selectObjectButton;
                        NetworkDiscoveryUIController.instance.joinButton.interactable = true;
                    });
                }

                // server buttons
                NetworkDiscoveryUIController.instance.serverAndPlayButton.interactable = !IsConnecting();
                NetworkDiscoveryUIController.instance.serverAndPlayButton.onClick.RemoveAllListeners();
                NetworkDiscoveryUIController.instance.serverAndPlayButton.onClick.AddListener(() =>
                {
                    Debug.Log("Host button called");

                });

                NetworkDiscoveryUIController.instance.joinButton.onClick.RemoveAllListeners();
                NetworkDiscoveryUIController.instance.joinButton.onClick.AddListener(() =>
                {
                    NetworkManager.singleton.StartClient();
                    NetworkDiscoveryUIController.instance.joinButton.interactable = false;
                });
            }

            // show connecting panel while connecting
            if (IsConnecting())
            {
                NetworkDiscoveryUIController.instance.connectingPanel.SetActive(true);

                // . => .. => ... => ....
                connectingDots = ((connectingDots + 1) % 4);
                NetworkDiscoveryUIController.instance.connectingText.text = "Connecting" + new string('.', connectingDots);

                // cancel button
                NetworkDiscoveryUIController.instance.connectingCancelButton.onClick.RemoveAllListeners();
                NetworkDiscoveryUIController.instance.connectingCancelButton.onClick.AddListener(NetworkManager.singleton.StopClient);
            }
            else NetworkDiscoveryUIController.instance.connectingPanel.SetActive(false);
        }

        // disconnect everything when pressing Stop in the Editor
        void OnApplicationQuit()
        {
            if (gameServerToListenConnection.Connected)
                gameServerToListenConnection.Disconnect();
            if (clientToListenConnection.Connected)
                clientToListenConnection.Disconnect();
        }
    }
}