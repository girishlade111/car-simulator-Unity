using UnityEngine;

public class SceneConnector : MonoBehaviour
{
    [Header("Auto-Connect Settings")]
    [SerializeField] private bool m_connectOnStart = true;
    [SerializeField] private bool m_findPlayerCar = true;
    [SerializeField] private bool m_connectCamera = true;
    [SerializeField] private bool m_connectUI = true;
    [SerializeField] private bool m_connectAudio = true;

    [Header("Manual References")]
    [SerializeField] private VehicleController m_playerCar;
    [SerializeField] private FollowCamera m_followCamera;
    [SerializeField] private HUD m_hud;
    [SerializeField] private PauseMenu m_pauseMenu;

    private void Start()
    {
        if (m_connectOnStart)
        {
            ConnectAllSystems();
        }
    }

    [ContextMenu("Connect All Systems")]
    public void ConnectAllSystems()
    {
        ConnectPlayerCar();
        ConnectCamera();
        ConnectUI();
        ConnectAudio();
        ConnectMissionSystem();

        Debug.Log("[SceneConnector] All systems connected!");
    }

    private void ConnectPlayerCar()
    {
        if (m_findPlayerCar && m_playerCar == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                m_playerCar = player.GetComponent<VehicleController>();
            }
        }

        if (m_playerCar != null)
        {
            Debug.Log($"[SceneConnector] Player car connected: {m_playerCar.name}");
        }
        else
        {
            Debug.LogWarning("[SceneConnector] Player car not found! Tag your car 'Player'.");
        }
    }

    private void ConnectCamera()
    {
        if (!m_connectCamera) return;

        if (m_followCamera == null)
        {
            var cameras = FindObjectsOfType<FollowCamera>();
            if (cameras.Length > 0)
            {
                m_followCamera = cameras[0];
            }
        }

        if (m_followCamera != null && m_playerCar != null)
        {
            m_followCamera.SetTarget(m_playerCar.transform);
            Debug.Log("[SceneConnector] Camera connected to player car");
        }
    }

    private void ConnectUI()
    {
        if (!m_connectUI) return;

        if (m_hud == null)
        {
            var hud = FindObjectOfType<HUD>();
            if (hud != null)
            {
                var field = typeof(HUD).GetField("m_vehicle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                field?.SetValue(hud, m_playerCar);
            }
        }

        if (m_pauseMenu == null)
        {
            m_pauseMenu = FindObjectOfType<PauseMenu>();
        }

        Debug.Log("[SceneConnector] UI systems connected");
    }

    private void ConnectAudio()
    {
        if (!m_connectAudio) return;

        if (m_playerCar != null)
        {
            var vehicleAudio = m_playerCar.GetComponent<VehicleAudio>();
            if (vehicleAudio == null)
            {
                vehicleAudio = m_playerCar.gameObject.AddComponent<VehicleAudio>();
                Debug.Log("[SceneConnector] Added VehicleAudio to player car");
            }
        }

        Debug.Log("[SceneConnector] Audio systems connected");
    }

    private void ConnectMissionSystem()
    {
        var missionManager = MissionManager.Instance;
        if (missionManager != null)
        {
            Debug.Log("[SceneConnector] Mission system connected");
        }
    }

    public void SetPlayerCar(VehicleController car)
    {
        m_playerCar = car;
        ConnectCamera();
        ConnectUI();
        ConnectAudio();
    }
}

public class GameInitializer : MonoBehaviour
{
    [Header("Initialization Settings")]
    [SerializeField] private bool m_initializeOnStart = true;
    [SerializeField] private bool m_spawnPlayerIfMissing = true;

    [Header("Player Prefab")]
    [SerializeField] private GameObject m_playerCarPrefab;

    [Header("Camera")]
    [SerializeField] private bool m_setupFollowCamera = true;

    [Header("Environment")]
    [SerializeField] private bool m_spawnEnvironment = false;
    [SerializeField] private SceneSetup m_sceneSetup;

    private void Start()
    {
        if (m_initializeOnStart)
        {
            Initialize();
        }
    }

    [ContextMenu("Initialize Game")]
    public void Initialize()
    {
        EnsurePlayerExists();
        SetupCamera();
        ConnectSystems();
        SetupEnvironment();

        Debug.Log("[GameInitializer] Game initialized!");
    }

    private void EnsurePlayerExists()
    {
        var player = GameObject.FindGameObjectWithTag("Player");

        if (player == null && m_spawnPlayerIfMissing)
        {
            if (m_playerCarPrefab != null)
            {
                player = Instantiate(m_playerCarPrefab, Vector3.zero, Quaternion.identity);
                player.name = "PlayerCar";
                player.tag = "Player";
                Debug.Log("[GameInitializer] Player car spawned from prefab");
            }
            else
            {
                Debug.LogWarning("[GameInitializer] No player car in scene and no prefab assigned!");
                CreatePlaceholderPlayer();
            }
        }
    }

    private void CreatePlaceholderPlayer()
    {
        GameObject player = new GameObject("PlayerCar");
        player.tag = "Player";

        player.AddComponent<Rigidbody>();
        player.AddComponent<VehicleController>();
        player.AddComponent<VehicleAudio>();

        player.transform.position = new Vector3(0, 1, 0);
        Debug.Log("[GameInitializer] Created placeholder player car");
    }

    private void SetupCamera()
    {
        if (!m_setupFollowCamera) return;

        Camera mainCam = Camera.main;
        if (mainCam == null) return;

        var followCam = mainCam.GetComponent<FollowCamera>();
        if (followCam == null)
        {
            followCam = mainCam.gameObject.AddComponent<FollowCamera>();
        }

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            followCam.SetTarget(player.transform);
        }
    }

    private void ConnectSystems()
    {
        var connector = FindObjectOfType<SceneConnector>();
        if (connector == null)
        {
            GameObject connectorObj = new GameObject("[SceneConnector]");
            connector = connectorObj.AddComponent<SceneConnector>();
        }

        connector.ConnectAllSystems();
    }

    private void SetupEnvironment()
    {
        if (!m_spawnEnvironment) return;

        if (m_sceneSetup == null)
        {
            var setups = FindObjectsOfType<SceneSetup>();
            if (setups.Length > 0)
            {
                m_sceneSetup = setups[0];
            }
        }

        if (m_sceneSetup != null)
        {
            m_sceneSetup.SetupScene();
        }
    }
}
