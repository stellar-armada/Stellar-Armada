using UnityEngine;
using System.Net;
using System.Net.Sockets;
using Mirror;
using System;
namespace SpaceCommander.Networking
{
	
	public class LocalNetworkDiscovery : MonoBehaviour
	{
		
		public class DiscoveryInfo
		{
			private IPEndPoint _endPoint;
            public ServerStatus serverStatusObject;
			private float timeWhenReceived = 0f;
			public DiscoveryInfo (IPEndPoint endPoint, ServerStatus newServerStatusObject)
			{
				_endPoint = endPoint;
                serverStatusObject = newServerStatusObject;
                timeWhenReceived = Time.realtimeSinceStartup;
			}
			public IPEndPoint EndPoint { get { return this._endPoint; } }
			public float TimeSinceReceived { get { return Time.realtimeSinceStartup - this.timeWhenReceived; } }
		}

		public static event System.Action<DiscoveryInfo> EventOnReceivedServerResponse = delegate {};


		public static LocalNetworkDiscovery singleton { get ; private set ; }

		int m_serverPort = 8885;
		static UdpClient m_serverUdpCl = null;
		static UdpClient m_clientUdpCl = null;
		
		static string m_signature = null;

		static bool IsServerActive { get { return NetworkServer.active; } }

		private void OnEnable()
        {
            StartCoroutine(ClientCoroutine());
            StartCoroutine(ServerCoroutine());
        }

        void OnDisable()
        {
            StopAllCoroutines();
            ShutdownUdpClients();

        }


        void Awake ()
		{
			if (singleton != null)
				return;

			singleton = this;

		}
        
		void Update ()
		{

			if(IsServerActive)
			{
				// make sure server's UDP client is created
				EnsureServerIsInitialized();
			}
			else
			{
				// we should close server's UDP client
				CloseServerUdpClient();
			}

		}


		static void EnsureServerIsInitialized()
		{
			if (m_serverUdpCl != null)
				return;

			m_serverUdpCl = new UdpClient (singleton.m_serverPort);
			m_serverUdpCl.EnableBroadcast = true;
			m_serverUdpCl.MulticastLoopback = false;

		}

		static void EnsureClientIsInitialized()
		{

			if (m_clientUdpCl != null)
				return;

			m_clientUdpCl = new UdpClient (0);
			m_clientUdpCl.EnableBroadcast = true;
			// turn off receiving from our IP
			m_clientUdpCl.MulticastLoopback = false;

		}

		static void ShutdownUdpClients()
		{
			CloseServerUdpClient();
			CloseClientUdpClient();
		}

		static void CloseServerUdpClient()
		{
			if (m_serverUdpCl != null) {
				m_serverUdpCl.Close ();
				m_serverUdpCl = null;
			}
		}

		static void CloseClientUdpClient()
		{
			if (m_clientUdpCl != null) {
				m_clientUdpCl.Close ();
				m_clientUdpCl = null;
			}
		}


		static DiscoveryInfo ReadDataFromUdpClient(UdpClient udpClient)
		{
			
			// only proceed if there is available data in network buffer, or otherwise Receive() will block
			// average time for UdpClient.Available : 10 us
			if (udpClient.Available <= 0)
				return null;
			
			IPEndPoint remoteEP = new IPEndPoint (IPAddress.Any, 0);
			byte[] receivedBytes = udpClient.Receive (ref remoteEP);

			if (remoteEP != null && receivedBytes != null && receivedBytes.Length > 0) {


                ServerStatus serverStatusObj = ServerStatus.Deserialize(receivedBytes);

				return new DiscoveryInfo(remoteEP, serverStatusObj);
			}

			return null;
		}

		static System.Collections.IEnumerator ServerCoroutine()
		{

			while (true)
			{

				yield return new WaitForSecondsRealtime (0.3f);

				if (null == m_serverUdpCl)
					continue;

				if(!IsServerActive)
					continue;
				
					var info = ReadDataFromUdpClient(m_serverUdpCl);
					if(info != null)
						OnReceivedBroadcast(info);
			}

		}

		static void OnReceivedBroadcast(DiscoveryInfo info)
		{
            if(info.serverStatusObject.signature == GetSignature())
            {

	            IPAddress[] ipv4Addresses = Array.FindAll(  Dns.GetHostEntry(string.Empty).AddressList, a => a.AddressFamily == AddressFamily.InterNetwork);
	            IPAddress addr = ipv4Addresses[0];
	            ServerStatus serverStatusObject = new ServerStatus(addr.ToString(), addr.ToString(), "Local Game", (ushort)NetworkServer.connections.Count, (ushort)NetworkManager.singleton.maxConnections, ServerStatus.ConnectionType.Local);



serverStatusObject.signature = GetSignature();
                serverStatusObject.port = 7777;

                byte[] bytes = ServerStatus.Serialize(serverStatusObject);
				m_serverUdpCl.Send( bytes, bytes.Length, info.EndPoint );
			}
		}

		static System.Collections.IEnumerator ClientCoroutine()
		{

			while (true)
			{
				yield return new WaitForSecondsRealtime (0.3f);

				if (null == m_clientUdpCl)
					continue;


					var info = ReadDataFromUdpClient (m_clientUdpCl);
					if (info != null)
						OnReceivedServerResponse(info);
				
			}

		}

		public static void SendBroadcast()
		{
			EnsureClientIsInitialized();

			if (null == m_clientUdpCl)
				return;
			
            ServerStatus serverStatusObj = new ServerStatus(GetSignature());
            
			IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, singleton.m_serverPort);

			byte[] bytes = ServerStatus.Serialize(serverStatusObj);

			try {
				m_clientUdpCl.Send (bytes, bytes.Length, endPoint);
			} catch(SocketException ex) {
				if(ex.ErrorCode == 10051) {
                    // Network is unreachable
                    // ignore this error
                    Debug.Log("Network is unreachable?");
				} else {
					throw;
				}
			}

		}
        
		static void OnReceivedServerResponse(DiscoveryInfo info) {

			// invoke event
			EventOnReceivedServerResponse(info);

		}

		/// Signature identifies this game among others.
		public static string GetSignature()
		{
			if (m_signature != null)
				return m_signature;
			
			string[] strings = new string[]{Application.productName, Application.version};

			m_signature = "";

			foreach(string str in strings)
			{
				// only use it's hash code
				m_signature += str.GetHashCode() + ".";
			}

			return m_signature;
		}
        
	}

}
