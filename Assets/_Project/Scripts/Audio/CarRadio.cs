using UnityEngine;
using System.Collections.Generic;
using CarSimulator.Vehicle;

namespace CarSimulator.Audio
{
    public class CarRadio : MonoBehaviour
    {
        [Header("Radio Settings")]
        [SerializeField] private bool m_radioEnabled = true;
        [SerializeField] private KeyCode m_nextStationKey = KeyCode.M;
        [SerializeField] private KeyCode m_toggleKey = KeyCode.R;
        [SerializeField] private float m_defaultVolume = 0.5f;

        [Header("Stations")]
        [SerializeField] private RadioStation[] m_stations;
        [SerializeField] private int m_currentStationIndex;

        [Header("Audio")]
        [SerializeField] private AudioSource m_radioSource;
        [SerializeField] private AudioSource m_engineSource;

        [Header("UI")]
        [SerializeField] private UnityEngine.UI.Text m_stationNameText;
        [SerializeField] private UnityEngine.UI.Text m_frequencyText;

        private bool m_isOn;
        private float m_targetVolume;

        [System.Serializable]
        public class RadioStation
        {
            public string name;
            public float frequency;
            public AudioClip[] songs;
        }

        private void Start()
        {
            SetupAudioSource();
            InitializeStations();
            
            if (m_radioSource != null)
            {
                m_radioSource.volume = 0;
            }
        }

        private void SetupAudioSource()
        {
            if (m_radioSource == null)
            {
                m_radioSource = gameObject.AddComponent<AudioSource>();
            }
            
            m_radioSource.loop = true;
            m_radioSource.playOnAwake = false;
            m_radioSource.spatialBlend = 1f;
            m_radioSource.minDistance = 1f;
            m_radioSource.maxDistance = 10f;
        }

        private void InitializeStations()
        {
            if (m_stations == null || m_stations.Length == 0)
            {
                m_stations = new RadioStation[5];
                
                m_stations[0] = new RadioStation { name = "Rock FM", frequency = 101.5f };
                m_stations[1] = new RadioStation { name = "Pop Hits", frequency = 104.3f };
                m_stations[2] = new RadioStation { name = "Jazz Radio", frequency = 92.7f };
                m_stations[3] = new RadioStation { name = "Classic Rock", frequency = 98.1f };
                m_stations[4] = new RadioStation { name = "Electronic", frequency = 106.9f };
            }
        }

        private void Update()
        {
            if (!m_radioEnabled) return;

            HandleInput();
            UpdateRadioState();
            UpdateUI();
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(m_toggleKey))
            {
                ToggleRadio();
            }

            if (Input.GetKeyDown(m_nextStationKey))
            {
                NextStation();
            }
        }

        private void UpdateRadioState()
        {
            if (!m_isOn || m_radioSource == null) return;

            float currentVolume = m_radioSource.volume;
            m_radioSource.volume = Mathf.Lerp(currentVolume, m_targetVolume, Time.deltaTime * 5f);

            if (!m_radioSource.isPlaying && m_stations.Length > 0 && m_currentStationIndex < m_stations.Length)
            {
                PlayCurrentStation();
            }
        }

        private void UpdateUI()
        {
            if (m_stations == null || m_currentStationIndex >= m_stations.Length) return;

            RadioStation station = m_stations[m_currentStationIndex];

            if (m_stationNameText != null)
            {
                m_stationNameText.text = m_isOn ? station.name : "Radio Off";
            }

            if (m_frequencyText != null)
            {
                m_frequencyText.text = m_isOn ? $"{station.frequency} MHz" : "--";
            }
        }

        public void ToggleRadio()
        {
            m_isOn = !m_isOn;
            m_targetVolume = m_isOn ? m_defaultVolume : 0f;

            if (m_isOn)
            {
                PlayCurrentStation();
            }
            else
            {
                m_radioSource.Stop();
            }

            Debug.Log($"[Radio] Radio turned {(m_isOn ? "ON" : "OFF")}");
        }

        public void NextStation()
        {
            if (m_stations == null || m_stations.Length == 0) return;

            m_currentStationIndex = (m_currentStationIndex + 1) % m_stations.Length;

            if (m_isOn)
            {
                PlayCurrentStation();
            }

            Debug.Log($"[Radio] Switched to {m_stations[m_currentStationIndex].name}");
        }

        public void PreviousStation()
        {
            if (m_stations == null || m_stations.Length == 0) return;

            m_currentStationIndex = (m_currentStationIndex - 1 + m_stations.Length) % m_stations.Length;

            if (m_isOn)
            {
                PlayCurrentStation();
            }
        }

        private void PlayCurrentStation()
        {
            if (m_stations == null || m_currentStationIndex >= m_stations.Length) return;

            RadioStation station = m_stations[m_currentStationIndex];

            // In a real implementation, you would load actual audio clips
            // For now, we generate a simple tone
            if (station.songs == null || station.songs.Length == 0)
            {
                // Radio would play music here
                Debug.Log($"[Radio] Playing: {station.name}");
            }
            else
            {
                m_radioSource.clip = station.songs[0];
                m_radioSource.Play();
            }
        }

        public void SetVolume(float volume)
        {
            m_defaultVolume = Mathf.Clamp01(volume);
            if (m_isOn)
            {
                m_targetVolume = m_defaultVolume;
            }
        }

        public void SetStation(int index)
        {
            if (index >= 0 && index < m_stations.Length)
            {
                m_currentStationIndex = index;
                if (m_isOn)
                {
                    PlayCurrentStation();
                }
            }
        }

        public bool IsOn => m_isOn;
        public int CurrentStationIndex => m_currentStationIndex;
        public string CurrentStationName => (m_stations != null && m_currentStationIndex < m_stations.Length) ? m_stations[m_currentStationIndex].name : "";
    }
}
