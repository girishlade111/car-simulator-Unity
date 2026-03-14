using UnityEngine;
using UnityEngine.UI;
using CarSimulator.Vehicle;
using CarSimulator.World;

namespace CarSimulator.UI
{
    public class CarStatistics : MonoBehaviour
    {
        [Header("Stats UI")]
        [SerializeField] private GameObject m_statsPanel;
        [SerializeField] private Text m_vehicleNameText;

        [Header("Performance Stats")]
        [SerializeField] private Text m_topSpeedText;
        [SerializeField] private Text m_accelerationText;
        [SerializeField] private Text m_handlingText;
        [SerializeField] private Text m_brakingText;

        [Header("Current Stats")]
        [SerializeField] private Text m_currentSpeedText;
        [SerializeField] private Text m_rpmText;
        [SerializeField] private Text m_gearText;

        [Header("Distance Stats")]
        [SerializeField] private Text m_totalDistanceText;
        [SerializeField] private Text m_sessionDistanceText;

        [Header("Extended Stats")]
        [SerializeField] private Text m_driftTimeText;
        [SerializeField] private Text m_raceWinsText;
        [SerializeField] private Text m_driveTimeText;
        [SerializeField] private Text m_topSpeedRecordText;
        [SerializeField] private Text m_sessionTimeText;
        [SerializeField] private Text m_avgSpeedText;
        [SerializeField] private Text m_hardBrakesText;
        [SerializeField] private Text m_gearShiftsText;

        [Header("References")]
        [SerializeField] private VehiclePhysics m_vehiclePhysics;
        [SerializeField] private GearSystem m_gearSystem;
        [SerializeField] private VehicleSpawner m_vehicleSpawner;

        private float m_totalDistance;
        private float m_sessionDistance;
        private float m_totalDriftTime;
        private float m_sessionDriftTime;
        private float m_totalDriveTime;
        private float m_sessionDriveTime;
        private float m_topSpeedRecord;
        private int m_raceWins;
        private int m_hardBrakes;
        private int m_gearShifts;
        private float m_speedAccumulator;
        private int m_speedSamples;

        private const string PREF_TOTAL_DISTANCE = "Stats_TotalDistance";
        private const string PREF_TOTAL_DRIFT = "Stats_TotalDrift";
        private const string PREF_TOTAL_DRIVE = "Stats_TotalDrive";
        private const string PREF_TOP_SPEED = "Stats_TopSpeed";
        private const string PREF_RACE_WINS = "Stats_RaceWins";
        private const string PREF_HARD_BRAKES = "Stats_HardBrakes";
        private const string PREF_GEAR_SHIFTS = "Stats_GearShifts";

        private void Start()
        {
            LoadStats();
            FindVehicleComponents();
        }

        private void OnDestroy()
        {
            SaveStats();
        }

        private void LoadStats()
        {
            m_totalDistance = PlayerPrefs.GetFloat(PREF_TOTAL_DISTANCE, 0f);
            m_totalDriftTime = PlayerPrefs.GetFloat(PREF_TOTAL_DRIFT, 0f);
            m_totalDriveTime = PlayerPrefs.GetFloat(PREF_TOTAL_DRIVE, 0f);
            m_topSpeedRecord = PlayerPrefs.GetFloat(PREF_TOP_SPEED, 0f);
            m_raceWins = PlayerPrefs.GetInt(PREF_RACE_WINS, 0);
            m_hardBrakes = PlayerPrefs.GetInt(PREF_HARD_BRAKES, 0);
            m_gearShifts = PlayerPrefs.GetInt(PREF_GEAR_SHIFTS, 0);
        }

        public void SaveStats()
        {
            PlayerPrefs.SetFloat(PREF_TOTAL_DISTANCE, m_totalDistance);
            PlayerPrefs.SetFloat(PREF_TOTAL_DRIFT, m_totalDriftTime);
            PlayerPrefs.SetFloat(PREF_TOTAL_DRIVE, m_totalDriveTime);
            PlayerPrefs.SetFloat(PREF_TOP_SPEED, m_topSpeedRecord);
            PlayerPrefs.SetInt(PREF_RACE_WINS, m_raceWins);
            PlayerPrefs.SetInt(PREF_HARD_BRAKES, m_hardBrakes);
            PlayerPrefs.SetInt(PREF_GEAR_SHIFTS, m_gearShifts);
            PlayerPrefs.Save();
        }

        private void FindVehicleComponents()
        {
            if (m_vehicleSpawner == null)
                m_vehicleSpawner = FindObjectOfType<VehicleSpawner>();

            if (m_vehicleSpawner?.CurrentVehicle != null)
            {
                GameObject vehicle = m_vehicleSpawner.CurrentVehicle;
                m_vehiclePhysics = vehicle.GetComponent<VehiclePhysics>();
                m_gearSystem = vehicle.GetComponent<GearSystem>();
            }
        }

        private void Update()
        {
            if (m_vehiclePhysics == null)
            {
                FindVehicleComponents();
                if (m_vehiclePhysics == null) return;
            }

            UpdateCurrentStats();
            UpdateDistanceStats();
            UpdateExtendedStats();
        }

        private void UpdateCurrentStats()
        {
            if (m_currentSpeedText != null)
            {
                m_currentSpeedText.text = $"{m_vehiclePhysics.CurrentSpeed:F1} km/h";
            }

            if (m_rpmText != null && m_gearSystem != null)
            {
                m_rpmText.text = $"{m_gearSystem.CurrentRPM:F0} RPM";
            }

            if (m_gearText != null && m_gearSystem != null)
            {
                int gear = m_gearSystem.CurrentGear;
                m_gearText.text = gear == 0 ? "N" : gear.ToString();
            }

            float currentSpeed = m_vehiclePhysics.CurrentSpeed;
            if (currentSpeed > m_topSpeedRecord)
            {
                m_topSpeedRecord = currentSpeed;
            }

            m_speedAccumulator += currentSpeed;
            m_speedSamples++;

            if (m_vehiclePhysics.CurrentBrakeInput > 0.7f && currentSpeed > 20f)
            {
                m_hardBrakes++;
                m_sessionDistance += 0;
            }

            CheckForDrift();
        }

        private void CheckForDrift()
        {
            var input = m_vehicleSpawner?.CurrentVehicle?.GetComponent<VehicleInput>();
            if (input != null && input.IsDrifting)
            {
                m_sessionDriftTime += Time.deltaTime;
                m_totalDriftTime += Time.deltaTime;
            }
        }

        private void UpdateDistanceStats()
        {
            if (m_vehiclePhysics == null) return;

            float speed = m_vehiclePhysics.CurrentSpeed / 3.6f;
            float distanceThisFrame = speed * Time.deltaTime;

            if (distanceThisFrame > 0)
            {
                m_sessionDistance += distanceThisFrame;
                m_totalDistance += distanceThisFrame;
                m_sessionDriveTime += Time.deltaTime;
                m_totalDriveTime += Time.deltaTime;
            }

            if (m_totalDistanceText != null)
            {
                m_totalDistanceText.text = FormatDistance(m_totalDistance);
            }

            if (m_sessionDistanceText != null)
            {
                m_sessionDistanceText.text = FormatDistance(m_sessionDistance);
            }
        }

        private void UpdateExtendedStats()
        {
            if (m_driftTimeText != null)
            {
                m_driftTimeText.text = FormatTime(m_sessionDriftTime);
            }

            if (m_driveTimeText != null)
            {
                m_driveTimeText.text = FormatTime(m_totalDriveTime);
            }

            if (m_sessionTimeText != null)
            {
                m_sessionTimeText.text = FormatTime(m_sessionDriveTime);
            }

            if (m_topSpeedRecordText != null)
            {
                m_topSpeedRecordText.text = $"{m_topSpeedRecord:F1} km/h";
            }

            if (m_raceWinsText != null)
            {
                m_raceWinsText.text = m_raceWins.ToString();
            }

            if (m_avgSpeedText != null && m_speedSamples > 0)
            {
                float avgSpeed = m_speedAccumulator / m_speedSamples;
                m_avgSpeedText.text = $"{avgSpeed:F1} km/h";
            }

            if (m_hardBrakesText != null)
            {
                m_hardBrakesText.text = m_hardBrakes.ToString();
            }

            if (m_gearShiftsText != null && m_gearSystem != null)
            {
                m_gearShiftsText.text = m_gearShifts.ToString();
            }
        }

        public void RecordGearShift()
        {
            m_gearShifts++;
        }

        public void AddRaceWin()
        {
            m_raceWins++;
            SaveStats();
        }

        private string FormatDistance(float meters)
        {
            if (meters < 1000)
            {
                return $"{meters:F0}m";
            }
            return $"{meters / 1000:F1}km";
        }

        private string FormatTime(float seconds)
        {
            int hours = Mathf.FloorToInt(seconds / 3600);
            int minutes = Mathf.FloorToInt((seconds % 3600) / 60);
            int secs = Mathf.FloorToInt(seconds % 60);

            if (hours > 0)
                return $"{hours}h {minutes}m";
            if (minutes > 0)
                return $"{minutes}m {secs}s";
            return $"{secs}s";
        }

        public void ShowStats()
        {
            if (m_statsPanel != null)
            {
                m_statsPanel.SetActive(true);
                UpdatePerformanceStats();
                UpdateExtendedStats();
            }
        }

        public void HideStats()
        {
            if (m_statsPanel != null)
            {
                m_statsPanel.SetActive(false);
            }
        }

        private void UpdatePerformanceStats()
        {
            if (m_vehiclePhysics?.m_tuning != null)
            {
                VehicleTuning tuning = m_vehiclePhysics.m_tuning;

                if (m_topSpeedText != null)
                    m_topSpeedText.text = $"{tuning.maxSpeed:F0} km/h";

                if (m_accelerationText != null)
                    m_accelerationText.text = $"{tuning.engineForce / 10:F0} HP";

                if (m_handlingText != null)
                    m_handlingText.text = $"{tuning.maxSteerAngle:F0}°";

                if (m_brakingText != null)
                    m_brakingText.text = $"{tuning.brakeForce / 100:F0}kN";
            }

            if (m_vehicleNameText != null)
            {
                m_vehicleNameText.text = "Current Vehicle";
            }
        }

        public void ResetSessionStats()
        {
            m_sessionDistance = 0f;
            m_sessionDriftTime = 0f;
            m_sessionDriveTime = 0f;
            m_speedAccumulator = 0f;
            m_speedSamples = 0;
            m_hardBrakes = 0;
            m_gearShifts = 0;
        }

        public void ResetAllStats()
        {
            m_totalDistance = 0f;
            m_totalDriftTime = 0f;
            m_totalDriveTime = 0f;
            m_topSpeedRecord = 0f;
            m_raceWins = 0;
            m_hardBrakes = 0;
            m_gearShifts = 0;
            ResetSessionStats();
            SaveStats();
        }

        public float GetTotalDistance() => m_totalDistance;
        public float GetSessionDistance() => m_sessionDistance;
        public float GetTotalDriftTime() => m_totalDriftTime;
        public float GetTopSpeedRecord() => m_topSpeedRecord;
        public int GetRaceWins() => m_raceWins;
    }
}
