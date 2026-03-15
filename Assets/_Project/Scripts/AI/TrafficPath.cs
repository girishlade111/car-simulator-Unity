using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.AI
{
    [ExecuteInEditMode]
    public class TrafficPath : MonoBehaviour
    {
        [Header("Path Settings")]
        [SerializeField] private string m_pathName = "Traffic Path";
        [SerializeField] private PathType m_pathType = PathType.Road;
        [SerializeField] private float m_speedLimit = 60f;
        [SerializeField] private bool m_oneWay = false;

        [Header("Waypoints")]
        [SerializeField] private List<Transform> m_waypoints = new List<Transform>();
        [SerializeField] private Transform m_startPoint;
        [SerializeField] private Transform m_endPoint;

        [Header("Connections")]
        [SerializeField] private TrafficPath[] m_connectedPaths;
        [SerializeField] private bool m_canMerge;
        [SerializeField] private bool m_canSplit;

        [Header("Lane Settings")]
        [SerializeField] private int m_laneCount = 2;
        [SerializeField] private float m_laneWidth = 3.5f;

        [Header("Visualization")]
        [SerializeField] private bool m_showGizmo = true;
        [SerializeField] private Color m_pathColor = Color.cyan;

        public enum PathType
        {
            Road,
            Highway,
            Ramp,
            Driveway,
            Alley
        }

        private void OnEnable()
        {
            if (!Application.isPlaying)
            {
                CreateWaypointsIfEmpty();
            }
        }

        private void CreateWaypointsIfEmpty()
        {
            if (m_waypoints.Count == 0)
            {
                if (m_startPoint != null && m_endPoint != null)
                {
                    GenerateWaypointsFromEndpoints();
                }
            }
        }

        private void GenerateWaypointsFromEndpoints()
        {
            float distance = Vector3.Distance(m_startPoint.position, m_endPoint.position);
            int waypointCount = Mathf.CeilToInt(distance / 15f);

            Vector3 direction = (m_endPoint.position - m_startPoint.position).normalized;

            for (int i = 0; i <= waypointCount; i++)
            {
                float t = (float)i / waypointCount;
                Vector3 pos = Vector3.Lerp(m_startPoint.position, m_endPoint.position, t);

                GameObject wp = new GameObject($"Waypoint_{i}");
                wp.transform.position = pos;
                wp.transform.SetParent(transform);
                m_waypoints.Add(wp.transform);
            }
        }

        public void AddWaypoint(Vector3 position)
        {
            GameObject wp = new GameObject($"Waypoint_{m_waypoints.Count}");
            wp.transform.position = position;
            wp.transform.SetParent(transform);
            m_waypoints.Add(wp.transform);
        }

        public void RemoveWaypoint(int index)
        {
            if (index >= 0 && index < m_waypoints.Count)
            {
                if (m_waypoints[index] != null)
                {
                    DestroyImmediate(m_waypoints[index].gameObject);
                }
                m_waypoints.RemoveAt(index);
            }
        }

        public void ClearWaypoints()
        {
            foreach (var wp in m_waypoints)
            {
                if (wp != null)
                {
                    DestroyImmediate(wp.gameObject);
                }
            }
            m_waypoints.Clear();
        }

        public Transform GetNextWaypoint(int currentIndex)
        {
            if (m_waypoints.Count == 0) return null;

            int nextIndex = currentIndex + 1;
            if (nextIndex >= m_waypoints.Count)
            {
                if (m_connectedPaths != null && m_connectedPaths.Length > 0)
                {
                    TrafficPath nextPath = m_connectedPaths[Random.Range(0, m_connectedPaths.Length)];
                    if (nextPath != null)
                    {
                        return nextPath.GetWaypoint(0);
                    }
                }
                return m_oneWay ? null : m_waypoints[0];
            }

            return m_waypoints[nextIndex];
        }

        public Transform GetWaypoint(int index)
        {
            if (index >= 0 && index < m_waypoints.Count)
            {
                return m_waypoints[index];
            }
            return null;
        }

        public int GetWaypointCount() => m_waypoints.Count;

        public Vector3 GetLaneOffset(int laneIndex)
        {
            if (laneIndex == 0) return Vector3.zero;

            Vector3 right = Vector3.Cross(GetDirection(), Vector3.up).normalized;
            float offset = (laneIndex - (m_laneCount - 1) / 2f) * m_laneWidth;
            return right * offset;
        }

        public Vector3 GetDirection()
        {
            if (m_waypoints.Count < 2) return transform.forward;
            return (m_waypoints[1].position - m_waypoints[0].position).normalized;
        }

        private void OnDrawGizmos()
        {
            if (!m_showGizmo) return;

            Gizmos.color = m_pathColor;

            if (m_waypoints.Count > 1)
            {
                for (int i = 0; i < m_waypoints.Count - 1; i++)
                {
                    if (m_waypoints[i] != null && m_waypoints[i + 1] != null)
                    {
                        Gizmos.DrawLine(m_waypoints[i].position, m_waypoints[i + 1].position);
                    }
                }
            }

            Gizmos.color = Color.yellow;
            foreach (var wp in m_waypoints)
            {
                if (wp != null)
                {
                    Gizmos.DrawWireSphere(wp.position, 0.5f);
                }
            }

            if (m_startPoint != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(m_startPoint.position, Vector3.one);
            }

            if (m_endPoint != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(m_endPoint.position, Vector3.one);
            }
        }
    }

    public class TrafficPathManager : MonoBehaviour
    {
        public static TrafficPathManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private bool m_autoFindPaths = true;
        [SerializeField] private List<TrafficPath> m_allPaths = new List<TrafficPath>();

        [Header("Traffic Lights")]
        [SerializeField] private List<TrafficLight> m_trafficLights = new List<TrafficLight>();

        [Header("Intersections")]
        [SerializeField] private List<IntersectionController> m_intersections = new List<IntersectionController>();

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
            if (m_autoFindPaths)
            {
                FindAllPaths();
            }
        }

        private void FindAllPaths()
        {
            var paths = FindObjectsOfType<TrafficPath>();
            m_allPaths.AddRange(paths);

            var lights = FindObjectsOfType<TrafficLight>();
            m_trafficLights.AddRange(lights);

            var intersections = FindObjectsOfType<IntersectionController>();
            m_intersections.AddRange(intersections);

            Debug.Log($"[TrafficPathManager] Found {m_allPaths.Count} paths, {m_trafficLights.Count} lights");
        }

        public TrafficPath GetRandomPath()
        {
            if (m_allPaths.Count == 0) return null;
            return m_allPaths[Random.Range(0, m_allPaths.Count)];
        }

        public TrafficPath GetNearestPath(Vector3 position)
        {
            TrafficPath nearest = null;
            float nearestDist = float.MaxValue;

            foreach (var path in m_allPaths)
            {
                if (path == null) continue;

                float dist = Vector3.Distance(position, path.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = path;
                }
            }

            return nearest;
        }

        public TrafficLight GetNearestTrafficLight(Vector3 position)
        {
            TrafficLight nearest = null;
            float nearestDist = float.MaxValue;

            foreach (var light in m_trafficLights)
            {
                if (light == null) continue;

                float dist = Vector3.Distance(position, light.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = light;
                }
            }

            return nearest;
        }

        public void RegisterPath(TrafficPath path)
        {
            if (!m_allPaths.Contains(path))
            {
                m_allPaths.Add(path);
            }
        }
    }
}
