using UnityEngine;
using CarSimulator.Vehicle;

namespace CarSimulator.Multiplayer
{
    public class NetworkedPlayer : MonoBehaviour
    {
        [Header("Player Info")]
        [SerializeField] private int m_playerId;
        [SerializeField] private string m_playerName;
        [SerializeField] private Text m_nameLabel;

        [Header("Sync Settings")]
        [SerializeField] private float m_smoothSpeed = 10f;
        [SerializeField] private float m_rotationSmoothSpeed = 10f;

        [Header("Vehicle Sync")]
        [SerializeField] private GameObject m_vehicleMesh;
        [SerializeField] private WheelCollider[] m_wheelColliders;
        [SerializeField] private Transform[] m_wheelMeshes;

        [Header("Visual")]
        [SerializeField] private GameObject m_nameTag;
        [SerializeField] private TextMesh m_3dNameLabel;
        [SerializeField] private Renderer[] m_vehicleRenderers;

        private Vector3 m_targetPosition;
        private Quaternion m_targetRotation;
        private float m_currentSpeed;
        private int m_currentGear;
        private bool m_isInitialized;

        public int PlayerId => m_playerId;
        public string PlayerName => m_playerName;

        public void Initialize(int id, string name)
        {
            m_playerId = id;
            m_playerName = name;
            m_isInitialized = true;

            if (m_nameTag != null)
            {
                m_nameTag.SetActive(true);
            }

            if (m_3dNameLabel != null)
            {
                m_3dNameLabel.text = name;
            }

            CreateNameLabel();
            SetupVehicleComponents();

            Debug.Log($"[Network] Player spawned: {name} (ID: {id})");
        }

        private void CreateNameLabel()
        {
            GameObject labelObj = new GameObject("NameLabel");
            labelObj.transform.SetParent(transform);
            labelObj.transform.localPosition = new Vector3(0, 3f, 0);

            m_3dNameLabel = labelObj.AddComponent<TextMesh>();
            m_3dNameLabel.text = m_playerName;
            m_3dNameLabel.characterSize = 0.1f;
            m_3dNameLabel.fontSize = 60;
            m_3dNameLabel.anchor = TextAnchor.MiddleCenter;
            m_3dNameLabel.color = Color.white;
            m_3dNameLabel.fontStyle = FontStyle.Bold;

            MeshRenderer mr = labelObj.GetComponent<MeshRenderer>();
            mr.material = new Material(Shader.Find("GUI/Text Shader"));
            mr.material.color = Color.black;
        }

        private void SetupVehicleComponents()
        {
            if (m_vehicleMesh == null)
            {
                m_vehicleMesh = CreatePlaceholderVehicle();
            }

            m_vehicleRenderers = m_vehicleMesh.GetComponentsInChildren<Renderer>();
        }

        private GameObject CreatePlaceholderVehicle()
        {
            GameObject car = GameObject.CreatePrimitive(PrimitiveType.Cube);
            car.name = $"NetworkCar_{m_playerName}";
            car.transform.SetParent(transform);
            car.transform.localPosition = Vector3.zero;
            car.transform.localScale = new Vector3(2f, 1f, 4f);

            GameObject hood = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hood.transform.SetParent(car.transform);
            hood.transform.localPosition = new Vector3(0, 0.6f, 1f);
            hood.transform.localScale = new Vector3(1.8f, 0.6f, 1.5f);

            GameObject roof = GameObject.CreatePrimitive(PrimitiveType.Cube);
            roof.transform.SetParent(car.transform);
            roof.transform.localPosition = new Vector3(0, 1f, -0.3f);
            roof.transform.localScale = new Vector3(1.7f, 0.7f, 1.8f);

            return car;
        }

        public void UpdateState(Vector3 position, Quaternion rotation, float speed, int gear)
        {
            m_targetPosition = position;
            m_targetRotation = rotation;
            m_currentSpeed = speed;
            m_currentGear = gear;
        }

        private void Update()
        {
            if (!m_isInitialized) return;

            transform.position = Vector3.Lerp(transform.position, m_targetPosition, Time.deltaTime * m_smoothSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, m_targetRotation, Time.deltaTime * m_rotationSmoothSpeed);

            UpdateWheelVisuals();
            UpdateVehicleEffects();
            UpdateNameLabel();
        }

        private void UpdateWheelVisuals()
        {
            if (m_wheelMeshes == null || m_wheelColliders == null) return;

            for (int i = 0; i < m_wheelMeshes.Length && i < m_wheelColliders.Length; i++)
            {
                if (m_wheelMeshes[i] != null && m_wheelColliders[i] != null)
                {
                    m_wheelMeshes[i].GetChild(0).localRotation = m_wheelColliders[i].steerAngle * Quaternion.Euler(0, 1, 0);
                    m_wheelMeshes[i].GetChild(1).localRotation = m_wheelColliders[i].rpm * Time.deltaTime * 360f * Quaternion.Euler(1, 0, 0);
                }
            }
        }

        private void UpdateVehicleEffects()
        {
            if (m_vehicleRenderers == null) return;

            foreach (var renderer in m_vehicleRenderers)
            {
                if (renderer != null)
                {
                    float emission = Mathf.PingPong(Time.time * 2f, 1f) * 0.5f;
                    renderer.material.EnableKeyword("_EMISSION");
                }
            }

            float speedKmh = m_currentSpeed;
            if (speedKmh > 50)
            {
                CreateSpeedTrail();
            }
        }

        private void CreateSpeedTrail()
        {
            if (Random.value > 0.9f)
            {
                GameObject trail = new GameObject("SpeedTrail");
                trail.transform.position = transform.position - transform.forward * 2f;
                trail.transform.SetParent(transform);

                var line = trail.AddComponent<LineRenderer>();
                line.startWidth = 0.1f;
                line.endWidth = 0.05f;
                line.material = new Material(Shader.Find("Sprites/Default"));
                line.startColor = new Color(1, 1, 1, 0.5f);
                line.endColor = new Color(1, 1, 1, 0f);
                line.positionCount = 2;
                line.SetPosition(0, trail.transform.position);
                line.SetPosition(1, trail.transform.position - transform.forward * 3f);

                Destroy(trail, 0.5f);
            }
        }

        private void UpdateNameLabel()
        {
            if (m_3dNameLabel != null)
            {
                m_3dNameLabel.transform.rotation = Camera.main.transform.rotation;
            }
        }

        public void SetColor(Color color)
        {
            if (m_vehicleRenderers != null)
            {
                foreach (var renderer in m_vehicleRenderers)
                {
                    renderer.material.color = color;
                }
            }
        }

        public float GetCurrentSpeed() => m_currentSpeed;
        public int GetCurrentGear() => m_currentGear;
    }
}
