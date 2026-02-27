using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Net;
using TMPro;

public class CustomNetworkMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject menuPanel; 
    [SerializeField] private TMP_InputField ipAddressInput;
    [SerializeField] private TMP_InputField portInput;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button serverButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Network Manager")]
    [SerializeField] private NetworkManager networkManager;

    private void Start()
    {
        // 确保 NetworkManager 存在
        if (networkManager == null)
            networkManager = FindObjectOfType<NetworkManager>();



        // 添加按钮事件监听
        hostButton.onClick.AddListener(StartHost);
        //serverButton.onClick.AddListener(StartServer);
        clientButton.onClick.AddListener(StartClient);
        stopButton.onClick.AddListener(StopConnection);

        // 初始化 UI
        UpdateStatus("Ready");
        ipAddressInput.text = GetLocalIPAddress();

        if (Transport.active is PortTransport portTransport)
        {
            portInput.text = portTransport.Port.ToString();
        }
        // 监听 Mirror 事件（新的事件系统）
        NetworkClient.OnConnectedEvent += OnClientConnected;
        NetworkClient.OnDisconnectedEvent += OnClientDisconnected;
        //NetworkServer.OnStartedEvent += OnServerStarted;
    }

    private void OnDestroy()
    {
        // 移除事件监听
        NetworkClient.OnConnectedEvent -= OnClientConnected;
        NetworkClient.OnDisconnectedEvent -= OnClientDisconnected;
        //NetworkServer.OnStartedEvent -= OnServerStarted;
    }

    // 获取本地 IP 地址
    private string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return "127.0.0.1";
    }

    // 获取用户使用的端口号
    private void GetPort()
    {
        if (Transport.active is PortTransport portTransport)
        {
            if (!string.IsNullOrEmpty(portInput.text))
                portTransport.Port = ushort.Parse(portInput.text);
        }
    }

    // 启动主机（服务器+客户端）
    public void StartHost()
    {
        GetPort();
        networkManager.StartHost();
        menuPanel.SetActive(false);
        UpdateStatus("Hosting...");

        
    }

    // 仅启动服务器
    public void StartServer()
    {
        GetPort();
        networkManager.StartServer();
        menuPanel.SetActive(false);
        UpdateStatus("Server Running...");
    }

    // 启动客户端并连接
    public void StartClient()
    {
        if (!string.IsNullOrEmpty(ipAddressInput.text))
            networkManager.networkAddress = ipAddressInput.text;

        GetPort();
        networkManager.StartClient();
        menuPanel.SetActive(false);
        UpdateStatus("Connecting...");
    }

    // 停止所有连接
    public void StopConnection()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            networkManager.StopHost();
        }
        else if (NetworkServer.active)
        {
            networkManager.StopServer();
        }
        else if (NetworkClient.isConnected)
        {
            networkManager.StopClient();
        }

        menuPanel.SetActive(true);
        UpdateStatus("Disconnected");
    }

    // 更新状态文本
    private void UpdateStatus(string message)
    {
        if (statusText != null)
            statusText.text = $"Status: {message}";
    }

    // Mirror 事件回调
    private void OnClientConnected()
    {
        UpdateStatus("Connected as Client");
    }

    private void OnClientDisconnected()
    {
        menuPanel.SetActive(true);
        UpdateStatus("Disconnected");
    }

    private void OnServerStarted()
    {
        UpdateStatus("Server Started");
    }

    public void OverGame()
    {
        Application.Quit();
    }
}