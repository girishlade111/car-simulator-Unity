using UnityEngine;

namespace CarSimulator.World
{
    [ExecuteInEditMode]
    public class ParkedCarSpawnPoint : MonoBehaviour
    {
        [Header("Vehicle Settings")]
        [SerializeField] private GameObject m_vehiclePrefab;
        [SerializeField] private bool m_useRandomFromList = false;
        [SerializeField] private GameObject[] m_randomVehicleList;
        [SerializeField] private bool m_randomizeRotation = false;
        [SerializeField] private float m_rotationOffset = 0f;

        [Header("Spawned Reference")]
        [SerializeField] private GameObject m_spawnedVehicle;

        [Header("Visualization")]
        [SerializeField] private bool m_showGizmo = true;
        [SerializeField] private Color m_gizmoColor = Color.cyan;

        private void Start()
        {
            if (Application.isPlaying)
            {
                SpawnVehicle();
            }
        }

        private void OnEnable()
        {
            if (!Application.isPlaying && m_spawnedVehicle == null)
            {
                SpawnVehicle();
            }
        }

        private void OnDisable()
        {
            if (!Application.isPlaying && m_spawnedVehicle != null)
            {
                DestroyImmediate(m_spawnedVehicle);
            }
        }

        public void SpawnVehicle()
        {
            if (m_spawnedVehicle != null)
            {
                DestroyImmediate(m_spawnedVehicle);
            }

            GameObject prefabToSpawn = GetVehiclePrefab();
            if (prefabToSpawn == null)
            {
                Debug.LogWarning("[ParkedCarSpawnPoint] No vehicle prefab assigned!", this);
                return;
            }

            m_spawnedVehicle = Instantiate(prefabToSpawn, transform.position, GetRotation());
            m_spawnedVehicle.transform.SetParent(transform.parent);
            m_spawnedVehicle.name = prefabToSpawn.name + "_Parked";

            var parkedCar = m_spawnedVehicle.GetComponent<ParkedCar>();
            if (parkedCar == null)
            {
                parkedCar = m_spawnedVehicle.AddComponent<ParkedCar>();
            }
            parkedCar.SetSpawnPoint(this);
        }

        private GameObject GetVehiclePrefab()
        {
            if (m_useRandomFromList && m_randomVehicleList != null && m_randomVehicleList.Length > 0)
            {
                return m_randomVehicleList[Random.Range(0, m_randomVehicleList.Length)];
            }
            return m_vehiclePrefab;
        }

        private Quaternion GetRotation()
        {
            float yRotation = m_rotationOffset;

            if (m_randomizeRotation)
            {
                yRotation += Random.Range(-15f, 15f);
            }

            return Quaternion.Euler(0, yRotation, 0);
        }

        public void RefreshSpawn()
        {
            SpawnVehicle();
        }

        public void ClearSpawn()
        {
            if (m_spawnedVehicle != null)
            {
                DestroyImmediate(m_spawnedVehicle);
                m_spawnedVehicle = null;
            }
        }

        private void OnDrawGizmos()
        {
            if (!m_showGizmo) return;

            Gizmos.color = m_gizmoColor;
            Gizmos.DrawCube(transform.position + Vector3.up * 0.5f, new Vector3(2f, 1f, 4f));

            Gizmos.color = m_gizmoColor * 0.5f;
            Vector3 direction = transform.forward * 2f;
            Gizmos.DrawLine(transform.position, transform.position + direction);
        }

        public GameObject GetSpawnedVehicle() => m_spawnedVehicle;
    }
}
