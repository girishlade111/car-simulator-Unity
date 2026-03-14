#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace CarSimulator.AI
{
    public class TrafficWaypointEditor : EditorWindow
    {
        private List<TrafficWaypoint> m_selectedWaypoints = new List<TrafficWaypoint>();
        private bool m_showConnections = true;
        private float m_connectionDistance = 20f;
        private bool m_autoConnect = true;

        [MenuItem("CarSimulator/Traffic Waypoint Editor")]
        public static void ShowWindow()
        {
            GetWindow<TrafficWaypointEditor>("Waypoint Editor");
        }

        private void OnGUI()
        {
            GUILayout.Label("Traffic Waypoint Editor", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (GUILayout.Button("Find All Waypoints"))
            {
                FindAllWaypoints();
            }

            if (GUILayout.Button("Auto-Connect All Waypoints"))
            {
                AutoConnectAll();
            }

            if (GUILayout.Button("Create Waypoint At Selection"))
            {
                CreateWaypointAtSelection();
            }

            EditorGUILayout.Space();
            m_showConnections = EditorGUILayout.Toggle("Show Connections", m_showConnections);
            m_connectionDistance = EditorGUILayout.FloatField("Connection Distance", m_connectionDistance);
            m_autoConnect = EditorGUILayout.Toggle("Auto-Connect on Create", m_autoConnect);

            EditorGUILayout.Space();
            
            int waypointCount = FindObjectsOfType<TrafficWaypoint>().Length;
            GUILayout.Label($"Total Waypoints: {waypointCount}", EditorStyles.label);
        }

        private void FindAllWaypoints()
        {
            var waypoints = FindObjectsOfType<TrafficWaypoint>();
            m_selectedWaypoints.Clear();
            m_selectedWaypoints.AddRange(waypoints);
        }

        private void AutoConnectAll()
        {
            var waypoints = FindObjectsOfType<TrafficWaypoint>();
            foreach (var wp in waypoints)
            {
                AutoConnectWaypoint(wp);
            }
            Debug.Log($"[WaypointEditor] Auto-connected {waypoints.Length} waypoints");
        }

        private void AutoConnectWaypoint(TrafficWaypoint waypoint)
        {
            var allWaypoints = FindObjectsOfType<TrafficWaypoint>();
            List<TrafficWaypoint> connections = new List<TrafficWaypoint>();

            foreach (var wp in allWaypoints)
            {
                if (wp == waypoint) continue;

                float dist = Vector3.Distance(waypoint.transform.position, wp.transform.position);
                if (dist <= m_connectionDistance)
                {
                    connections.Add(wp);
                }
            }
        }

        private void CreateWaypointAtSelection()
        {
            if (Selection.activeGameObject == null)
            {
                Debug.LogWarning("[WaypointEditor] No object selected");
                return;
            }

            GameObject wpObj = new GameObject("TrafficWaypoint");
            wpObj.transform.position = Selection.activeGameObject.transform.position;
            wpObj.transform.rotation = Selection.activeGameObject.transform.rotation;
            
            TrafficWaypoint waypoint = wpObj.AddComponent<TrafficWaypoint>();
            waypoint.m_connectDistance = m_connectionDistance;

            if (m_autoConnect)
            {
                AutoConnectWaypoint(waypoint);
            }

            Selection.activeGameObject = wpObj;
            Debug.Log($"[WaypointEditor] Created waypoint at {wpObj.transform.position}");
        }

        private void OnSceneGUI()
        {
            if (!m_showConnections) return;

            var waypoints = FindObjectsOfType<TrafficWaypoint>();
            foreach (var wp in waypoints)
            {
                foreach (var conn in wp.Connections)
                {
                    if (conn != null)
                    {
                        Handles.color = new Color(0f, 1f, 0f, 0.2f);
                        Handles.DrawLine(wp.transform.position, conn.transform.position);
                    }
                }
            }

            if (Selection.activeGameObject != null)
            {
                Handles.color = Color.yellow;
                Handles.DrawWireDisc(Selection.activeGameObject.transform.position, Vector3.up, m_connectionDistance);
            }
        }
    }
}
#endif
