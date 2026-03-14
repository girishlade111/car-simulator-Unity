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
        [SerializeField] private KeyCode m_prevStationKey = KeyCode.N;
        [SerializeField] private KeyCode m_toggleKey = KeyCode.R;
        [SerializeField] private float m_defaultVolume = 0.5f;

        [Header("Stations")]
        [SerializeField] private List<RadioStation> m_stations = new List<RadioStation>();
        [SerializeField] private int m_currentStationIndex;
        [SerializeField] private int m_currentTrackIndex;

        [Header("Audio")]
        [SerializeField] private AudioSource m_radioSource;
        [SerializeField] private AudioSource m_engineSource;

        [Header("UI")]
        [SerializeField] private UnityEngine.UI.Text m_stationNameText;
        [SerializeField] private UnityEngine.UI.Text m_frequencyText;
        [SerializeField] private UnityEngine.UI.Text m_trackNameText;

        private bool m_isOn;
        private float m_targetVolume;

        [System.Serializable]
        public class RadioStation
        {
            public string name;
            public string genre;
            public float frequency;
            public Color stationColor;
            public List<AudioClip> playlist = new List<AudioClip>();
        }

        private void Start()
        {
            SetupAudioSource();
            InitializeDefaultStations();
            
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
            
            m_radioSource.loop = false;
            m_radioSource.playOnAwake = false;
            m_radioSource.spatialBlend = 1f;
            m_radioSource.minDistance = 1f;
            m_radioSource.maxDistance = 10f;
        }

        private void InitializeDefaultStations()
        {
            m_stations = new List<RadioStation>
            {
                CreateStation("RADIO ROCK", "Rock", 101.5f, Color.red),
                CreateStation("POP FM", "Pop", 104.3f, Color.magenta),
                CreateStation("JAZZ LOUNGE", "Jazz", 92.7f, new Color(0.8f, 0.5f, 0.2f)),
                CreateStation("ELECTRO BEATS", "Electronic", 106.9f, Color.cyan),
                CreateStation("CLASSIC HITS", "Classic Rock", 98.1f, Color.yellow),
                CreateStation("HIP HOP FM", "Hip Hop", 95.5f, new Color(1f, 0.5f, 0f)),
                CreateStation("COUNTRY ROADS", "Country", 107.9f, new Color(0.2f, 0.6f, 0.2f)),
                CreateStation("CHILL STATION", "Ambient", 89.3f, Color.blue)
            };
        }

        private RadioStation CreateStation(string name, string genre, float frequency, Color color)
        {
            RadioStation station = new RadioStation
            {
                name = name,
                genre = genre,
                frequency = frequency,
                stationColor = color
            };
            return station;
        }

        public void AddSongToStation(int stationIndex, AudioClip song)
        {
            if (stationIndex >= 0 && stationIndex < m_stations.Count)
            {
                m_stations[stationIndex].playlist.Add(song);
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

            if (Input.GetKeyDown(m_prevStationKey))
            {
                PreviousStation();
            }
        }

        private void UpdateRadioState()
        {
            if (!m_isOn || m_radioSource == null) return;

            float currentVolume = m_radioSource.volume;
            m_radioSource.volume = Mathf.Lerp(currentVolume, m_targetVolume, Time.deltaTime * 5f);

            if (!m_radioSource.isPlaying && m_stations.Count > 0 && m_currentStationIndex < m_stations.Count)
            {
                PlayCurrentTrack();
            }
        }

        private void UpdateUI()
        {
            if (m_stations == null || m_currentStationIndex >= m_stations.Count) return;

            RadioStation station = m_stations[m_currentStationIndex];

            if (m_stationNameText != null)
            {
                m_stationNameText.text = m_isOn ? station.name : "RADIO OFF";
            }

            if (m_frequencyText != null)
            {
                m_frequencyText.text = m_isOn ? $"{station.frequency} MHz" : "--";
            }

            if (m_trackNameText != null)
            {
                m_trackNameText.text = m_isOn ? $"Now Playing: {station.genre}" : "";
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
            if (m_stations == null || m_stations.Count == 0) return;

            m_currentStationIndex = (m_currentStationIndex + 1) % m_stations.Count;
            m_currentTrackIndex = 0;

            if (m_isOn)
            {
                PlayCurrentStation();
            }

            Debug.Log($"[Radio] Switched to {m_stations[m_currentStationIndex].name}");
        }

        public void PreviousStation()
        {
            if (m_stations == null || m_stations.Count == 0) return;

            m_currentStationIndex = (m_currentStationIndex - 1 + m_stations.Count) % m_stations.Count;
            m_currentTrackIndex = 0;

            if (m_isOn)
            {
                PlayCurrentStation();
            }
        }

        private void PlayCurrentStation()
        {
            if (m_stations == null || m_currentStationIndex >= m_stations.Count) return;

            RadioStation station = m_stations[m_currentStationIndex];

            if (station.playlist != null && station.playlist.Count > 0)
            {
                m_currentTrackIndex = m_currentTrackIndex % station.playlist.Count;
                m_radioSource.clip = station.playlist[m_currentTrackIndex];
                m_radioSource.Play();
            }
            else
            {
                Debug.Log($"[Radio] Playing: {station.name} ({station.genre})");
            }
        }

        private void PlayCurrentTrack()
        {
            if (m_stations == null || m_currentStationIndex >= m_stations.Count) return;

            RadioStation station = m_stations[m_currentStationIndex];

            if (station.playlist != null && station.playlist.Count > 0)
            {
                m_currentTrackIndex = (m_currentTrackIndex + 1) % station.playlist.Count;
                m_radioSource.clip = station.playlist[m_currentTrackIndex];
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
            if (index >= 0 && index < m_stations.Count)
            {
                m_currentStationIndex = index;
                m_currentTrackIndex = 0;
                if (m_isOn)
                {
                    PlayCurrentStation();
                }
            }
        }

        public void SkipTrack()
        {
            if (m_isOn)
            {
                m_radioSource.Stop();
                PlayCurrentTrack();
            }
        }

        public int GetStationCount() => m_stations != null ? m_stations.Count : 0;
        public RadioStation GetCurrentStation() => (m_stations != null && m_currentStationIndex < m_stations.Count) ? m_stations[m_currentStationIndex] : null;
        public bool IsOn => m_isOn;
    }
}
