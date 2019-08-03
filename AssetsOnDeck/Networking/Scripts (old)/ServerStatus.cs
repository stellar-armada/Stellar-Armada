using UnityEngine;

public class ServerStatus
{

    public enum ConnectionType { Local = 0, Internet = 1 };



    public string signature;
    public string ip;
    public string localip;
    public ushort port; // <- not all transports use a port. assume default port. feel free to also send a port if needed.
    public string title;
    public ushort players;
    public ushort capacity;
    public ConnectionType connectionType;

    public float timeStamp;

    public int lastLatency = -1;
    public Ping ping;

    public ServerStatus(string signature)
    {
        this.signature = signature;
    }

    public ServerStatus(string ip, string localip, /*ushort port,*/ string title, ushort players, ushort capacity, ConnectionType connectionType)
    {
        this.ip = ip;
        this.localip = localip;
        this.title = title;
        this.players = players;
        this.capacity = capacity;
        this.connectionType = connectionType;
        ping = new Ping(ip);
        timeStamp = Time.time;
        Debug.Log("Address of server status message: " + this.ip);
    }

    public static byte[] Serialize(ServerStatus obj)
    {
        string json = JsonUtility.ToJson(obj);
        return System.Text.Encoding.UTF8.GetBytes(json);
    }

    public static ServerStatus Deserialize(byte[] bytes)
    {
        ServerStatus obj = JsonUtility.FromJson<ServerStatus>(System.Text.Encoding.UTF8.GetString(bytes));
        return obj;
    }

}

