using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.World
{
    [ExecuteInEditMode]
    public class RoadSideSpawner : MonoBehaviour
    {
        [Header("Reference Points")]
        [SerializeField] private Transform m_roadStart;
        [SerializeField] private Transform m_roadEnd;
        [SerializeField] private bool m_useBothSides = true;

        [Header("Spawn Settings")]
        [SerializeField] private GameObject[] m_vehiclePrefabs;
        [SerializeField] private int m_maxCars = 10;
        [SerializeField] private float m_minSpacing = 8f;
        [SerializeField] private float m_roadEdgeOffset = 3.5f;
        [SerializeField] private float m_randomOffset = 1f;

        [Header("Variations")]
        [SerializeField] private bool m_randomizeColors = true;
        [SerializeField] private bool m_randomizeRotation = true;
        [SerializeField] private float m_maxRotationVariation = 10f;

        [Header("Spawned")]
        [SerializeField] private List<GameObject> m_spawnedCars = new List<GameObject>();

        [Header("Visualization")]
        [SerializeField] private bool m_showGizmo = true;
        [SerializeField] private Color m_gizmoColor = Color.yellow;

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

            if (m_roadStart == null || m_roadEnd == null)
            {
                Debug.LogWarning("[RoadSideSpawner] Road start/end not assigned!", this);
                return;
            }

            if (m_vehiclePrefabs == null || m_vehiclePrefabs.Length == 0)
            {
                Debug.LogWarning("[RoadSideSpawner] No vehicle prefabs assigned!", this);
                return;
            }

            Vector3 roadDirection = (m_roadEnd.position - m_roadStart.position).normalized;
            float roadLength = Vector3.Distance(m_roadStart.position, m_roadEnd.position);

            int carsThisSide = m_maxCars;
            if (m_useBothSides)
            {
                carsThisSide = Mathf.CeilToInt(m_maxCars / 2f);
            }

            float currentDistance = Random.Range(m_minSpacing, m_minSpacing * 2);

            for (int side = 0; side < (m_useBothSides ? 2 : 1); side++)
            {
                currentDistance = Random.Range(m_minSpacing, m_minSpacing * 2);

                while (currentDistance < roadLength - m_minSpacing && m_spawnedCars.Count < m_maxCars)
                {
                    Vector3 basePosition = m_roadStart.position + (roadDirection * currentDistance);
                    
                    Vector3 sideOffset = (side == 0 ? 1 : -1) * m_roadEdgeOffset * Vector3.Cross(roadDirection, Vector3.up).normalized;
                    Vector3 randomOffset3D = new Vector3(
                        Random.Range(-m_randomOffset, m_randomOffset),
                        0,
                        Random.Range(-m_randomOffset, m_randomOffset)
                    );

                    Vector3 spawnPosition = basePosition + sideOffset + randomOffset3D;

                    float rotationY = Quaternion.LookRotation(roadDirection).eulerAngles.y;
                    if (side == 1)
                    {
                        rotationY += 180f;
                    }

                    if (m_randomizeRotation)
                    {
                        rotationY += Random.Range(-m_maxRotationVariation, m_maxRotationVariation);
                    }

                    GameObject prefab = m_vehiclePrefabs[Random.Range(0, m_vehiclePrefabs.Length)];
                    GameObject car = Instantiate(prefab, spawnPosition, Quaternion.Euler(0, rotationY, 0));
                    car.transform.SetParent(transform);
                    car.name = $"RoadsideCar_{m_spawnedCars.Count}";

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

                    currentDistance += m_minSpacing + Random.Range(-2f, 2f);
                }
            }

            Debug.Log($"[RoadSideSpawner] Generated {m_spawnedCars.Count} roadside cars");
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
            if (m_roadStart == null || m_roadEnd == null) return;

            Gizmos.color = m_gizmoColor;
            Gizmos.DrawLine(m_roadStart.position, m_roadEnd.position);

            Gizmos.color = m_gizmoColor * 0.5f;
            Vector3 direction = (m_roadEnd.position - m_roadStart.position).normalized;
            Vector3 right = Vector3.Cross(direction, Vector3.up).normalized;

            Gizmos.DrawLine(m_roadStart.position + right * m_roadEdgeOffset, 
                           m_roadEnd.position + right * m_roadEdgeOffset);
            Gizmos.DrawLine(m_roadStart.position - right * m_roadEdgeOffset, 
                           m_roadEnd.position - right * m_roadEdgeOffset);
        }

        public List<GameObject> GetSpawnedCars() => m_spawnedCars;
    }
}
