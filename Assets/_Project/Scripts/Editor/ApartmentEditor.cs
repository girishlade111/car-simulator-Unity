using UnityEngine;
using UnityEditor;

namespace CarSimulator.Editor
{
    public class ApartmentEditor : EditorWindow
    {
        [MenuItem("Car Simulator/Apartment Tools")]
        public static void ShowWindow()
        {
            GetWindow<ApartmentEditor>("Apartment Tools");
        }

        [SerializeField] private string m_apartmentName = "Apartment";
        [SerializeField] private ApartmentEntrance.ApartmentType m_apartmentType = ApartmentEntrance.ApartmentType.Residential;
        [SerializeField] private ApartmentEntrance.TransitionType m_transitionType = ApartmentEntrance.TransitionType.SameScene;
        [SerializeField] private bool m_addOwnership = true;
        [SerializeField] private int m_price = 50000;

        private void OnGUI()
        {
            GUILayout.Label("Apartment Entrance Creator", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            m_apartmentName = EditorGUILayout.TextField("Apartment Name", m_apartmentName);
            m_apartmentType = (ApartmentEntrance.ApartmentType)EditorGUILayout.EnumPopup("Type", m_apartmentType);
            m_transitionType = (ApartmentEntrance.TransitionType)EditorGUILayout.EnumPopup("Transition Type", m_transitionType);

            EditorGUILayout.Space();
            GUILayout.Label("Ownership", EditorStyles.boldLabel);
            m_addOwnership = EditorGUILayout.Toggle("Can Be Purchased", m_addOwnership);

            if (m_addOwnership)
            {
                m_price = EditorGUILayout.IntField("Price", m_price);
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Create Apartment Entrance"))
            {
                CreateApartmentEntrance();
            }

            if (GUILayout.Button("Find All Entrances"))
            {
                FindAllEntrances();
            }
        }

        private void CreateApartmentEntrance()
        {
            GameObject entrance = new GameObject($"Entrance_{m_apartmentName}");
            entrance.transform.position = SceneView.lastActiveSceneView.camera.transform.position + Vector3.forward * 3f;

            var apartmentEntrance = entrance.AddComponent<ApartmentEntrance>();
            apartmentEntrance.m_apartmentName = m_apartmentName;
            apartmentEntrance.m_apartmentType = m_apartmentType;
            apartmentEntrance.m_transitionType = m_transitionType;

            GameObject exteriorSpawn = new GameObject("ExteriorSpawn");
            exteriorSpawn.transform.SetParent(entrance.transform);
            exteriorSpawn.transform.localPosition = new Vector3(0, 0, -3f);
            apartmentEntrance.m_exteriorSpawn = exteriorSpawn.transform;

            GameObject interiorSpawn = new GameObject("InteriorSpawn");
            interiorSpawn.transform.SetParent(entrance.transform);
            interiorSpawn.transform.localPosition = new Vector3(0, 0, 3f);
            apartmentEntrance.m_interiorSpawn = interiorSpawn.transform;

            GameObject door = GameObject.CreatePrimitive(PrimitiveType.Cube);
            door.name = "Door";
            door.transform.SetParent(entrance.transform);
            door.transform.localPosition = Vector3.zero;
            door.transform.localScale = new Vector3(2f, 3f, 0.2f);

            Material doorMat = new Material(Shader.Find("Standard"));
            doorMat.color = new Color(0.4f, 0.25f, 0.1f);
            door.GetComponent<Renderer>().material = doorMat;

            if (m_addOwnership)
            {
                var ownership = entrance.AddComponent<ApartmentOwnership>();
                ownership.m_price = m_price;
            }

            entrance.layer = LayerMask.NameToLayer("Prop");

            Selection.activeGameObject = entrance;
            Debug.Log($"[ApartmentEditor] Created apartment entrance: {m_apartmentName}");
        }

        private void FindAllEntrances()
        {
            var entrances = FindObjectsOfType<ApartmentEntrance>();
            var interiors = FindObjectsOfType<InteriorScene>();
            var ownerships = FindObjectsOfType<ApartmentOwnership>();

            Debug.Log($"[ApartmentEditor] Found:\n" +
                     $"  Apartment Entrances: {entrances.Length}\n" +
                     $"  Interior Scenes: {interiors.Length}\n" +
                     $"  Ownership Systems: {ownerships.Length}");

            List<GameObject> allObjects = new List<GameObject>();
            foreach (var e in entrances) allObjects.Add(e.gameObject);
            foreach (var i in interiors) allObjects.Add(i.gameObject);
            foreach (var o in ownerships) allObjects.Add(o.gameObject);

            if (allObjects.Count > 0)
            {
                Selection.objects = allObjects.ToArray();
            }
        }
    }
}
