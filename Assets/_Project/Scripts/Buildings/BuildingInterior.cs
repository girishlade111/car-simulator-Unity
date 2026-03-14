using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.Buildings
{
    public class BuildingInterior : MonoBehaviour
    {
        [Header("Interior Info")]
        [SerializeField] private string m_interiorName;
        [SerializeField] private string m_buildingId;
        [SerializeField] private InteriorType m_interiorType;
        [SerializeField] private int m_floorCount = 1;

        [Header("Rooms")]
        [SerializeField] private List<InteriorRoom> m_rooms = new List<InteriorRoom>();
        [SerializeField] private Transform m_roomsRoot;

        [Header("Props")]
        [SerializeField] private List<InteriorProp> m_props = new List<InteriorProp>();
        [SerializeField] private Transform m_propsRoot;

        [Header("Lighting")]
        [SerializeField] private List<Light> m_lights = new List<Light>();
        [SerializeField] private Color m_ambientColor = new Color(0.3f, 0.3f, 0.3f);
        [SerializeField] private float m_lightIntensity = 1f;

        [Header("Exits")]
        [SerializeField] private List<InteriorExit> m_exits = new List<InteriorExit>();

        public enum InteriorType
        {
            Apartment,
            Shop,
            Restaurant,
            Office,
            Lobby,
            Warehouse
        }

        [System.Serializable]
        public class InteriorRoom
        {
            public string roomId;
            public string roomName;
            public Transform transform;
            public Vector3 size;
            public bool isLocked;
        }

        [System.Serializable]
        public class InteriorProp
        {
            public string propId;
            public GameObject prefab;
            public Transform transform;
            public Vector3 localPosition;
            public Quaternion localRotation;
        }

        [System.Serializable]
        public class InteriorExit
        {
            public string exitId;
            public Transform transform;
            public Vector3 size;
            public bool leadsOutside;
        }

        public string InteriorName => m_interiorName;
        public InteriorType Type => m_interiorType;
        public List<InteriorRoom> Rooms => m_rooms;
        public List<InteriorExit> Exits => m_exits;

        public void Initialize(string name, InteriorType type, int floors)
        {
            m_interiorName = name;
            m_interiorType = type;
            m_floorCount = floors;

            CreateRootObjects();
            SetupInterior();
            SetupLighting();
        }

        private void CreateRootObjects()
        {
            GameObject roomsObj = new GameObject("Rooms");
            roomsObj.transform.SetParent(transform);
            m_roomsRoot = roomsObj.transform;

            GameObject propsObj = new GameObject("Props");
            propsObj.transform.SetParent(transform);
            m_propsRoot = propsObj.transform;
        }

        private void SetupInterior()
        {
            switch (m_interiorType)
            {
                case InteriorType.Apartment:
                    CreateApartmentInterior();
                    break;
                case InteriorType.Shop:
                    CreateShopInterior();
                    break;
                case InteriorType.Restaurant:
                    CreateRestaurantInterior();
                    break;
                case InteriorType.Office:
                    CreateOfficeInterior();
                    break;
                case InteriorType.Lobby:
                    CreateLobbyInterior();
                    break;
                case InteriorType.Warehouse:
                    CreateWarehouseInterior();
                    break;
            }
        }

        private void CreateApartmentInterior()
        {
            for (int floor = 0; floor < m_floorCount; floor++)
            {
                CreateRoom($"Floor_{floor}_LivingRoom", new Vector3(6f, 2.5f, 5f), new Vector3(-3f, floor * 3f, 0));
                CreateRoom($"Floor_{floor}_Bedroom", new Vector3(4f, 2.5f, 4f), new Vector3(3f, floor * 3f, 0));
                CreateRoom($"Floor_{floor}_Kitchen", new Vector3(3f, 2.5f, 3f), new Vector3(5f, floor * 3f, 2f));
                CreateRoom($"Floor_{floor}_Bathroom", new Vector3(2.5f, 2.5f, 2.5f), new Vector3(-5f, floor * 3f, 2f));
            }

            AddFurniture("LivingRoom", new Vector3(0, 0, 0), "Sofa");
            AddFurniture("LivingRoom", new Vector3(2f, 0, 0), "Table");
            AddFurniture("Bedroom", new Vector3(0, 0, 0), "Bed");
            AddFurniture("Kitchen", new Vector3(0, 0, 0), "Counter");

            CreateExit("MainDoor", new Vector3(0, 0, -3f), true);
        }

        private void CreateShopInterior()
        {
            CreateRoom("SalesFloor", new Vector3(8f, 3f, 10f), Vector3.zero);
            CreateRoom("Storage", new Vector3(4f, 3f, 4f), new Vector3(5f, 0, -2f));
            CreateRoom("Checkout", new Vector3(3f, 3f, 2f), new Vector3(-4f, 0, 3f));

            AddFurniture("SalesFloor", new Vector3(0, 0, 0), "Shelf");
            AddFurniture("SalesFloor", new Vector3(2f, 0, 0), "Display");
            AddFurniture("Checkout", new Vector3(0, 0, 0), "Counter");

            CreateExit("FrontDoor", new Vector3(0, 0, 5f), true);
            CreateExit("BackDoor", new Vector3(5f, 0, -3f), false);
        }

        private void CreateRestaurantInterior()
        {
            CreateRoom("DiningArea", new Vector3(10f, 3f, 12f), Vector3.zero);
            CreateRoom("Kitchen", new Vector3(6f, 3f, 5f), new Vector3(7f, 0, -3f));
            CreateRoom("Restroom", new Vector3(3f, 3f, 3f), new Vector3(-6f, 0, 4f));

            for (int i = 0; i < 6; i++)
            {
                Vector3 pos = new Vector3(Random.Range(-3f, 3f), 0, Random.Range(-4f, 4f));
                AddFurniture("DiningArea", pos, "Table");
            }

            CreateExit("MainEntrance", new Vector3(0, 0, 6f), true);
            CreateExit("KitchenExit", new Vector3(7f, 0, 0), false);
        }

        private void CreateOfficeInterior()
        {
            for (int floor = 0; floor < m_floorCount; floor++)
            {
                CreateRoom($"Floor_{floor}_Reception", new Vector3(5f, 2.5f, 5f), new Vector3(-4f, floor * 3f, 3f));

                for (int i = 0; i < 3; i++)
                {
                    CreateRoom($"Floor_{floor}_Office_{i}", new Vector3(4f, 2.5f, 4f), new Vector3(i * 4f - 2f, floor * 3f, -2f));
                    AddFurniture($"Floor_{floor}_Office_{i}", Vector3.zero, "Desk");
                }
            }

            CreateExit("MainEntrance", new Vector3(-4f, 0, 5f), true);
            CreateExit("EmergencyExit", new Vector3(4f, 0, -5f), false);
        }

        private void CreateLobbyInterior()
        {
            CreateRoom("MainHall", new Vector3(8f, 4f, 10f), Vector3.zero);
            CreateRoom("Reception", new Vector3(4f, 4f, 3f), new Vector3(-3f, 0, 3f));
            CreateRoom("ElevatorHall", new Vector3(3f, 4f, 3f), new Vector3(3f, 0, 3f));

            AddFurniture("MainHall", new Vector3(0, 0, 0), "ReceptionDesk");
            AddFurniture("MainHall", new Vector3(-2f, 0, -2f), "Seating");

            CreateExit("MainDoor", new Vector3(0, 0, 5f), true);
        }

        private void CreateWarehouseInterior()
        {
            CreateRoom("MainStorage", new Vector3(15f, 5f, 20f), Vector3.zero);
            CreateRoom("Office", new Vector3(5f, 5f, 4f), new Vector3(8f, 0, 7f));

            AddFurniture("MainStorage", new Vector3(0, 0, 0), "Shelf");
            AddFurniture("MainStorage", new Vector3(4f, 0, 0), "Pallet");

            CreateExit("LoadingBay", new Vector3(0, 0, 10f), true);
            CreateExit("SideDoor", new Vector3(8f, 0, 0), false);
        }

        private void CreateRoom(string name, Vector3 size, Vector3 position)
        {
            GameObject roomObj = new GameObject(name);
            roomObj.transform.SetParent(m_roomsRoot);
            roomObj.transform.localPosition = position;

            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Floor";
            floor.transform.SetParent(roomObj.transform);
            floor.transform.localPosition = new Vector3(0, 0.01f, 0);
            floor.transform.localScale = new Vector3(size.x, 0.02f, size.z);

            Renderer floorRenderer = floor.GetComponent<Renderer>();
            if (floorRenderer != null)
            {
                floorRenderer.material = new Material(Shader.Find("Standard"));
                floorRenderer.material.color = new Color(0.4f, 0.35f, 0.3f);
            }

            GameObject.Destroy(floor.GetComponent<Collider>());

            InteriorRoom room = new InteriorRoom
            {
                roomId = name,
                roomName = name,
                transform = roomObj.transform,
                size = size,
                isLocked = false
            };

            m_rooms.Add(room);
        }

        private void AddFurniture(string roomName, Vector3 localPos, string furnitureType)
        {
            GameObject prop = CreateFurniture(furnitureType);
            prop.transform.SetParent(m_propsRoot);
            prop.transform.localPosition = localPos;
        }

        private GameObject CreateFurniture(string type)
        {
            GameObject furniture = new GameObject($"Furniture_{type}");

            switch (type)
            {
                case "Sofa":
                    CreateFurniturePiece(furniture, new Vector3(2f, 0.5f, 1f), new Color(0.3f, 0.3f, 0.4f));
                    break;
                case "Table":
                    CreateFurniturePiece(furniture, new Vector3(1f, 0.5f, 0.6f), new Color(0.4f, 0.3f, 0.2f));
                    break;
                case "Bed":
                    CreateFurniturePiece(furniture, new Vector3(2f, 0.4f, 2.5f), new Color(0.8f, 0.8f, 0.8f));
                    break;
                case "Counter":
                    CreateFurniturePiece(furniture, new Vector3(2f, 1f, 0.5f), new Color(0.5f, 0.5f, 0.5f));
                    break;
                case "Desk":
                    CreateFurniturePiece(furniture, new Vector3(1.5f, 0.75f, 0.8f), new Color(0.4f, 0.3f, 0.2f));
                    break;
                case "Shelf":
                    CreateFurniturePiece(furniture, new Vector3(1f, 2f, 0.4f), new Color(0.5f, 0.4f, 0.3f));
                    break;
                case "Display":
                    CreateFurniturePiece(furniture, new Vector3(1f, 1.5f, 0.5f), new Color(0.6f, 0.6f, 0.6f));
                    break;
                default:
                    CreateFurniturePiece(furniture, new Vector3(1f, 1f, 1f), new Color(0.5f, 0.5f, 0.5f));
                    break;
            }

            return furniture;
        }

        private void CreateFurniturePiece(GameObject parent, Vector3 size, Color color)
        {
            GameObject piece = GameObject.CreatePrimitive(PrimitiveType.Cube);
            piece.transform.SetParent(parent.transform);
            piece.transform.localPosition = new Vector3(0, size.y / 2f, 0);
            piece.transform.localScale = size;

            Renderer renderer = piece.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = new Material(Shader.Find("Standard"));
                renderer.material.color = color;
            }
        }

        private void CreateExit(string id, Vector3 position, bool leadsOutside)
        {
            GameObject exitObj = new GameObject($"Exit_{id}");
            exitObj.transform.SetParent(m_roomsRoot);
            exitObj.transform.localPosition = position;

            InteriorExit exit = new InteriorExit
            {
                exitId = id,
                transform = exitObj.transform,
                size = new Vector3(1.5f, 2.5f, 0.3f),
                leadsOutside = leadsOutside
            };

            m_exits.Add(exit);
        }

        private void SetupLighting()
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = m_ambientColor;

            foreach (var room in m_rooms)
            {
                if (room.transform.childCount > 0)
                {
                    GameObject lightObj = new GameObject("RoomLight");
                    lightObj.transform.SetParent(room.transform);
                    lightObj.transform.localPosition = new Vector3(0, 2.5f, 0);

                    Light light = lightObj.AddComponent<Light>();
                    light.type = LightType.Point;
                    light.color = Color.white;
                    light.intensity = m_lightIntensity;
                    light.range = room.size.x;

                    m_lights.Add(light);
                }
            }
        }

        public InteriorExit GetExit(string id)
        {
            return m_exits.Find(e => e.exitId == id);
        }

        public Transform GetRandomExit()
        {
            if (m_exits.Count == 0) return transform;
            return m_exits[Random.Range(0, m_exits.Count)].transform;
        }
    }
}
