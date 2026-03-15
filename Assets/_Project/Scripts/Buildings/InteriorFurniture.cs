using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.Buildings
{
    public class InteriorFurniture : MonoBehaviour
    {
        [Header("Furniture Info")]
        [SerializeField] private string m_furnitureId;
        [SerializeField] private FurnitureType m_furnitureType;
        [SerializeField] private string m_displayName;

        [Header("Interaction")]
        [SerializeField] private bool m_canInteract = true;
        [SerializeField] private InteractionType m_interactionType;
        [SerializeField] private string m_interactionPrompt = "Press E to interact";

        [Header("States")]
        [SerializeField] private bool m_isLocked;
        [SerializeField] private bool m_isOccupied;
        [SerializeField] private float m_usageDuration = 2f;

        [Header("Visual")]
        [SerializeField] private Renderer[] m_visualRenderers;
        [SerializeField] private Color m_highlightColor = Color.yellow;

        private float m_usageTimer;
        private bool m_isBeingUsed;
        private Color[] m_originalColors;

        public enum FurnitureType
        {
            Chair,
            Table,
            Bed,
            Sofa,
            Desk,
            Counter,
            DisplayCase,
            VendingMachine,
            ATM,
            PhoneBooth,
            Toilet,
            Sink,
            Appliance,
            Storage,
            Decoration
        }

        public enum InteractionType
        {
            None,
            Sit,
            Sleep,
            Use,
            Open,
            Drink,
            Eat,
            Withdraw,
            Call,
            Wash
        }

        private void Start()
        {
            StoreOriginalColors();
        }

        private void Update()
        {
            if (m_isBeingUsed)
            {
                m_usageTimer += Time.deltaTime;
                if (m_usageTimer >= m_usageDuration)
                {
                    CompleteUsage();
                }
            }
        }

        private void StoreOriginalColors()
        {
            if (m_visualRenderers == null || m_visualRenderers.Length == 0)
            {
                m_visualRenderers = GetComponentsInChildren<Renderer>();
            }

            m_originalColors = new Color[m_visualRenderers.Length];
            for (int i = 0; i < m_visualRenderers.Length; i++)
            {
                if (m_visualRenderers[i] != null && m_visualRenderers[i].material != null)
                {
                    m_originalColors[i] = m_visualRenderers[i].material.color;
                }
            }
        }

        public bool CanUse()
        {
            return m_canInteract && !m_isLocked && !m_isBeingUsed && !m_isOccupied;
        }

        public void StartUse()
        {
            if (!CanUse()) return;

            m_isBeingUsed = true;
            m_usageTimer = 0f;
            Debug.Log($"[InteriorFurniture] Started using {m_displayName}");
        }

        private void CompleteUsage()
        {
            m_isBeingUsed = false;
            m_usageTimer = 0f;

            OnUseComplete();
            Debug.Log($"[InteriorFurniture] Finished using {m_displayName}");
        }

        private void OnUseComplete()
        {
            switch (m_interactionType)
            {
                case InteractionType.Drink:
                    Debug.Log("[InteriorFurniture] Consumed drink");
                    break;
                case InteractionType.Eat:
                    Debug.Log("[InteriorFurniture] Ate food");
                    break;
                case InteractionType.Withdraw:
                    Debug.Log("[InteriorFurniture] Withdrew money");
                    break;
                case InteractionType.Call:
                    Debug.Log("[InteriorFurniture] Made a call");
                    break;
            }
        }

        public void CancelUse()
        {
            m_isBeingUsed = false;
            m_usageTimer = 0f;
        }

        public void SetLocked(bool locked)
        {
            m_isLocked = locked;
        }

        public void SetOccupied(bool occupied)
        {
            m_isOccupied = occupied;
        }

        public void Highlight(bool highlight)
        {
            if (m_visualRenderers == null) return;

            Color targetColor = highlight ? m_highlightColor : Color.white;

            for (int i = 0; i < m_visualRenderers.Length; i++)
            {
                if (m_visualRenderers[i] != null && m_visualRenderers[i].material != null)
                {
                    m_visualRenderers[i].material.color = Color.Lerp(m_originalColors[i], targetColor, highlight ? 0.3f : 0f);
                }
            }
        }

        public string GetInteractionPrompt()
        {
            if (m_isLocked) return "Locked";
            if (m_isOccupied) return "In use";
            return m_interactionPrompt;
        }

        public FurnitureType GetFurnitureType() => m_furnitureType;
        public InteractionType GetInteractionType() => m_interactionType;
        public bool IsBeingUsed() => m_isBeingUsed;
        public bool IsLocked() => m_isLocked;
    }

    public class InteriorRoomTemplate : MonoBehaviour
    {
        [Header("Room Template")]
        [SerializeField] private string m_templateId;
        [SerializeField] private RoomCategory m_category;
        [SerializeField] private Vector2 m_roomSize = new Vector2(5f, 5f);

        [Header("Furniture Prefabs")]
        [SerializeField] private GameObject[] m_furniturePrefabs;
        [SerializeField] private int[] m_furnitureCounts;

        [Header("Floor")]
        [SerializeField] private Material m_floorMaterial;
        [SerializeField] private Material m_wallMaterial;
        [SerializeField] private Material m_ceilingMaterial;

        [Header("Lighting")]
        [SerializeField] private Color m_lightColor = Color.white;
        [SerializeField] private float m_lightIntensity = 1f;
        [SerializeField] private int m_lightCount = 1;

        [Header("Doors")]
        [SerializeField] private int m_doorCount = 1;
        [SerializeField] private bool m_hasWindows = true;

        public enum RoomCategory
        {
            LivingRoom,
            Bedroom,
            Bathroom,
            Kitchen,
            DiningRoom,
            Office,
            Retail,
            Restaurant,
            Lobby,
            Hallway,
            Storage,
            Garage
        }

        public GameObject GenerateRoom(Transform parent, Vector3 position)
        {
            GameObject room = new GameObject($"Room_{m_category}");
            room.transform.SetParent(parent);
            room.transform.position = position;

            CreateFloor(room);
            CreateWalls(room);
            if (m_hasWindows) CreateWindows(room);
            CreateDoors(room);
            CreateLighting(room);
            SpawnFurniture(room);

            return room;
        }

        private void CreateFloor(GameObject room)
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Floor";
            floor.transform.SetParent(room.transform);
            floor.transform.localPosition = Vector3.zero;
            floor.transform.localScale = new Vector3(m_roomSize.x / 10f, 1f, m_roomSize.y / 10f);

            if (m_floorMaterial != null)
            {
                floor.GetComponent<Renderer>().material = m_floorMaterial;
            }
        }

        private void CreateWalls(GameObject room)
        {
            Vector3[] wallPositions = {
                new Vector3(0, 1.5f, m_roomSize.y / 2f),
                new Vector3(0, 1.5f, -m_roomSize.y / 2f),
                new Vector3(m_roomSize.x / 2f, 1.5f, 0),
                new Vector3(-m_roomSize.x / 2f, 1.5f, 0)
            };

            Vector3[] wallScales = {
                new Vector3(m_roomSize.x, 3f, 0.1f),
                new Vector3(m_roomSize.x, 3f, 0.1f),
                new Vector3(0.1f, 3f, m_roomSize.y),
                new Vector3(0.1f, 3f, m_roomSize.y)
            };

            for (int i = 0; i < 4; i++)
            {
                GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wall.name = $"Wall_{i}";
                wall.transform.SetParent(room.transform);
                wall.transform.localPosition = wallPositions[i];
                wall.transform.localScale = wallScales[i];

                if (m_wallMaterial != null)
                {
                    wall.GetComponent<Renderer>().material = m_wallMaterial;
                }
            }
        }

        private void CreateWindows(GameObject room)
        {
            for (int i = 0; i < 2; i++)
            {
                GameObject window = GameObject.CreatePrimitive(PrimitiveType.Quad);
                window.name = $"Window_{i}";
                window.transform.SetParent(room.transform);
                window.transform.localPosition = new Vector3(0, 1.5f, m_roomSize.y / 2f + 0.01f);
                window.transform.localScale = new Vector3(2f, 1.5f, 1f);
            }
        }

        private void CreateDoors(GameObject room)
        {
            for (int i = 0; i < m_doorCount; i++)
            {
                GameObject door = GameObject.CreatePrimitive(PrimitiveType.Cube);
                door.name = $"Door_{i}";
                door.transform.SetParent(room.transform);
                door.transform.localPosition = new Vector3(0, 1f, m_roomSize.y / 2f);
                door.transform.localScale = new Vector3(1f, 2f, 0.1f);
            }
        }

        private void CreateLighting(GameObject room)
        {
            for (int i = 0; i < m_lightCount; i++)
            {
                GameObject light = new GameObject($"Light_{i}");
                light.transform.SetParent(room.transform);
                light.transform.localPosition = new Vector3(0, 2.8f, 0);

                Light lightComponent = light.AddComponent<Light>();
                lightComponent.color = m_lightColor;
                lightComponent.intensity = m_lightIntensity;
                lightComponent.range = 10f;
                lightComponent.type = LightType.Point;
            }
        }

        private void SpawnFurniture(GameObject room)
        {
            if (m_furniturePrefabs == null) return;

            for (int i = 0; i < m_furniturePrefabs.Length; i++)
            {
                if (m_furniturePrefabs[i] == null) continue;

                int count = m_furnitureCounts != null && i < m_furnitureCounts.Length ? m_furnitureCounts[i] : 1;

                for (int j = 0; j < count; j++)
                {
                    Vector3 offset = new Vector3(
                        Random.Range(-m_roomSize.x / 3f, m_roomSize.x / 3f),
                        0,
                        Random.Range(-m_roomSize.y / 3f, m_roomSize.y / 3f)
                    );

                    GameObject furniture = Instantiate(m_furniturePrefabs[i], room.transform);
                    furniture.transform.localPosition = offset;
                    furniture.transform.localRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                }
            }
        }

        public RoomCategory GetCategory() => m_category;
        public Vector2 GetRoomSize() => m_roomSize;
    }
}
