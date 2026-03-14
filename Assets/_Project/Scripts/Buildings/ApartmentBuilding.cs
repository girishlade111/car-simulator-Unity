using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.Buildings
{
    public class ApartmentBuilding : MonoBehaviour
    {
        [Header("Building Info")]
        [SerializeField] private string m_buildingName;
        [SerializeField] private int m_floorCount = 5;
        [SerializeField] private ApartmentBuildingData.BuildingStyle m_style;

        [Header("Components")]
        [SerializeField] private Transform m_roof;
        [SerializeField] private Transform[] m_floors;
        [SerializeField] private Renderer[] m_wallRenderers;
        [SerializeField] private Renderer[] m_windowRenderers;
        [SerializeField] private Renderer m_roofRenderer;

        [Header("Entry Points")]
        [SerializeField] private List<BuildingEntryPoint> m_entryPoints = new List<BuildingEntryPoint>();
        [SerializeField] private List<Transform> m_doorTransforms;

        [Header("Parking")]
        [SerializeField] private Transform m_parkingArea;
        [SerializeField] private List<Transform> m_parkingSpots;
        [SerializeField] private int m_parkingSpaceCount = 6;

        [Header("Interior")]
        [SerializeField] private List<InteriorPortal> m_interiorPortals = new List<InteriorPortal>();
        [SerializeField] private bool m_interiorLoaded;

        [Header("LOD")]
        [SerializeField] private LODGroup m_lodGroup;
        [SerializeField] private float m_lodDistance = 80f;

        [Header("Variation")]
        [SerializeField] private int m_variantIndex;
        [SerializeField] private Color m_buildingColor;
        [SerializeField] private Material m_buildingMaterial;

        private ApartmentBuildingData m_buildingData;

        [System.Serializable]
        public class BuildingEntryPoint
        {
            public Transform transform;
            public string entryId;
            public bool isMainEntrance;
            public int floorLevel;
        }

        [System.Serializable]
        public class InteriorPortal
        {
            public Transform portalTransform;
            public string portalId;
            public Vector3 size;
            public bool isLocked;
            public bool isVisible;
        }

        public string BuildingName => m_buildingName;
        public int FloorCount => m_floorCount;
        public int ParkingSpaceCount => m_parkingSpaceCount;
        public bool HasInterior => m_interiorLoaded;
        public List<BuildingEntryPoint> EntryPoints => m_entryPoints;
        public List<InteriorPortal> InteriorPortals => m_interiorPortals;

        public void Initialize(ApartmentBuildingData data)
        {
            m_buildingData = data;
            m_buildingName = data.BuildingName;
            m_style = data.Style;
            m_floorCount = Random.Range(data.MinFloors, data.MaxFloors + 1);
            m_parkingSpaceCount = data.ParkingSpaces;

            ApplyColors();
            SetupLOD();
            GenerateEntryPoints();
            GenerateParkingSpots();
            SetupInteriorPortals();
        }

        private void ApplyColors()
        {
            if (m_buildingData == null) return;

            m_buildingColor = m_buildingData.PrimaryColor;

            if (m_wallRenderers != null)
            {
                foreach (var renderer in m_wallRenderers)
                {
                    if (renderer != null && renderer.material != null)
                    {
                        renderer.material.color = m_buildingColor;
                    }
                }
            }

            if (m_roofRenderer != null && m_roofRenderer.material != null)
            {
                m_roofRenderer.material.color = m_buildingData.RoofColor;
            }

            if (m_windowRenderers != null)
            {
                foreach (var renderer in m_windowRenderers)
                {
                    if (renderer != null && renderer.material != null)
                    {
                        renderer.material.color = m_buildingData.WindowFrameColor;
                    }
                }
            }
        }

        private void SetupLOD()
        {
            if (m_lodGroup == null)
            {
                m_lodGroup = GetComponent<LODGroup>();
            }

            if (m_lodGroup == null)
            {
                m_lodGroup = gameObject.AddComponent<LODGroup>();
            }

            Renderer[] renderers = GetComponentsInChildren<Renderer>();

            LOD[] lods = new LOD[3];
            lods[0] = new LOD(1f / m_lodDistance, renderers);
            lods[1] = new LOD(1f / (m_lodDistance * 2), renderers);
            lods[2] = new LOD(1f / (m_lodDistance * 3), renderers);

            m_lodGroup.SetLODs(lods);
            m_lodGroup.RecalculateBounds();
        }

        private void GenerateEntryPoints()
        {
            m_entryPoints.Clear();
            m_doorTransforms.Clear();

            if (m_buildingData == null) return;

            for (int i = 0; i < m_buildingData.EntryCount; i++)
            {
                GameObject entryObj = new GameObject($"Entry_{i}");
                entryObj.transform.SetParent(transform);

                float xOffset = (i - (m_buildingData.EntryCount - 1) / 2f) * m_buildingData.EntrySpacing.x;
                entryObj.transform.localPosition = new Vector3(xOffset, 0, m_buildingData.Dimensions.z / 2f + 0.5f);

                BuildingEntryPoint entry = new BuildingEntryPoint
                {
                    transform = entryObj.transform,
                    entryId = $"Entry_{m_buildingName}_{i}",
                    isMainEntrance = i == 0,
                    floorLevel = 0
                };

                m_entryPoints.Add(entry);
                m_doorTransforms.Add(entryObj.transform);

                CreateEntryMarker(entryObj);
            }
        }

        private void CreateEntryMarker(GameObject entry)
        {
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            marker.name = "EntryMarker";
            marker.transform.SetParent(entry.transform);
            marker.transform.localPosition = Vector3.zero;
            marker.transform.localScale = new Vector3(1.5f, 2.5f, 0.3f);

            Renderer renderer = marker.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = new Material(Shader.Find("Standard"));
                renderer.material.color = new Color(0.3f, 0.2f, 0.1f);
            }

            GameObject.Destroy(marker.GetComponent<Collider>());
        }

        private void GenerateParkingSpots()
        {
            if (!m_buildingData.HasParking || m_parkingArea == null) return;

            m_parkingSpots.Clear();

            Vector2 areaSize = m_buildingData.ParkingAreaSize;
            int spotsX = Mathf.FloorToInt(areaSize.x / 3f);
            int spotsZ = Mathf.FloorToInt(areaSize.y / 5f);

            for (int x = 0; x < spotsX; x++)
            {
                for (int z = 0; z < spotsZ; z++)
                {
                    if (m_parkingSpots.Count >= m_parkingSpaceCount) break;

                    GameObject spot = new GameObject($"ParkingSpot_{x}_{z}");
                    spot.transform.SetParent(m_parkingArea);

                    float xPos = (x - spotsX / 2f) * 3f + 1.5f;
                    float zPos = (z - spotsZ / 2f) * 5f + 2.5f;
                    spot.transform.localPosition = new Vector3(xPos, 0, zPos);

                    CreateParkingSpaceMarker(spot);
                    m_parkingSpots.Add(spot.transform);
                }
            }
        }

        private void CreateParkingSpaceMarker(GameObject spot)
        {
            GameObject line = GameObject.CreatePrimitive(PrimitiveType.Quad);
            line.transform.SetParent(spot.transform);
            line.transform.localPosition = Vector3.zero;
            line.transform.localRotation = Quaternion.Euler(90, 0, 0);
            line.transform.localScale = new Vector3(2.5f, 0.1f, 4.5f);

            Renderer renderer = line.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = new Material(Shader.Find("Standard"));
                renderer.material.color = Color.white;
                renderer.material.SetFloat("_Mode", 3);
            }

            GameObject.Destroy(line.GetComponent<Collider>());
        }

        private void SetupInteriorPortals()
        {
            m_interiorPortals.Clear();

            if (!m_buildingData.HasInterior) return;

            var portals = m_buildingData.Portals;
            if (portals == null) return;

            foreach (var portalData in portals)
            {
                GameObject portalObj = new GameObject($"InteriorPortal_{portalData.portalId}");
                portalObj.transform.SetParent(transform);
                portalObj.transform.localPosition = portalData.position;

                CreatePortalVisuals(portalObj, portalData.size);

                InteriorPortal portal = new InteriorPortal
                {
                    portalTransform = portalObj.transform,
                    portalId = portalData.portalId,
                    size = portalData.size,
                    isLocked = portalData.isLocked,
                    isVisible = true
                };

                m_interiorPortals.Add(portal);
            }
        }

        private void CreatePortalVisuals(GameObject portal, Vector3 size)
        {
            GameObject frame = GameObject.CreatePrimitive(PrimitiveType.Cube);
            frame.name = "PortalFrame";
            frame.transform.SetParent(portal.transform);
            frame.transform.localPosition = Vector3.zero;
            frame.transform.localScale = size;

            Renderer renderer = frame.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = new Material(Shader.Find("Standard"));
                renderer.material.color = new Color(0.1f, 0.1f, 0.15f);
            }

            GameObject.Destroy(frame.GetComponent<Collider>());

            GameObject glow = GameObject.CreatePrimitive(PrimitiveType.Quad);
            glow.name = "PortalGlow";
            glow.transform.SetParent(portal.transform);
            glow.transform.localPosition = new Vector3(0, 0, size.z / 2f + 0.01f);
            glow.transform.localScale = new Vector3(size.x * 0.8f, size.y * 0.8f, 1f);

            Renderer glowRenderer = glow.GetComponent<Renderer>();
            if (glowRenderer != null)
            {
                glowRenderer.material = new Material(Shader.Find("Unlit/Transparent"));
                glowRenderer.material.color = new Color(0.2f, 0.5f, 1f, 0.5f);
            }

            GameObject.Destroy(glow.GetComponent<Collider>());
        }

        public Transform GetRandomEntryPoint()
        {
            if (m_doorTransforms.Count == 0) return transform;

            return m_doorTransforms[Random.Range(0, m_doorTransforms.Count)];
        }

        public Transform GetNearestEntryPoint(Vector3 position)
        {
            if (m_doorTransforms.Count == 0) return transform;

            Transform nearest = m_doorTransforms[0];
            float minDist = Vector3.Distance(position, nearest.position);

            foreach (var entry in m_doorTransforms)
            {
                float dist = Vector3.Distance(position, entry.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = entry;
                }
            }

            return nearest;
        }

        public Transform GetFreeParkingSpot()
        {
            if (m_parkingSpots.Count == 0) return null;

            return m_parkingSpots[Random.Range(0, m_parkingSpots.Count)];
        }

        public bool TryEnterInterior(string portalId)
        {
            foreach (var portal in m_interiorPortals)
            {
                if (portal.portalId == portalId)
                {
                    if (portal.isLocked)
                    {
                        Debug.Log($"[ApartmentBuilding] Portal {portalId} is locked");
                        return false;
                    }

                    Debug.Log($"[ApartmentBuilding] Entering interior via {portalId}");
                    return true;
                }
            }

            return false;
        }

        public void SetColor(Color color)
        {
            m_buildingColor = color;

            if (m_wallRenderers != null)
            {
                foreach (var renderer in m_wallRenderers)
                {
                    if (renderer != null && renderer.material != null)
                    {
                        renderer.material.color = color;
                    }
                }
            }
        }

        public int GetVariantIndex() => m_variantIndex;
        public void SetVariantIndex(int index) => m_variantIndex = index;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 1f, 0f, 0.3f);

            foreach (var entry in m_entryPoints)
            {
                if (entry.transform != null)
                {
                    Gizmos.DrawWireCube(entry.transform.position, new Vector3(1f, 2f, 1f));
                }
            }

            Gizmos.color = new Color(0f, 1f, 0f, 0.3f);

            if (m_parkingSpots != null)
            {
                foreach (var spot in m_parkingSpots)
                {
                    if (spot != null)
                    {
                        Gizmos.DrawWireCube(spot.position, new Vector3(2.5f, 0.1f, 4.5f));
                    }
                }
            }

            Gizmos.color = new Color(0f, 0.5f, 1f, 0.3f);

            foreach (var portal in m_interiorPortals)
            {
                if (portal.portalTransform != null)
                {
                    Gizmos.DrawWireCube(portal.portalTransform.position, portal.size);
                }
            }
        }
    }
}
