using UnityEngine;
using System.Collections.Generic;
using CarSimulator.AI;

namespace CarSimulator.World
{
    public class PedestrianManager : MonoBehaviour
    {
        public static PedestrianManager Instance { get; private set; }

        [Header("Spawn Settings")]
        [SerializeField] private int m_maxPedestrians = 20;
        [SerializeField] private float m_spawnRadius = 50f;
        [SerializeField] private float m_despawnDistance = 80f;

        [Header("Pedestrian Prefabs")]
        [SerializeField] private GameObject[] m_pedestrianPrefabs;

        [Header("References")]
        [SerializeField] private Transform m_playerTransform;

        private List<Pedestrian> m_activePedestrians = new List<Pedestrian>();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            FindPlayer();
            SpawnInitialPedestrians();
        }

        private void Update()
        {
            ManagePedestrians();
        }

        private void FindPlayer()
        {
            if (m_playerTransform == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    m_playerTransform = player.transform;
                }
            }
        }

        private void SpawnInitialPedestrians()
        {
            for (int i = 0; i < m_maxPedestrians; i++)
            {
                SpawnPedestrian();
            }
        }

        private void SpawnPedestrian()
        {
            if (m_pedestrianPrefabs == null || m_pedestrianPrefabs.Length == 0) return;

            Vector3 spawnPos = GetRandomSpawnPosition();
            GameObject prefab = m_pedestrianPrefabs[Random.Range(0, m_pedestrianPrefabs.Length)];
            
            GameObject pedObj = Instantiate(prefab, spawnPos, Quaternion.identity);
            Pedestrian pedestrian = pedObj.GetComponent<Pedestrian>();
            
            if (pedestrian == null)
            {
                pedestrian = pedObj.AddComponent<Pedestrian>();
            }

            m_activePedestrians.Add(pedestrian);
        }

        private Vector3 GetRandomSpawnPosition()
        {
            if (m_playerTransform == null) return Vector3.zero;

            Vector2 randomPoint = Random.insideUnitCircle * m_spawnRadius;
            return new Vector3(randomPoint.x, 0, randomPoint.y) + m_playerTransform.position;
        }

        private void ManagePedestrians()
        {
            if (m_playerTransform == null)
            {
                FindPlayer();
                if (m_playerTransform == null) return;
            }

            for (int i = m_activePedestrians.Count - 1; i >= 0; i--)
            {
                if (m_activePedestrians[i] == null)
                {
                    m_activePedestrians.RemoveAt(i);
                    continue;
                }

                float dist = Vector3.Distance(m_playerTransform.position, m_activePedestrians[i].transform.position);
                
                if (dist > m_despawnDistance)
                {
                    Destroy(m_activePedestrians[i].gameObject);
                    m_activePedestrians.RemoveAt(i);
                }
            }

            while (m_activePedestrians.Count < m_maxPedestrians)
            {
                SpawnPedestrian();
            }
        }
    }
}
