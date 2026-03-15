using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.AI
{
    public class TrafficLight : MonoBehaviour
    {
        public enum LightState
        {
            Red,
            Yellow,
            Green,
            RedYellow
        }

        [Header("Light Settings")]
        [SerializeField] private LightState m_currentState = LightState.Red;
        [SerializeField] private float m_greenDuration = 10f;
        [SerializeField] private float m_yellowDuration = 3f;
        [SerializeField] private float m_allRedDuration = 2f;

        [Header("Visual")]
        [SerializeField] private Renderer m_lightRenderer;
        [SerializeField] private Material m_redMaterial;
        [SerializeField] private Material m_yellowMaterial;
        [SerializeField] private Material m_greenMaterial;
        [SerializeField] private Light[] m_redLights;
        [SerializeField] private Light[] m_yellowLights;
        [SerializeField] private Light[] m_greenLights;

        [Header("Directions")]
        [SerializeField] private bool m_controlledDirections;
        [SerializeField] private Transform[] m_stopLines;

        [Header("Linked Lights")]
        [SerializeField] private TrafficLight m_linkedLight;
        [SerializeField] private bool m_isPartOfIntersection;

        private float m_stateTimer;
        private bool m_isGreenForPrimary;

        public LightState CurrentState => m_currentState;
        public bool IsGreen => m_currentState == LightState.Green || m_currentState == LightState.RedYellow;

        private void Start()
        {
            m_stateTimer = 0;
            SetLightState(LightState.Red);
        }

        private void Update()
        {
            m_stateTimer += Time.deltaTime;
            UpdateLightState();
        }

        private void UpdateLightState()
        {
            float duration = GetStateDuration();

            if (m_stateTimer >= duration)
            {
                m_stateTimer = 0;
                TransitionToNextState();
            }
        }

        private float GetStateDuration()
        {
            switch (m_currentState)
            {
                case LightState.Green:
                    return m_greenDuration;
                case LightState.Yellow:
                    return m_yellowDuration;
                case LightState.Red:
                case LightState.RedYellow:
                    return m_allRedDuration;
                default:
                    return m_greenDuration;
            }
        }

        private void TransitionToNextState()
        {
            switch (m_currentState)
            {
                case LightState.Red:
                    SetLightState(LightState.RedYellow);
                    break;
                case LightState.RedYellow:
                    SetLightState(LightState.Green);
                    if (m_linkedLight != null)
                    {
                        m_linkedLight.SetLightState(LightState.Red);
                    }
                    break;
                case LightState.Green:
                    SetLightState(LightState.Yellow);
                    break;
                case LightState.Yellow:
                    SetLightState(LightState.Red);
                    break;
            }
        }

        public void SetLightState(LightState newState)
        {
            m_currentState = newState;
            UpdateVisuals();
        }

        public void ForceGreen()
        {
            SetLightState(LightState.Green);
            if (m_linkedLight != null)
            {
                m_linkedLight.SetLightState(LightState.Red);
            }
        }

        public void ForceRed()
        {
            SetLightState(LightState.Red);
            if (m_linkedLight != null)
            {
                m_linkedLight.ForceGreen();
            }
        }

        private void UpdateVisuals()
        {
            if (m_lightRenderer == null) return;

            bool redActive = m_currentState == LightState.Red || m_currentState == LightState.RedYellow;
            bool yellowActive = m_currentState == LightState.Yellow;
            bool greenActive = m_currentState == LightState.Green;

            foreach (var light in m_redLights)
            {
                if (light != null) light.enabled = redActive;
            }

            foreach (var light in m_yellowLights)
            {
                if (light != null) light.enabled = yellowActive;
            }

            foreach (var light in m_greenLights)
            {
                if (light != null) light.enabled = greenActive;
            }

            if (m_redMaterial != null)
                m_redMaterial.EnableKeyword("_EMISSION");
            if (m_yellowMaterial != null)
                m_yellowMaterial.EnableKeyword("_EMISSION");
            if (m_greenMaterial != null)
                m_greenMaterial.EnableKeyword("_EMISSION");
        }

        public bool ShouldStop(Vector3 vehiclePosition)
        {
            if (m_currentState == LightState.Red || m_currentState == LightState.Yellow)
            {
                return true;
            }
            return false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = GetStateColor();
            Gizmos.DrawSphere(transform.position, 0.5f);

            if (m_linkedLight != null)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(transform.position, m_linkedLight.transform.position);
            }
        }

        private Color GetStateColor()
        {
            switch (m_currentState)
            {
                case LightState.Red:
                    return Color.red;
                case LightState.Yellow:
                    return Color.yellow;
                case LightState.Green:
                    return Color.green;
                case LightState.RedYellow:
                    return new Color(1f, 0.5f, 0f);
                default:
                    return Color.white;
            }
        }
    }

    public class TrafficLightController : MonoBehaviour
    {
        public static TrafficLightController Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private bool m_enabled = true;
        [SerializeField] private float m_globalGreenDuration = 10f;
        [SerializeField] private float m_globalYellowDuration = 3f;

        [Header("Traffic Lights")]
        [SerializeField] private List<TrafficLight> m_allLights = new List<TrafficLight>();

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
            FindAllLights();
        }

        private void FindAllLights()
        {
            var lights = FindObjectsOfType<TrafficLight>();
            m_allLights.AddRange(lights);

            Debug.Log($"[TrafficLightController] Found {m_allLights.Count} traffic lights");
        }

        public void RegisterLight(TrafficLight light)
        {
            if (!m_allLights.Contains(light))
            {
                m_allLights.Add(light);
            }
        }

        public void SetGreenDuration(float duration)
        {
            m_globalGreenDuration = duration;
            foreach (var light in m_allLights)
            {
                light.SetValue("m_greenDuration", duration);
            }
        }

        public void SetYellowDuration(float duration)
        {
            m_globalYellowDuration = duration;
            foreach (var light in m_allLights)
            {
                light.SetValue("m_yellowDuration", duration);
            }
        }

        public TrafficLight GetNearestLight(Vector3 position)
        {
            TrafficLight nearest = null;
            float nearestDist = float.MaxValue;

            foreach (var light in m_allLights)
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

        public void EmergencyOverride(bool enable)
        {
            if (enable)
            {
                foreach (var light in m_allLights)
                {
                    light.ForceRed();
                }
            }
        }
    }

    public class IntersectionController : MonoBehaviour
    {
        [Header("Intersection Lights")]
        [SerializeField] private TrafficLight[] m_northSouthLights;
        [SerializeField] private TrafficLight[] m_eastWestLights;

        [Header("Settings")]
        [SerializeField] private float m_cycleDuration = 30f;
        [SerializeField] private float m_yellowDuration = 3f;
        [SerializeField] private bool m_pedestrianCrossing = true;

        [Header("Waypoints")]
        [SerializeField] private Transform[] m_northWaypoints;
        [SerializeField] private Transform[] m_southWaypoints;
        [SerializeField] private Transform[] m_eastWaypoints;
        [SerializeField] private Transform[] m_westWaypoints;

        private float m_timer;
        private bool m_northSouthGreen = true;

        private void Start()
        {
            InitializeLights();
        }

        private void Update()
        {
            m_timer += Time.deltaTime;

            if (m_timer >= m_cycleDuration)
            {
                m_timer = 0;
                SwitchPhase();
            }
        }

        private void InitializeLights()
        {
            if (m_northSouthLights != null)
            {
                foreach (var light in m_northSouthLights)
                {
                    light.SetLightState(TrafficLight.LightState.Green);
                }
            }

            if (m_eastWestLights != null)
            {
                foreach (var light in m_eastWestLights)
                {
                    light.SetLightState(TrafficLight.LightState.Red);
                }
            }
        }

        private void SwitchPhase()
        {
            m_northSouthGreen = !m_northSouthGreen;

            if (m_northSouthGreen)
            {
                SetLights(m_northSouthLights, TrafficLight.LightState.Green);
                SetLights(m_eastWestLights, TrafficLight.LightState.Red);
            }
            else
            {
                SetLights(m_northSouthLights, TrafficLight.LightState.Red);
                SetLights(m_eastWestLights, TrafficLight.LightState.Green);
            }
        }

        private void SetLights(TrafficLight[] lights, TrafficLight.LightState state)
        {
            if (lights == null) return;

            foreach (var light in lights)
            {
                if (light != null)
                {
                    light.SetLightState(state);
                }
            }
        }

        public bool CanPass(Vector3 position, Vector3 direction)
        {
            Vector3 forward = transform.forward;
            float dot = Vector3.Dot(direction.normalized, forward.normalized);

            if (dot > 0.5f)
            {
                return m_northSouthGreen;
            }
            else if (dot < -0.5f)
            {
                return !m_northSouthGreen;
            }

            return true;
        }
    }
}
