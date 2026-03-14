using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using CarSimulator.Vehicle;

namespace CarSimulator.Multiplayer
{
    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager Instance { get; private set; }

        [Header("Network Settings")]
        [SerializeField] private string m_serverAddress = "127.0.0.1";
        [SerializeField] private int m_serverPort = 8888;
        [SerializeField] private bool m_isServer;
        [SerializeField] private string m_playerName = "Player";

        [Header("Connection UI")]
        [SerializeField] private GameObject m_connectionPanel;
        [SerializeField] private InputField m_addressInput;
        [SerializeField] private InputField m_portInput;
        [SerializeField] private InputField m_nameInput;
        [SerializeField] private Text m_statusText;
        [SerializeField] private Button m_hostButton;
        [SerializeField] private Button m_joinButton;
        [SerializeField] private Button m_disconnectButton;

        [Header("Player Prefab")]
        [SerializeField] private GameObject m_networkedPlayerPrefab;

        private TcpClient m_client;
        private NetworkStream m_stream;
        private Thread m_receiveThread;
        private bool m_isConnected;
        private int m_playerId;
        private List<NetworkedPlayer> m_players = new List<NetworkedPlayer>();
        private Dictionary<int, PlayerData> m_playerData = new Dictionary<int, PlayerData>();

        public bool IsConnected => m_isConnected;
        public int PlayerId => m_playerId;
        public string PlayerName => m_playerName;

        private struct PlayerData
        {
            public int id;
            public string name;
            public Vector3 position;
            public Quaternion rotation;
            public float speed;
            public int gear;
            public float rpm;
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (m_addressInput != null) m_addressInput.text = m_serverAddress;
            if (m_portInput != null) m_portInput.text = m_serverPort.ToString();
            if (m_nameInput != null) m_nameInput.text = m_playerName;
        }

        public void StartHost()
        {
            m_isServer = true;
            m_serverAddress = "127.0.0.1";
            ConnectToServer();
        }

        public void StartClient()
        {
            m_isServer = false;
            if (m_addressInput != null) m_serverAddress = m_addressInput.text;
            if (m_portInput != null) int.TryParse(m_portInput.text, out m_serverPort);
            if (m_nameInput != null) m_playerName = m_nameInput.text;
            ConnectToServer();
        }

        private void ConnectToServer()
        {
            try
            {
                m_client = new TcpClient(m_serverAddress, m_serverPort);
                m_stream = m_client.GetStream();
                m_isConnected = true;

                m_receiveThread = new Thread(ReceiveData);
                m_receiveThread.IsBackground = true;
                m_receiveThread.Start();

                UpdateStatus("Connected!");
                if (m_connectionPanel != null) m_connectionPanel.SetActive(false);

                SendPlayerInfo(m_playerName);

                Debug.Log($"[Network] Connected to {m_serverAddress}:{m_serverPort}");
            }
            catch (System.Exception e)
            {
                UpdateStatus($"Connection failed: {e.Message}");
                Debug.LogError($"[Network] Connection error: {e.Message}");
            }
        }

        public void Disconnect()
        {
            m_isConnected = false;

            if (m_receiveThread != null)
                m_receiveThread.Abort();

            if (m_stream != null)
                m_stream.Close();

            if (m_client != null)
                m_client.Close();

            UpdateStatus("Disconnected");
            if (m_connectionPanel != null) m_connectionPanel.SetActive(true);

            foreach (var player in m_players)
            {
                if (player != null)
                    Destroy(player.gameObject);
            }
            m_players.Clear();
            m_playerData.Clear();
        }

        private void ReceiveData()
        {
            byte[] buffer = new byte[4096];

            while (m_isConnected)
            {
                try
                {
                    if (m_stream.DataAvailable)
                    {
                        int bytesRead = m_stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead > 0)
                        {
                            string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            ProcessMessage(data);
                        }
                    }
                    Thread.Sleep(10);
                }
                catch
                {
                    break;
                }
            }
        }

        private void ProcessMessage(string data)
        {
            string[] parts = data.Split('|');

            switch (parts[0])
            {
                case "WELCOME":
                    if (parts.Length > 1)
                    {
                        int.TryParse(parts[1], out m_playerId);
                        Debug.Log($"[Network] Assigned ID: {m_playerId}");
                    }
                    break;

                case "PLAYER_JOIN":
                    if (parts.Length >= 3)
                    {
                        int id = int.Parse(parts[1]);
                        string name = parts[2];
                        if (id != m_playerId)
                        {
                            SpawnNetworkedPlayer(id, name);
                        }
                    }
                    break;

                case "PLAYER_LEAVE":
                    if (parts.Length > 1)
                    {
                        int id = int.Parse(parts[1]);
                        RemovePlayer(id);
                    }
                    break;

                case "PLAYER_UPDATE":
                    if (parts.Length >= 7)
                    {
                        int id = int.Parse(parts[1]);
                        float x = float.Parse(parts[2]);
                        float y = float.Parse(parts[3]);
                        float z = float.Parse(parts[4]);
                        float rx = float.Parse(parts[5]);
                        float ry = float.Parse(parts[6]);
                        float rz = float.Parse(parts[7]);
                        float speed = parts.Length > 8 ? float.Parse(parts[8]) : 0;
                        int gear = parts.Length > 9 ? int.Parse(parts[9]) : 0;

                        UpdatePlayerData(id, new Vector3(x, y, z), Quaternion.Euler(rx, ry, rz), speed, gear);
                    }
                    break;

                case "PLAYER_LIST":
                    for (int i = 1; i < parts.Length; i += 3)
                    {
                        int id = int.Parse(parts[i]);
                        string name = parts[i + 1];
                        if (id != m_playerId)
                        {
                            SpawnNetworkedPlayer(id, name);
                        }
                    }
                    break;
            }
        }

        private void SendPlayerInfo(string name)
        {
            string msg = $"JOIN|{m_playerId}|{name}";
            SendMessage(msg);
        }

        public void SendPlayerUpdate(Vector3 position, Quaternion rotation, float speed, int gear)
        {
            if (!m_isConnected) return;

            string msg = string.Format("UPDATE|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}",
                m_playerId,
                position.x.ToString("F2"),
                position.y.ToString("F2"),
                position.z.ToString("F2"),
                rotation.eulerAngles.x.ToString("F2"),
                rotation.eulerAngles.y.ToString("F2"),
                rotation.eulerAngles.z.ToString("F2"),
                speed.ToString("F1"),
                gear);

            SendMessage(msg);
        }

        private void SendMessage(string message)
        {
            if (!m_isConnected || m_stream == null) return;

            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                m_stream.Write(data, 0, data.Length);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[Network] Send error: {e.Message}");
                Disconnect();
            }
        }

        private void SpawnNetworkedPlayer(int id, string name)
        {
            if (m_playerData.ContainsKey(id)) return;

            if (m_networkedPlayerPrefab != null)
            {
                GameObject player = Instantiate(m_networkedPlayerPrefab, Vector3.zero, Quaternion.identity);
                NetworkedPlayer np = player.GetComponent<NetworkedPlayer>();
                if (np != null)
                {
                    np.Initialize(id, name);
                    m_players.Add(np);
                    m_playerData[id] = new PlayerData { id = id, name = name };
                }
            }
        }

        private void RemovePlayer(int id)
        {
            NetworkedPlayer player = m_players.Find(p => p.PlayerId == id);
            if (player != null)
            {
                m_players.Remove(player);
                Destroy(player.gameObject);
            }
            m_playerData.Remove(id);
        }

        private void UpdatePlayerData(int id, Vector3 pos, Quaternion rot, float speed, int gear)
        {
            if (!m_playerData.ContainsKey(id)) return;

            m_playerData[id].position = pos;
            m_playerData[id].rotation = rot;
            m_playerData[id].speed = speed;
            m_playerData[id].gear = gear;

            NetworkedPlayer player = m_players.Find(p => p.PlayerId == id);
            if (player != null)
            {
                player.UpdateState(pos, rot, speed, gear);
            }
        }

        private void Update()
        {
            if (m_isConnected)
            {
                var spawner = FindObjectOfType<VehicleSpawner>();
                if (spawner?.CurrentVehicle != null)
                {
                    var vehicle = spawner.CurrentVehicle;
                    var physics = vehicle.GetComponent<VehiclePhysics>();
                    var gear = vehicle.GetComponent<GearSystem>();

                    SendPlayerUpdate(
                        vehicle.transform.position,
                        vehicle.transform.rotation,
                        physics?.CurrentSpeed ?? 0,
                        gear?.CurrentGear ?? 0
                    );
                }
            }
        }

        private void UpdateStatus(string status)
        {
            if (m_statusText != null)
                m_statusText.text = status;
        }

        public List<NetworkedPlayer> GetPlayers() => m_players;
    }
}
