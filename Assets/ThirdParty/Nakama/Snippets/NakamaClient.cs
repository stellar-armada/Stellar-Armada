using System.Text;
using UnityEngine;
using Nakama;
using UnityEngine.UI;

public class NakamaClient : MonoBehaviour
{
    private IClient _client;

    [SerializeField] private InputField playerNameInput;
    [SerializeField] private Button Play;
    [SerializeField] private Text debugStatusArea;
    
    [SerializeField] private string ipAddress = "192.168.1.237";
    [SerializeField] private int port = 7350;

    private string _playerNameKey = "PlayerName";

    void Awake()
    {
        _client = new Client("http", ipAddress, port, "defaultkey");
        Play.onClick.AddListener(Connect);
        if (PlayerPrefs.HasKey(_playerNameKey))
            playerNameInput.text = PlayerPrefs.GetString(_playerNameKey);
    }

    async void Connect()
    {
        // Get the unique ID of this device
        var deviceid = SystemInfo.deviceUniqueIdentifier;
        string userName = playerNameInput.text;
        PlayerPrefs.SetString(_playerNameKey, userName);

        var session = await _client.AuthenticateDeviceAsync(deviceid, userName);

        var account = await _client.GetAccountAsync(session);
        StringBuilder sb = new StringBuilder();
        // Account properties.
        sb.AppendLine("Account devices: " + string.Join(",", account.Devices));
        sb.AppendLine("Account custom id: " + account.CustomId);
        // sb.AppendLine("Account email:" + account.Email);
        // sb.AppendLine("Account verify time: " + account.VerifyTime);
        sb.AppendLine("Account wallet: " + account.Wallet);

        // User properties.
        sb.AppendLine("User id: " + account.User.Id);
        sb.AppendLine("User metadata: " + account.User.Metadata);
        sb.AppendLine("User username: " + account.User.Username);
        sb.AppendLine("User online: " + account.User.Online);

        // var result = await _client.GetUsersAsync(session, new[] {session.UserId});
        // sb.AppendLine("Users: " + string.Join(",\n", result.Users));

        debugStatusArea.text = sb.ToString();
    }
}