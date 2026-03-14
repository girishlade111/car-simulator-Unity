using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.World
{
    public class RoadNetwork : MonoBehaviour
    {
        public static RoadNetwork Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private float m_roadSnapDistance = 2f;
        [SerializeField] private bool m_snapToRoads = true;

        private List<RoadSegment> m_roads = new List<RoadSegment>();

        public IReadOnlyList<RoadSegment> Roads => m_roads;

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
            var roads = FindObjectsOfType<RoadSegment>();
            m_roads.AddRange(roads);
        }

        public void RegisterRoad(RoadSegment road)
        {
            if (!m_roads.Contains(road))
            {
                m_roads.Add(road);
            }
        }

        public void UnregisterRoad(RoadSegment road)
        {
            m_roads.Remove(road);
        }

        public RoadSegment GetClosestRoad(Vector3 position)
        {
            RoadSegment closest = null;
            float closestDist = float.MaxValue;

            foreach (var road in m_roads)
            {
                float dist = Vector3.Distance(position, road.Center);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = road;
                }
            }

            return closest;
        }

        public Vector3? SnapToRoadNetwork(Vector3 position)
        {
            if (!m_snapToRoads) return null;

            RoadSegment closest = GetClosestRoad(position);
            if (closest == null) return null;

            float distToRoad = Vector3.Distance(position, closest.Center);
            if (distToRoad > m_roadSnapDistance) return null;

            Vector3 snappedPos = position;
            snappedPos.y = closest.Center.y;

            return snappedPos;
        }

        public List<RoadSegment> GetConnectedRoads(RoadSegment road)
        {
            List<RoadSegment> connected = new List<RoadSegment>();

            foreach (var otherRoad in m_roads)
            {
                if (otherRoad == road) continue;

                if (IsConnected(road, otherRoad))
                {
                    connected.Add(otherRoad);
                }
            }

            return connected;
        }

        private bool IsConnected(RoadSegment roadA, RoadSegment roadB)
        {
            float connectDist = (roadA.Length + roadB.Length) / 2f;

            if (roadA.HasConnection("north"))
            {
                Vector3 connPoint = roadA.GetConnectionPoint("north");
                if (Vector3.Distance(connPoint, roadB.Center) < connectDist) return true;
            }
            if (roadA.HasConnection("south"))
            {
                Vector3 connPoint = roadA.GetConnectionPoint("south");
                if (Vector3.Distance(connPoint, roadB.Center) < connectDist) return true;
            }
            if (roadA.HasConnection("east"))
            {
                Vector3 connPoint = roadA.GetConnectionPoint("east");
                if (Vector3.Distance(connPoint, roadB.Center) < connectDist) return true;
            }
            if (roadA.HasConnection("west"))
            {
                Vector3 connPoint = roadA.GetConnectionPoint("west");
                if (Vector3.Distance(connPoint, roadB.Center) < connectDist) return true;
            }

            return false;
        }
    }
}
