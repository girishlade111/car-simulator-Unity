using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.World
{
    [ExecuteInEditMode]
    public class ParkedCarGroup : MonoBehaviour
    {
        [Header("Group Settings")]
        [SerializeField] private string m_groupName = "Parking Area";
        [SerializeField] private ParkingLayout m_layout = ParkingLayout.Linear;
        
        [Header("Vehicle Pool")]
        [SerializeField] private GameObject[] m_vehiclePrefabs;
        [SerializeField] private bool m_randomizeSelection = true;
        [SerializeField] private bool m_randomizeColors = true;

        [Header("Layout Settings")]
        [SerializeField] private int m_carCount = 5;
        [SerializeField] private float m_spacing = 6f;
        [SerializeField] private float m_parallelOffset = 3f;
        [SerializeField] private float m_rotation = 0f;
        [SerializeField] private bool m_alternateDirection = true;

        [Header("Spawned Cars")]
        [SerializeField] private List<GameObject> m_spawnedCars = new List<GameObject>();

        [Header("Visualization")]
        [SerializeField] private bool m_showGizmo = true;
        [SerializeField] private Color m_gizmoColor = Color.green;

        public enum ParkingLayout
        {
            Linear,
            Parallel,
            Staggered,
            Grid
        }

        private void OnEnable()
        {
            if (!Application.isPlaying && m_spawnedCars.Count == 0)
            {
                GenerateCars();
            }
        }

        private void OnDisable()
        {
            if (!Application.isPlaying)
            {
                ClearCars();
            }
        }

        public void GenerateCars()
        {
            ClearCars();

            if (m_vehiclePrefabs == null || m_vehiclePrefabs.Length == 0)
            {
                Debug.LogWarning("[ParkedCarGroup] No vehicle prefabs assigned!", this);
                return;
            }

            for (int i = 0; i < m_carCount; i++)
            {
                Vector3 position = GetCarPosition(i);
                float rotation = GetCarRotation(i);

                GameObject prefab = GetVehiclePrefab(i);
                GameObject car = Instantiate(prefab, position, Quaternion.Euler(0, rotation, 0));
                car.transform.SetParent(transform);
                car.name = $"{m_groupName}_Car_{i}";

                var parkedCar = car.GetComponent<ParkedCar>();
                if (parkedCar == null)
                {
                    parkedCar = car.AddComponent<ParkedCar>();
                }

                if (m_randomizeColors)
                {
                    parkedCar.SetBodyColor(GetRandomCarColor());
                }

                m_spawnedCars.Add(car);
            }

            Debug.Log($"[ParkedCarGroup] Generated {m_spawnedCars.Count} cars in {m_groupName}");
        }

        private Vector3 GetCarPosition(int index)
        {
            switch (m_layout)
            {
                case ParkingLayout.Linear:
                    return transform.position + transform.forward * (index * m_spacing);

                case ParkingLayout.Parallel:
                    int row = index / 2;
                    int side = index % 2 == 0 ? -1 : 1;
                    return transform.position + 
                           transform.forward * (row * m_spacing) + 
                           transform.right * (side * m_parallelOffset);

                case ParkingLayout.Staggered:
                    float staggerOffset = (index % 2 == 0) ? 0 : m_spacing * 0.5f;
                    return transform.position + 
                           transform.forward * (index * m_spacing * 0.5f) +
                           transform.right * staggerOffset;

                case ParkingLayout.Grid:
                    int cols = Mathf.CeilToInt(Mathf.Sqrt(m_carCount));
                    int rowGrid = index / cols;
                    int col = index % cols;
                    return transform.position + 
                           transform.forward * (rowGrid * m_spacing) + 
                           transform.right * (col * m_parallelOffset);

                default:
                    return transform.position;
            }
        }

        private float GetCarRotation(int index)
        {
            float baseRotation = m_rotation + transform.eulerAngles.y;

            if (m_alternateDirection && m_layout != ParkingLayout.Linear)
            {
                if (index % 2 == 1)
                {
                    baseRotation += 180f;
                }
            }

            return baseRotation;
        }

        private GameObject GetVehiclePrefab(int index)
        {
            if (!m_randomizeSelection || m_vehiclePrefabs.Length == 1)
            {
                return m_vehiclePrefabs[0];
            }

            return m_vehiclePrefabs[Random.Range(0, m_vehiclePrefabs.Length)];
        }

        private Color GetRandomCarColor()
        {
            Color[] colors = {
                Color.white,
                Color.black,
                new Color(0.8f, 0.2f, 0.2f),
                new Color(0.2f, 0.3f, 0.8f),
                new Color(0.9f, 0.9f, 0.2f),
                new Color(0.2f, 0.7f, 0.3f),
                new Color(0.6f, 0.3f, 0.7f),
                new Color(0.9f, 0.5f, 0.2f),
                new Color(0.3f, 0.3f, 0.3f),
                new Color(0.7f, 0.7f, 0.7f)
            };

            return colors[Random.Range(0, colors.Length)];
        }

        public void ClearCars()
        {
            foreach (var car in m_spawnedCars)
            {
                if (car != null)
                {
                    DestroyImmediate(car);
                }
            }
            m_spawnedCars.Clear();
        }

        public void RefreshCars()
        {
            GenerateCars();
        }

        private void OnDrawGizmos()
        {
            if (!m_showGizmo) return;

            Gizmos.color = m_gizmoColor;

            for (int i = 0; i < m_carCount; i++)
            {
                Vector3 pos = GetCarPosition(i);
                Quaternion rot = Quaternion.Euler(0, GetCarRotation(i), 0);

                Matrix4x4 oldMatrix = Gizmos.matrix;
                Gizmos.matrix = Matrix4x4.TRS(pos, rot, Vector3.one);
                Gizmos.DrawWireCube(Vector3.up * 0.5f, new Vector3(2f, 1f, 4f));
                Gizmos.matrix = oldMatrix;
            }

            Gizmos.color = m_gizmoColor * 0.3f;
            Vector3 direction = transform.forward * (m_carCount * m_spacing * 0.5f);
            Gizmos.DrawLine(transform.position, transform.position + direction);
        }

        public List<GameObject> GetSpawnedCars() => m_spawnedCars;
        public int GetCarCount() => m_spawnedCars.Count;
    }
}
