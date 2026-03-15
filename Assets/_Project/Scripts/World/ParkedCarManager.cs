using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.World
{
    public class ParkedCarManager : MonoBehaviour
    {
        public static ParkedCarManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private bool m_autoFindSpawnPoints = true;
        [SerializeField] private bool m_autoFindGroups = true;

        [Header("Organization")]
        [SerializeField] private Transform m_parkedCarsRoot;

        [Header("Registered Vehicles")]
        [SerializeField] private List<ParkedCar> m_allParkedCars = new List<ParkedCar>();
        [SerializeField] private List<ParkedCarSpawnPoint> m_spawnPoints = new List<ParkedCarSpawnPoint>();
        [SerializeField] private List<ParkedCarGroup> m_carGroups = new List<ParkedCarGroup>();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitializeRoot();
        }

        private void Start()
        {
            if (Application.isPlaying)
            {
                RegisterAllParkedCars();
            }
        }

        private void InitializeRoot()
        {
            if (m_parkedCarsRoot == null)
            {
                GameObject root = new GameObject("[ParkedVehicles]");
                root.transform.SetParent(transform);
                m_parkedCarsRoot = root.transform;
            }
        }

        private void RegisterAllParkedCars()
        {
            m_allParkedCars.Clear();
            m_spawnPoints.Clear();
            m_carGroups.Clear();

            if (m_autoFindSpawnPoints)
            {
                var spawnPoints = FindObjectsOfType<ParkedCarSpawnPoint>();
                m_spawnPoints.AddRange(spawnPoints);
            }

            if (m_autoFindGroups)
            {
                var groups = FindObjectsOfType<ParkedCarGroup>();
                m_carGroups.AddRange(groups);

                foreach (var group in groups)
                {
                    foreach (var car in group.GetSpawnedCars())
                    {
                        var parkedCar = car.GetComponent<ParkedCar>();
                        if (parkedCar != null)
                        {
                            m_allParkedCars.Add(parkedCar);
                        }
                    }
                }
            }

            var directCars = FindObjectsOfType<ParkedCar>();
            foreach (var car in directCars)
            {
                if (!m_allParkedCars.Contains(car))
                {
                    m_allParkedCars.Add(car);
                }
            }

            OrganizeInHierarchy();

            Debug.Log($"[ParkedCarManager] Registered {m_allParkedCars.Count} parked cars, {m_spawnPoints.Count} spawn points, {m_carGroups.Count} groups");
        }

        private void OrganizeInHierarchy()
        {
            if (m_parkedCarsRoot == null) return;

            foreach (var spawnPoint in m_spawnPoints)
            {
                var spawned = spawnPoint.GetSpawnedVehicle();
                if (spawned != null)
                {
                    spawned.transform.SetParent(m_parkedCarsRoot);
                }
            }

            foreach (var group in m_carGroups)
            {
                foreach (var car in group.GetSpawnedCars())
                {
                    if (car != null)
                    {
                        car.transform.SetParent(m_parkedCarsRoot);
                    }
                }
            }
        }

        public void RegisterParkedCar(ParkedCar car)
        {
            if (!m_allParkedCars.Contains(car))
            {
                m_allParkedCars.Add(car);
            }
        }

        public void UnregisterParkedCar(ParkedCar car)
        {
            m_allParkedCars.Remove(car);
        }

        public ParkedCar[] GetAllParkedCars()
        {
            return m_allParkedCars.ToArray();
        }

        public ParkedCar[] GetParkedCarsInRadius(Vector3 position, float radius)
        {
            List<ParkedCar> result = new List<ParkedCar>();

            foreach (var car in m_allParkedCars)
            {
                if (car == null) continue;

                float distance = Vector3.Distance(position, car.transform.position);
                if (distance <= radius)
                {
                    result.Add(car);
                }
            }

            return result.ToArray();
        }

        public ParkedCar FindNearestParkedCar(Vector3 position)
        {
            ParkedCar nearest = null;
            float nearestDist = float.MaxValue;

            foreach (var car in m_allParkedCars)
            {
                if (car == null) continue;

                float distance = Vector3.Distance(position, car.transform.position);
                if (distance < nearestDist)
                {
                    nearestDist = distance;
                    nearest = car;
                }
            }

            return nearest;
        }

        public void GetStatistics(out int totalCars, out int sedans, out int suvs, out int trucks, out int sports)
        {
            totalCars = m_allParkedCars.Count;
            sedans = 0;
            suvs = 0;
            trucks = 0;
            sports = 0;

            foreach (var car in m_allParkedCars)
            {
                if (car == null) continue;

                switch (car.GetCarType())
                {
                    case ParkedCar.ParkedCarType.Sedan:
                    case ParkedCar.ParkedCarType.Compact:
                        sedans++;
                        break;
                    case ParkedCar.ParkedCarType.SUV:
                    case ParkedCar.ParkedCarType.Van:
                        suvs++;
                        break;
                    case ParkedCar.ParkedCarType.Truck:
                        trucks++;
                        break;
                    case ParkedCar.ParkedCarType.Sports:
                        sports++;
                        break;
                }
            }
        }

        public Transform GetRoot() => m_parkedCarsRoot;
    }
}
