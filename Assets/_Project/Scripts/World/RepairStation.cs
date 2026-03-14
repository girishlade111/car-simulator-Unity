using UnityEngine;
using CarSimulator.Vehicle;

namespace CarSimulator.World
{
    public class RepairStation : MonoBehaviour
    {
        [Header("Repair Settings")]
        [SerializeField] private float m_repairRate = 20f;
        [SerializeField] private float m_repairRange = 5f;
        [SerializeField] private bool m_autoRepair = true;
        [SerializeField] private bool m_refillNitrous = true;

        [Header("Visual")]
        [SerializeField] private GameObject m_repairEffect;
        [SerializeField] private Color m_stationColor = Color.cyan;
        [SerializeField] private Light m_stationLight;

        [Header("References")]
        [SerializeField] private Transform m_centerPoint;

        private bool m_isRepairing;
        private VehicleDamage m_nearbyVehicle;

        private void Start()
        {
            if (m_centerPoint == null)
                m_centerPoint = transform;

            CreateVisuals();
        }

        private void CreateVisuals()
        {
            if (m_stationLight == null)
            {
                GameObject lightObj = new GameObject("RepairLight");
                lightObj.transform.SetParent(transform);
                lightObj.transform.localPosition = Vector3.up * 3f;

                m_stationLight = lightObj.AddComponent<Light>();
                m_stationLight.color = m_stationColor;
                m_stationLight.intensity = 1f;
                m_stationLight.range = 10f;
            }
        }

        private void Update()
        {
            if (!m_autoRepair) return;

            FindNearbyVehicle();

            if (m_nearbyVehicle != null && m_isRepairing)
            {
                PerformRepair();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                StartRepair(other.GetComponent<VehicleDamage>());
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player") && m_nearbyVehicle == null)
            {
                StartRepair(other.GetComponent<VehicleDamage>());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                StopRepair();
            }
        }

        private void FindNearbyVehicle()
        {
            Collider[] hits = Physics.OverlapSphere(m_centerPoint.position, m_repairRange);
            
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    m_nearbyVehicle = hit.GetComponent<VehicleDamage>();
                    return;
                }
            }
        }

        private void StartRepair(VehicleDamage vehicle)
        {
            if (vehicle == null) return;

            m_isRepairing = true;
            m_nearbyVehicle = vehicle;

            if (m_repairEffect != null)
            {
                m_repairEffect.SetActive(true);
            }

            if (m_stationLight != null)
            {
                m_stationLight.intensity = 2f;
            }
        }

        private void StopRepair()
        {
            m_isRepairing = false;
            m_nearbyVehicle = null;

            if (m_repairEffect != null)
            {
                m_repairEffect.SetActive(false);
            }

            if (m_stationLight != null)
            {
                m_stationLight.intensity = 1f;
            }
        }

        private void PerformRepair()
        {
            if (m_nearbyVehicle == null) return;

            m_nearbyVehicle.Repair(m_repairRate * Time.deltaTime);

            if (m_refillNitrous)
            {
                var nitrous = m_nearbyVehicle.GetComponent<NitrousOxide>();
                if (nitrous != null)
                {
                    nitrous.AddNitrous(10f * Time.deltaTime);
                }
            }

            if (m_nearbyVehicle.HealthPercent >= 1f)
            {
                Debug.Log("[RepairStation] Vehicle fully repaired!");
                StopRepair();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = m_stationColor;
            Gizmos.DrawWireSphere(transform.position, m_repairRange);

            Gizmos.color = new Color(m_stationColor.r, m_stationColor.g, m_stationColor.b, 0.2f);
            Gizmos.DrawSphere(transform.position, m_repairRange);
        }
    }
}
