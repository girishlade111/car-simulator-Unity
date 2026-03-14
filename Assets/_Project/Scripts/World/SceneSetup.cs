using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string m_sceneName = "OpenWorld_TestDistrict";
    [SerializeField] private Vector2 m_districtSize = new Vector2(500f, 500f);

    [Header("Ground")]
    [SerializeField] private bool m_createGround = true;
    [SerializeField] private Vector2 m_groundSize = new Vector2(500f, 500f);

    [Header("Roads")]
    [SerializeField] private bool m_createRoads = true;
    [SerializeField] private int m_mainRoads = 2;
    [SerializeField] private int m_crossRoads = 2;
    [SerializeField] private float m_roadWidth = 8f;

    [Header("Buildings")]
    [SerializeField] private bool m_createBuildings = true;
    [SerializeField] private int m_buildingCount = 8;
    [SerializeField] private Vector2 m_buildingArea = new Vector2(200f, 200f);

    [Header("Parked Cars")]
    [SerializeField] private bool m_createParkedCars = true;
    [SerializeField] private int m_parkedCarCount = 10;

    [Header("Environment")]
    [SerializeField] private bool m_createEnvironment = true;
    [SerializeField] private int m_treeCount = 30;
    [SerializeField] private int m_rockCount = 20;

    [Header("References")]
    [SerializeField] private GameObject m_playerCarPrefab;
    [SerializeField] private Transform m_cameraPrefab;

    [Header("Root Containers")]
    [SerializeField] private bool m_createRootContainers = true;

    private Transform m_worldRoot;
    private Transform m_roadRoot;
    private Transform m_buildingRoot;
    private Transform m_vehicleRoot;
    private Transform m_environmentRoot;

    [ContextMenu("Setup Scene")]
    public void SetupScene()
    {
        ClearScene();

        if (m_createRootContainers)
        {
            CreateRootContainers();
        }

        if (m_createGround)
        {
            CreateGround();
        }

        if (m_createRoads)
        {
            CreateRoadNetwork();
        }

        if (m_createBuildings)
        {
            CreateBuildings();
        }

        if (m_createParkedCars)
        {
            CreateParkedCars();
        }

        if (m_createEnvironment)
        {
            CreateEnvironment();
        }

        if (m_playerCarPrefab != null)
        {
            SpawnPlayerCar();
        }

        SetupLighting();
        SetupCamera();

        Debug.Log("[SceneSetup] Scene setup complete!");
    }

    [ContextMenu("Clear Scene")]
    public void ClearScene()
    {
        if (m_worldRoot != null)
        {
            DestroyImmediate(m_worldRoot.gameObject);
        }

        var cameras = FindObjectsOfType<Camera>();
        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i].gameObject.name != "Main Camera")
            {
                DestroyImmediate(cameras[i].gameObject);
            }
        }

        Debug.Log("[SceneSetup] Scene cleared");
    }

    private void CreateRootContainers()
    {
        GameObject worldRoot = new GameObject("WorldRoot");
        worldRoot.transform.position = Vector3.zero;
        m_worldRoot = worldRoot.transform;

        m_roadRoot = CreateChildContainer("Roads", m_worldRoot);
        m_buildingRoot = CreateChildContainer("Buildings", m_worldRoot);
        m_vehicleRoot = CreateChildContainer("Vehicles", m_worldRoot);
        m_environmentRoot = CreateChildContainer("Environment", m_worldRoot);
    }

    private Transform CreateChildContainer(string name, Transform parent)
    {
        GameObject child = new GameObject(name);
        child.transform.SetParent(parent);
        child.transform.localPosition = Vector3.zero;
        return child.transform;
    }

    private void CreateGround()
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "GroundPlane";
        ground.transform.SetParent(m_worldRoot);
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = new Vector3(m_groundSize.x / 10f, 1f, m_groundSize.y / 10f);

        ground.layer = LayerMask.NameToLayer("Ground");

        Debug.Log($"[SceneSetup] Ground created: {m_groundSize.x}x{m_groundSize.y}");
    }

    private void CreateRoadNetwork()
    {
        for (int i = 0; i < m_mainRoads; i++)
        {
            float z = (i - (m_mainRoads - 1) / 2f) * (m_groundSize.y / (m_mainRoads + 1));
            CreateRoad(Vector3.forward * z, new Vector2(m_roadWidth, m_groundSize.x), 0);
        }

        for (int i = 0; i < m_crossRoads; i++)
        {
            float x = (i - (m_crossRoads - 1) / 2f) * (m_groundSize.x / (m_crossRoads + 1));
            CreateRoad(Vector3.right * x, new Vector2(m_groundSize.x, m_roadWidth), 90);
        }

        Debug.Log($"[SceneSetup] Roads created: {m_mainRoads} main + {m_crossRoads} cross");
    }

    private void CreateRoad(Vector3 position, Vector2 size, float rotation)
    {
        GameObject road = GameObject.CreatePrimitive(PrimitiveType.Cube);
        road.name = "Road";
        road.transform.SetParent(m_roadRoot);
        road.transform.position = position + Vector3.up * 0.05f;
        road.transform.rotation = Quaternion.Euler(0, rotation, 0);
        road.transform.localScale = new Vector3(size.x, 0.1f, size.y);
    }

    private void CreateBuildings()
    {
        for (int i = 0; i < m_buildingCount; i++)
        {
            Vector2 pos = GetRandomBuildingPosition();
            float rotation = Random.Range(0, 4) * 90f;

            GameObject building = CreateBuildingAt(pos, rotation);
            building.transform.SetParent(m_buildingRoot);
        }

        Debug.Log($"[SceneSetup] Buildings created: {m_buildingCount}");
    }

    private Vector2 GetRandomBuildingPosition()
    {
        float minDist = 30f;
        Vector2 candidate;
        int attempts = 0;

        do
        {
            candidate = new Vector2(
                Random.Range(-m_buildingArea.x / 2f, m_buildingArea.x / 2f),
                Random.Range(-m_buildingArea.y / 2f, m_buildingArea.y / 2f)
            );
            attempts++;
        }
        while (IsNearRoad(candidate) && attempts < 50);

        return candidate;
    }

    private bool IsNearRoad(Vector2 pos)
    {
        float halfWidth = m_roadWidth / 2f + 5f;

        for (int i = 0; i < m_mainRoads; i++)
        {
            float roadZ = (i - (m_mainRoads - 1) / 2f) * (m_groundSize.y / (m_mainRoads + 1));
            if (Mathf.Abs(pos.y - roadZ) < halfWidth)
                return true;
        }

        for (int i = 0; i < m_crossRoads; i++)
        {
            float roadX = (i - (m_crossRoads - 1) / 2f) * (m_groundSize.x / (m_crossRoads + 1));
            if (Mathf.Abs(pos.x - roadX) < halfWidth)
                return true;
        }

        return false;
    }

    private GameObject CreateBuildingAt(Vector2 position, float rotationY)
    {
        GameObject building = new GameObject($"Building_{Random.Range(1, 1000)}");
        building.transform.position = new Vector3(position.x, 0, position.y);
        building.transform.rotation = Quaternion.Euler(0, rotationY, 0);

        float width = Random.Range(12f, 18f);
        float height = Random.Range(15f, 25f);
        float depth = Random.Range(12f, 18f);

        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
        body.transform.SetParent(building.transform);
        body.transform.localPosition = new Vector3(0, height / 2f, 0);
        body.transform.localScale = new Vector3(width, height, depth);
        body.name = "Body";

        GameObject roof = GameObject.CreatePrimitive(PrimitiveType.Cube);
        roof.transform.SetParent(building.transform);
        roof.transform.localPosition = new Vector3(0, height + 0.5f, 0);
        roof.transform.localScale = new Vector3(width + 1, 1, depth + 1);
        roof.name = "Roof";

        return building;
    }

    private void CreateParkedCars()
    {
        for (int i = 0; i < m_parkedCarCount; i++)
        {
            Vector2 pos = GetRandomCarPosition();
            float rotation = GetCarRotation(pos);

            GameObject car = CreateParkedCarAt(pos, rotation);
            car.transform.SetParent(m_vehicleRoot);
        }

        Debug.Log($"[SceneSetup] Parked cars created: {m_parkedCarCount}");
    }

    private Vector2 GetRandomCarPosition()
    {
        Vector2 candidate;
        int attempts = 0;

        do
        {
            candidate = new Vector2(
                Random.Range(-m_groundSize.x / 2f + 10f, m_groundSize.x / 2f - 10f),
                Random.Range(-m_groundSize.y / 2f + 10f, m_groundSize.y / 2f - 10f)
            );
            attempts++;
        }
        while (IsNearIntersection(candidate) && attempts < 50);

        return candidate;
    }

    private bool IsNearIntersection(Vector2 pos)
    {
        float minDist = m_roadWidth + 5f;

        for (int i = 0; i < m_mainRoads; i++)
        {
            float roadZ = (i - (m_mainRoads - 1) / 2f) * (m_groundSize.y / (m_mainRoads + 1));
            if (Mathf.Abs(pos.y - roadZ) < minDist)
            {
                for (int j = 0; j < m_crossRoads; j++)
                {
                    float roadX = (j - (m_crossRoads - 1) / 2f) * (m_groundSize.x / (m_crossRoads + 1));
                    if (Mathf.Abs(pos.x - roadX) < minDist)
                        return true;
                }
            }
        }
        return false;
    }

    private float GetCarRotation(Vector2 pos)
    {
        float minDist = m_roadWidth + 10f;

        for (int i = 0; i < m_mainRoads; i++)
        {
            float roadZ = (i - (m_mainRoads - 1) / 2f) * (m_groundSize.y / (m_mainRoads + 1));
            if (Mathf.Abs(pos.y - roadZ) < minDist)
                return 90f;
        }

        for (int j = 0; j < m_crossRoads; j++)
        {
            float roadX = (j - (m_crossRoads - 1) / 2f) * (m_groundSize.x / (m_crossRoads + 1));
            if (Mathf.Abs(pos.x - roadX) < minDist)
                return 0f;
        }

        return Random.Range(0, 2) == 0 ? 0f : 90f;
    }

    private GameObject CreateParkedCarAt(Vector2 position, float rotation)
    {
        GameObject car = new GameObject($"ParkedCar_{Random.Range(1, 1000)}");
        car.transform.position = new Vector3(position.x, 0.4f, position.y);
        car.transform.rotation = Quaternion.Euler(0, rotation, 0);

        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
        body.transform.SetParent(car.transform);
        body.transform.localPosition = Vector3.zero;
        body.transform.localScale = new Vector3(2f, 0.8f, 4f);
        body.name = "Body";

        GameObject roof = GameObject.CreatePrimitive(PrimitiveType.Cube);
        roof.transform.SetParent(car.transform);
        roof.transform.localPosition = new Vector3(0, 0.7f, -0.3f);
        roof.transform.localScale = new Vector3(1.8f, 0.6f, 2f);
        roof.name = "Roof";

        return car;
    }

    private void CreateEnvironment()
    {
        for (int i = 0; i < m_treeCount; i++)
        {
            Vector3 pos = GetRandomEnvironmentPosition();
            if (pos.y > -100f)
            {
                CreateTreeAt(pos);
            }
        }

        for (int i = 0; i < m_rockCount; i++)
        {
            Vector3 pos = GetRandomEnvironmentPosition();
            if (pos.y > -100f)
            {
                CreateRockAt(pos);
            }
        }

        Debug.Log($"[SceneSetup] Environment created: {m_treeCount} trees, {m_rockCount} rocks");
    }

    private Vector3 GetRandomEnvironmentPosition()
    {
        Vector3 candidate;
        int attempts = 0;

        do
        {
            candidate = new Vector3(
                Random.Range(-m_groundSize.x / 2f, m_groundSize.x / 2f),
                50f,
                Random.Range(-m_groundSize.y / 2f, m_groundSize.y / 2f)
            );
            attempts++;
        }
        while ((IsNearRoad(candidate) || IsNearBuilding(candidate)) && attempts < 50);

        RaycastHit hit;
        if (Physics.Raycast(candidate, Vector3.down, out hit, 100f))
        {
            return hit.point;
        }

        return new Vector3(candidate.x, -1000f, candidate.z);
    }

    private bool IsNearBuilding(Vector3 pos)
    {
        if (m_buildingRoot == null) return false;

        for (int i = 0; i < m_buildingRoot.childCount; i++)
        {
            float dist = Vector3.Distance(pos, m_buildingRoot.GetChild(i).position);
            if (dist < 25f)
                return true;
        }
        return false;
    }

    private void CreateTreeAt(Vector3 position)
    {
        GameObject tree = new GameObject($"Tree_{Random.Range(1, 1000)}");
        tree.transform.SetParent(m_environmentRoot);
        tree.transform.position = position;

        GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.transform.SetParent(tree.transform);
        trunk.transform.localPosition = new Vector3(0, 1.5f, 0);
        trunk.transform.localScale = new Vector3(0.4f, 1.5f, 0.4f);
        trunk.name = "Trunk";

        GameObject foliage = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        foliage.transform.SetParent(tree.transform);
        foliage.transform.localPosition = new Vector3(0, 4f, 0);
        foliage.transform.localScale = new Vector3(3f, 3f, 3f);
        foliage.name = "Foliage";
    }

    private void CreateRockAt(Vector3 position)
    {
        GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rock.name = $"Rock_{Random.Range(1, 1000)}";
        rock.transform.SetParent(m_environmentRoot);
        rock.transform.position = position;
        rock.transform.localScale = new Vector3(Random.Range(1f, 3f), Random.Range(0.5f, 1.5f), Random.Range(1f, 3f));
        rock.transform.rotation = Quaternion.Euler(Random.Range(0, 30), Random.Range(0, 360), Random.Range(0, 30));
    }

    private void SpawnPlayerCar()
    {
        GameObject player = Instantiate(m_playerCarPrefab, Vector3.zero, Quaternion.identity);
        player.name = "PlayerCar";
        player.tag = "Player";

        if (m_vehicleRoot != null)
        {
            player.transform.SetParent(m_vehicleRoot);
        }

        Debug.Log("[SceneSetup] Player car spawned");
    }

    private void SetupLighting()
    {
        var lights = FindObjectsOfType<Light>();

        bool hasDirectional = false;
        for (int i = 0; i < lights.Length; i++)
        {
            if (lights[i].type == LightType.Directional)
            {
                hasDirectional = true;
                break;
            }
        }

        if (!hasDirectional)
        {
            GameObject lightObj = new GameObject("Directional Light");
            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            lightObj.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }

        Debug.Log("[SceneSetup] Lighting configured");
    }

    private void SetupCamera()
    {
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.transform.position = new Vector3(0, 10f, -15f);
            mainCam.transform.rotation = Quaternion.Euler(20f, 0f, 0f);

            var followCam = mainCam.GetComponent<FollowCamera>();
            if (followCam == null)
            {
                mainCam.gameObject.AddComponent<FollowCamera>();
            }
        }

        Debug.Log("[SceneSetup] Camera configured");
    }
}
