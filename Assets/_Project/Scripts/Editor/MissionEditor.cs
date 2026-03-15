using UnityEngine;
using UnityEditor;

namespace CarSimulator.Editor
{
    public class MissionEditor : EditorWindow
    {
        [MenuItem("Car Simulator/Mission Editor")]
        public static void ShowWindow()
        {
            GetWindow<MissionEditor>("Mission Editor");
        }

        [SerializeField] private DriveMissionData m_missionData;
        [SerializeField] private MissionObjectiveType m_selectedObjectiveType = MissionObjectiveType.DriveToLocation;
        [SerializeField] private Vector3 m_targetPosition;
        [SerializeField] private float m_targetRadius = 5f;
        [SerializeField] private int m_objectiveCount = 1;

        private void OnGUI()
        {
            GUILayout.Label("Mission Creator", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            m_missionData = (DriveMissionData)EditorGUILayout.ObjectField("Mission Data", m_missionData, typeof(DriveMissionData), false);

            if (m_missionData == null)
            {
                if (GUILayout.Button("Create New Mission"))
                {
                    CreateNewMission();
                }
            }
            else
            {
                EditorGUILayout.LabelField("Mission: " + m_missionData.missionName);
                EditorGUILayout.LabelField("Stages: " + m_missionData.stages.Count);

                EditorGUILayout.Space();

                GUILayout.Label("Add Objective", EditorStyles.boldLabel);
                m_selectedObjectiveType = (MissionObjectiveType)EditorGUILayout.EnumPopup("Type", m_selectedObjectiveType);
                m_targetPosition = EditorGUILayout.Vector3Field("Target Position", m_targetPosition);
                m_targetRadius = EditorGUILayout.FloatField("Radius", m_targetRadius);
                m_objectiveCount = EditorGUILayout.IntField("Count", m_objectiveCount);

                if (GUILayout.Button("Add Checkpoint"))
                {
                    AddCheckpoint();
                }

                EditorGUILayout.Space();

                if (GUILayout.Button("Create Parking Zone"))
                {
                    CreateParkingZone();
                }

                if (GUILayout.Button("Create Apartment Trigger"))
                {
                    CreateApartmentTrigger();
                }

                if (GUILayout.Button("Create Mission Trigger"))
                {
                    CreateMissionTrigger();
                }
            }

            EditorGUILayout.Space();

            GUILayout.Label("Quick Start", EditorStyles.boldLabel);

            if (GUILayout.Button("Start Default Mission"))
            {
                StartDefaultMission();
            }

            if (GUILayout.Button("Create Mission Starter"))
            {
                CreateMissionStarter();
            }
        }

        private void CreateNewMission()
        {
            m_missionData = DriveMissionData.CreateDefault();
            Debug.Log("[MissionEditor] Created new mission: " + m_missionData.missionName);
        }

        private void AddCheckpoint()
        {
            GameObject checkpoint = new GameObject($"Checkpoint_{m_selectedObjectiveType}");
            checkpoint.transform.position = m_targetPosition;

            var trigger = checkpoint.AddComponent<MissionTrigger>();
            trigger.m_triggerType = MissionTrigger.TriggerType.Location;
            trigger.m_targetPosition = m_targetPosition;
            trigger.m_triggerRadius = m_targetRadius;

            Selection.activeGameObject = checkpoint;
            Debug.Log("[MissionEditor] Created checkpoint at " + m_targetPosition);
        }

        private void CreateParkingZone()
        {
            GameObject zone = new GameObject("ParkingZone");
            zone.transform.position = SceneView.lastActiveSceneView.camera.transform.position + Vector3.forward * 5f;

            BoxCollider collider = zone.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.size = new Vector3(5f, 2f, 10f);

            ParkingZone parkingZone = zone.AddComponent<ParkingZone>();
            parkingZone.m_zoneName = "Mission Parking";
            parkingZone.m_requiredParkingTime = 2f;

            Selection.activeGameObject = zone;
            Debug.Log("[MissionEditor] Created parking zone");
        }

        private void CreateApartmentTrigger()
        {
            GameObject apartment = new GameObject("MissionApartment");
            apartment.transform.position = SceneView.lastActiveSceneView.camera.transform.position + Vector3.forward * 10f;

            World.ApartmentEntrance entrance = apartment.AddComponent<World.ApartmentEntrance>();
            entrance.m_apartmentName = "Target Apartment";

            GameObject spawn = new GameObject("InteriorSpawn");
            spawn.transform.SetParent(apartment.transform);
            spawn.transform.localPosition = new Vector3(0, 0, 5f);
            entrance.m_interiorSpawn = spawn.transform;

            Selection.activeGameObject = apartment;
            Debug.Log("[MissionEditor] Created apartment trigger");
        }

        private void CreateMissionTrigger()
        {
            GameObject trigger = new GameObject("MissionStartTrigger");
            trigger.transform.position = SceneView.lastActiveSceneView.camera.transform.position + Vector3.forward * 3f;

            SphereCollider collider = trigger.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = 5f;

            MissionTrigger missionTrigger = trigger.AddComponent<MissionTrigger>();
            missionTrigger.m_triggerType = MissionTrigger.TriggerType.OnEnter;

            Selection.activeGameObject = trigger;
            Debug.Log("[MissionEditor] Created mission trigger");
        }

        private void StartDefaultMission()
        {
            var manager = MissionManager.Instance;
            if (manager != null)
            {
                manager.StartDefaultMission();
            }
            else
            {
                GameObject go = new GameObject("MissionManager");
                go.AddComponent<MissionLoopManager>();
                manager = MissionManager.Instance;
                manager.StartDefaultMission();
            }
        }

        private void CreateMissionStarter()
        {
            GameObject starter = new GameObject("MissionStarter");
            starter.transform.position = SceneView.lastActiveSceneView.camera.transform.position + Vector3.forward * 3f;

            MissionStarter missionStarter = starter.AddComponent<MissionStarter>();
            missionStarter.m_startOnTrigger = true;

            SphereCollider collider = starter.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = 5f;

            Selection.activeGameObject = starter;
            Debug.Log("[MissionEditor] Created mission starter");
        }
    }
}
