using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.Buildings
{
    public class InteriorLoader : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool m_loadOnAwake = false;
        [SerializeField] private bool m_asyncLoading = true;

        [Header("Room Templates")]
        [SerializeField] private InteriorRoomTemplate[] m_roomTemplates;

        [Header("Furniture")]
        [SerializeField] private GameObject[] m_furniturePrefabs;

        [Header("Building")]
        [SerializeField] private int m_floorCount = 3;
        [SerializeField] private Vector2 m_buildingSize = new Vector2(20f, 20f);

        private List<GameObject> m_loadedRooms = new List<GameObject>();
        private bool m_isLoaded;

        private void Awake()
        {
            if (m_loadOnAwake)
            {
                LoadInterior();
            }
        }

        public void LoadInterior()
        {
            if (m_isLoaded) return;

            ClearInterior();

            for (int floor = 0; floor < m_floorCount; floor++)
            {
                GenerateFloor(floor);
            }

            m_isLoaded = true;
            Debug.Log($"[InteriorLoader] Loaded interior with {m_floorCount} floors");
        }

        private void GenerateFloor(int floorIndex)
        {
            GameObject floorObj = new GameObject($"Floor_{floorIndex}");
            floorObj.transform.SetParent(transform);
            floorObj.transform.localPosition = new Vector3(0, floorIndex * 4f, 0);

            if (m_roomTemplates != null && m_roomTemplates.Length > 0)
            {
                int roomsPerFloor = Mathf.CeilToInt(m_buildingSize.x / 10f);

                for (int i = 0; i < roomsPerFloor; i++)
                {
                    if (i < m_roomTemplates.Length)
                    {
                        Vector3 roomPos = new Vector3(i * 10f - m_buildingSize.x / 2f + 5f, 0, 0);
                        m_roomTemplates[i].GenerateRoom(floorObj.transform, roomPos);
                        m_loadedRooms.Add(floorObj);
                    }
                }
            }

            GenerateHallway(floorObj);
        }

        private void GenerateHallway(GameObject floorObj)
        {
            GameObject hallway = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hallway.name = "Hallway";
            hallway.transform.SetParent(floorObj.transform);
            hallway.transform.localPosition = new Vector3(m_buildingSize.x / 2f + 2f, 1.5f, 0);
            hallway.transform.localScale = new Vector3(4f, 3f, m_buildingSize.y);

            m_loadedRooms.Add(hallway);
        }

        public void ClearInterior()
        {
            foreach (var room in m_loadedRooms)
            {
                if (room != null)
                {
                    Destroy(room);
                }
            }
            m_loadedRooms.Clear();
            m_isLoaded = false;
        }

        public void ReloadInterior()
        {
            ClearInterior();
            LoadInterior();
        }

        public bool IsLoaded() => m_isLoaded;
    }

    public class InteriorExitPoint : MonoBehaviour
    {
        [Header("Exit Settings")]
        [SerializeField] private string m_exitId;
        [SerializeField] private ExitType m_exitType = ExitType.Door;
        [SerializeField] private Transform m_destination;
        [SerializeField] private bool m_leadsOutside;

        [Header("Requirements")]
        [SerializeField] private bool m_requiresKey = false;
        [SerializeField] private string m_requiredKeyId;

        [Header("Visual")]
        [SerializeField] private Renderer m_doorRenderer;
        [SerializeField] private Color m_lockedColor = Color.red;
        [SerializeField] private Color m_unlockedColor = Color.green;

        private bool m_isLocked;

        public enum ExitType
        {
            Door,
            Stairs,
            Elevator,
            Window,
            EmergencyExit
        }

        private void Start()
        {
            m_isLocked = m_requiresKey;
            UpdateDoorColor();
        }

        private void UpdateDoorColor()
        {
            if (m_doorRenderer != null)
            {
                m_doorRenderer.material.color = m_isLocked ? m_lockedColor : m_unlockedColor;
            }
        }

        public bool CanUse()
        {
            return !m_isLocked;
        }

        public void UseExit(GameObject user)
        {
            if (!CanUse())
            {
                Debug.Log("[InteriorExitPoint] Exit is locked!");
                return;
            }

            if (m_destination != null)
            {
                user.transform.position = m_destination.position;
                Debug.Log($"[InteriorExitPoint] Used exit {m_exitId}");
            }
            else if (m_leadsOutside)
            {
                user.transform.position = transform.position + transform.forward * 5f;
                Debug.Log($"[InteriorExitPoint] Exited building {m_exitId}");
            }
        }

        public void Unlock()
        {
            m_isLocked = false;
            UpdateDoorColor();
        }

        public void Lock()
        {
            m_isLocked = true;
            UpdateDoorColor();
        }

        public bool TryUnlock(string keyId)
        {
            if (m_requiredKeyId == keyId)
            {
                Unlock();
                return true;
            }
            return false;
        }

        public ExitType GetExitType() => m_exitType;
        public bool IsLocked() => m_isLocked;
    }

    public class InteriorLightManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool m_autoManage = true;
        [SerializeField] private Color m_ambientColor = new Color(0.2f, 0.2f, 0.25f);
        [SerializeField] private float m_ambientIntensity = 0.3f;

        [Header("Lights")]
        [SerializeField] private List<Light> m_interiorLights = new List<Light>();
        [SerializeField] private Light[] m_floorLights;

        [Header("Time of Day")]
        [SerializeField] private bool m_respondToTimeOfDay = true;
        [SerializeField] private float m_dayIntensity = 1f;
        [SerializeField] private float m_nightIntensity = 0.5f;

        private void Start()
        {
            FindLights();
            if (m_autoManage)
            {
                SetDayMode();
            }
        }

        private void FindLights()
        {
            var lights = GetComponentsInChildren<Light>();
            m_interiorLights.AddRange(lights);
        }

        private void Update()
        {
            if (m_respondToTimeOfDay)
            {
                UpdateForTimeOfDay();
            }
        }

        private void UpdateForTimeOfDay()
        {
            var timeOfDay = FindObjectOfType<World.EnhancedTimeOfDay>();
            if (timeOfDay == null) return;

            float currentHour = timeOfDay.CurrentTime;
            bool isNight = currentHour < 6f || currentHour > 20f;

            float targetIntensity = isNight ? m_nightIntensity : m_dayIntensity;

            foreach (var light in m_interiorLights)
            {
                if (light != null)
                {
                    light.intensity = Mathf.Lerp(light.intensity, targetIntensity, Time.deltaTime);
                }
            }
        }

        public void SetDayMode()
        {
            SetIntensity(m_dayIntensity);
            RenderSettings.ambientLight = m_ambientColor * m_ambientIntensity;
        }

        public void SetNightMode()
        {
            SetIntensity(m_nightIntensity);
            RenderSettings.ambientLight = m_ambientColor * m_ambientIntensity * 0.5f;
        }

        private void SetIntensity(float intensity)
        {
            foreach (var light in m_interiorLights)
            {
                if (light != null)
                {
                    light.intensity = intensity;
                }
            }
        }

        public void TurnOnAll()
        {
            foreach (var light in m_interiorLights)
            {
                if (light != null)
                {
                    light.enabled = true;
                }
            }
        }

        public void TurnOffAll()
        {
            foreach (var light in m_interiorLights)
            {
                if (light != null)
                {
                    light.enabled = false;
                }
            }
        }
    }
}
