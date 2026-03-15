using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

namespace CarSimulator.World
{
    public class InteriorScene : MonoBehaviour
    {
        [Header("Interior Settings")]
        [SerializeField] private string m_interiorId;
        [SerializeField] private string m_interiorName = "Interior";
        [SerializeField] private InteriorType m_interiorType = InteriorType.Apartment;

        [Header("Scene Reference")]
        [SerializeField] private string m_sceneName;
        [SerializeField] private bool m_useSeparateScene = false;

        [Header("Interior Objects")]
        [SerializeField] private GameObject m_interiorPrefab;
        [SerializeField] private Transform m_interiorRoot;
        [SerializeField] private Vector3 m_interiorOffset = new Vector3(0, 100f, 0);

        [Header("Spawn Points")]
        [SerializeField] private Transform m_entryPoint;
        [SerializeField] private Transform m_exitPoint;

        [Header("Interior Content")]
        [SerializeField] private InteriorRoom[] m_rooms;

        [Header("State")]
        [SerializeField] private bool m_isLoaded;

        private GameObject m_interiorInstance;
        private List<GameObject> m_dynamicObjects = new List<GameObject>();

        public enum InteriorType
        {
            Apartment,
            Shop,
            Office,
            Restaurant,
            Warehouse,
            Garage
        }

        private void Start()
        {
            if (m_interiorId == null)
            {
                m_interiorId = System.Guid.NewGuid().ToString();
            }
        }

        public GameObject CreateInterior(Vector3 worldPosition)
        {
            if (m_interiorPrefab != null)
            {
                m_interiorInstance = Instantiate(m_interiorPrefab, worldPosition + m_interiorOffset, Quaternion.identity);
            }
            else
            {
                m_interiorInstance = GeneratePlaceholderInterior();
            }

            m_interiorInstance.name = m_interiorName;

            InitializeInterior();

            m_isLoaded = true;
            return m_interiorInstance;
        }

        private GameObject GeneratePlaceholderInterior()
        {
            GameObject interior = new GameObject(m_interiorName);

            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Floor";
            floor.transform.SetParent(interior.transform);
            floor.transform.localPosition = Vector3.zero;
            floor.transform.localScale = new Vector3(2f, 1f, 2f);

            Material floorMat = new Material(Shader.Find("Standard"));
            floorMat.color = new Color(0.4f, 0.35f, 0.3f);
            floor.GetComponent<Renderer>().material = floorMat;

            for (int i = 0; i < 4; i++)
            {
                GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wall.name = $"Wall_{i}";
                wall.transform.SetParent(interior.transform);

                float wallLength = 10f;

                switch (i)
                {
                    case 0:
                        wall.transform.localPosition = new Vector3(0, 2f, wallLength / 2f);
                        wall.transform.localScale = new Vector3(wallLength, 4f, 0.2f);
                        break;
                    case 1:
                        wall.transform.localPosition = new Vector3(0, 2f, -wallLength / 2f);
                        wall.transform.localScale = new Vector3(wallLength, 4f, 0.2f);
                        break;
                    case 2:
                        wall.transform.localPosition = new Vector3(wallLength / 2f, 2f, 0);
                        wall.transform.localScale = new Vector3(0.2f, 4f, wallLength);
                        break;
                    case 3:
                        wall.transform.localPosition = new Vector3(-wallLength / 2f, 2f, 0);
                        wall.transform.localScale = new Vector3(0.2f, 4f, wallLength);
                        break;
                }

                Material wallMat = new Material(Shader.Find("Standard"));
                wallMat.color = new Color(0.8f, 0.8f, 0.8f);
                wall.GetComponent<Renderer>().material = wallMat;
            }

            if (m_rooms != null && m_rooms.Length > 0)
            {
                foreach (var room in m_rooms)
                {
                    CreateRoom(interior.transform, room);
                }
            }
            else
            {
                CreateDefaultFurniture(interior.transform);
            }

            return interior;
        }

        private void CreateRoom(Transform parent, InteriorRoom room)
        {
            GameObject roomObj = new GameObject(room.roomName);
            roomObj.transform.SetParent(parent);
            roomObj.transform.localPosition = room.position;

            if (room.furniturePrefabs != null)
            {
                foreach (var furniture in room.furniturePrefabs)
                {
                    if (furniture != null)
                    {
                        GameObject furnitureObj = Instantiate(furniture, roomObj.transform);
                        m_dynamicObjects.Add(furnitureObj);
                    }
                }
            }
        }

        private void CreateDefaultFurniture(Transform parent)
        {
            GameObject sofa = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sofa.name = "Sofa";
            sofa.transform.SetParent(parent.transform);
            sofa.transform.localPosition = new Vector3(0, 0.5f, 3f);
            sofa.transform.localScale = new Vector3(2f, 0.8f, 1f);
            m_dynamicObjects.Add(sofa);

            GameObject table = GameObject.CreatePrimitive(PrimitiveType.Cube);
            table.name = "Table";
            table.transform.SetParent(parent.transform);
            table.transform.localPosition = new Vector3(0, 0.4f, 0f);
            table.transform.localScale = new Vector3(1.2f, 0.4f, 0.8f);
            m_dynamicObjects.Add(table);

            GameObject tv = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tv.name = "TV";
            tv.transform.SetParent(parent.transform);
            tv.transform.localPosition = new Vector3(0, 1.5f, 4.5f);
            tv.transform.localScale = new Vector3(1.5f, 1f, 0.1f);
            m_dynamicObjects.Add(tv);

            Material furnitureMat = new Material(Shader.Find("Standard"));
            furnitureMat.color = new Color(0.3f, 0.3f, 0.35f);

            foreach (var obj in m_dynamicObjects)
            {
                if (obj.GetComponent<Renderer>() != null)
                {
                    obj.GetComponent<Renderer>().material = furnitureMat;
                }
            }
        }

        private void InitializeInterior()
        {
            var lights = m_interiorInstance.GetComponentsInChildren<Light>();
            foreach (var light in lights)
            {
                light.intensity = 1f;
            }
        }

        public IEnumerator LoadAdditive()
        {
            if (string.IsNullOrEmpty(m_sceneName))
            {
                Debug.LogWarning("[InteriorScene] No scene name specified for additive loading");
                yield break;
            }

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(m_sceneName, LoadSceneMode.Additive);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            Scene loadedScene = SceneManager.GetSceneByName(m_sceneName);
            if (loadedScene.IsValid())
            {
                GameObject[] roots = loadedScene.GetRootGameObjects();
                foreach (var root in roots)
                {
                    root.SetActive(true);
                }
            }

            m_isLoaded = true;
            Debug.Log($"[InteriorScene] Loaded additive scene: {m_sceneName}");
        }

        public IEnumerator UnloadAdditive()
        {
            if (string.IsNullOrEmpty(m_sceneName))
            {
                yield break;
            }

            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(m_sceneName);
            while (!asyncUnload.isDone)
            {
                yield return null;
            }

            m_isLoaded = false;
            Debug.Log($"[InteriorScene] Unloaded additive scene: {m_sceneName}");
        }

        public void DestroyInterior()
        {
            if (m_interiorInstance != null)
            {
                Destroy(m_interiorInstance);
                m_interiorInstance = null;
            }

            foreach (var obj in m_dynamicObjects)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
            m_dynamicObjects.Clear();

            m_isLoaded = false;
        }

        public Transform GetEntryPoint()
        {
            return m_entryPoint ?? transform;
        }

        public Transform GetExitPoint()
        {
            return m_exitPoint ?? transform;
        }

        public bool IsLoaded() => m_isLoaded;
        public string GetInteriorId() => m_interiorId;

        [System.Serializable]
        public class InteriorRoom
        {
            public string roomName;
            public Vector3 position;
            public GameObject[] furniturePrefabs;
        }
    }

    public class ApartmentOwnership : MonoBehaviour
    {
        [Header("Ownership Settings")]
        [SerializeField] private string m_ownerId;
        [SerializeField] private bool m_isForSale = true;
        [SerializeField] private int m_price;

        [Header("Upgrades")]
        [SerializeField] private int m_upgradeLevel;
        [SerializeField] private int m_maxUpgrades = 5;

        [Header("References")]
        [SerializeField] private ApartmentEntrance m_entrance;

        public void Purchase(string buyerId, int offeredPrice)
        {
            if (!m_isForSale || offeredPrice < m_price)
            {
                Debug.Log($"[ApartmentOwnership] Cannot purchase - Price: {m_price}, Offered: {offeredPrice}");
                return;
            }

            int playerCredits = PlayerPrefs.GetInt("PlayerCredits", 0);
            if (playerCredits < m_price)
            {
                Debug.Log($"[ApartmentOwnership] Cannot purchase - Not enough credits");
                return;
            }

            playerCredits -= m_price;
            PlayerPrefs.SetInt("PlayerCredits", playerCredits);
            PlayerPrefs.Save();

            m_ownerId = buyerId;
            m_isForSale = false;

            if (m_entrance != null)
            {
                m_entrance.SetOwned(true, buyerId);
            }

            Debug.Log($"[ApartmentOwnership] Purchased by {buyerId} for ${m_price}");
        }

        public void Upgrade()
        {
            if (m_upgradeLevel >= m_maxUpgrades)
            {
                Debug.Log("[ApartmentOwnership] Max upgrade level reached");
                return;
            }

            m_upgradeLevel++;
            Debug.Log($"[ApartmentOwnership] Upgraded to level {m_upgradeLevel}");
        }

        public bool IsOwned() => !m_isForSale;
        public bool IsForSale() => m_isForSale;
        public int GetPrice() => m_price;
        public int GetUpgradeLevel() => m_upgradeLevel;
    }
}
