using UnityEngine;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CarSimulator.SaveSystem
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        private const string SAVE_FOLDER = "Saves";
        private const string SETTINGS_FILE = "settings.json";
        private const string QUICKSAVE_FILE = "quicksave.json";
        private const int MAX_SAVES = 10;

        [Header("Settings")]
        [SerializeField] private bool m_autoSave = true;
        [SerializeField] private float m_autoSaveInterval = 300f;
        [SerializeField] private bool m_saveOnQuit = true;

        [Header("Current Save")]
        [SerializeField] private GameSaveData m_currentSave;
        [SerializeField] private int m_currentSlot;

        [Header("Paths")]
        private string m_savePath;

        public string SavePath => m_savePath;
        public GameSaveData CurrentSave => m_currentSave;
        public bool HasSave => m_currentSave != null;

        public event System.Action<GameSaveData> OnSaveLoaded;
        public event System.Action<GameSaveData> OnGameSaved;
        public event System.Action OnSaveDeleted;

        private float m_autoSaveTimer;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            m_savePath = Path.Combine(Application.persistentDataPath, SAVE_FOLDER);
            EnsureSaveDirectory();
        }

        private void Start()
        {
            m_currentSave = new GameSaveData();
        }

        private void Update()
        {
            if (m_autoSave && m_currentSave != null)
            {
                m_autoSaveTimer += Time.deltaTime;
                if (m_autoSaveTimer >= m_autoSaveInterval)
                {
                    m_autoSaveTimer = 0;
                    QuickSave();
                }
            }
        }

        private void OnApplicationQuit()
        {
            if (m_saveOnQuit && m_currentSave != null)
            {
                SaveGame(m_currentSave, m_currentSlot);
            }
        }

        private void EnsureSaveDirectory()
        {
            if (!Directory.Exists(m_savePath))
            {
                Directory.CreateDirectory(m_savePath);
            }
        }

        public bool SaveGame(GameSaveData saveData, int slotIndex = 0)
        {
            try
            {
                saveData.lastPlayedAt = DateTime.Now;
                saveData.version = GameSaveData.CURRENT_VERSION;

                string fileName = GetSaveFileName(slotIndex);
                string filePath = Path.Combine(m_savePath, fileName);

                string json = JsonUtility.ToJson(saveData, true);
                File.WriteAllText(filePath, json);

                m_currentSave = saveData;
                m_currentSlot = slotIndex;

                Debug.Log($"[SaveManager] Saved game to slot {slotIndex}: {filePath}");
                OnGameSaved?.Invoke(saveData);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] Failed to save game: {e.Message}");
                return false;
            }
        }

        public async Task<bool> SaveGameAsync(GameSaveData saveData, int slotIndex = 0)
        {
            try
            {
                saveData.lastPlayedAt = DateTime.Now;
                saveData.version = GameSaveData.CURRENT_VERSION;

                string fileName = GetSaveFileName(slotIndex);
                string filePath = Path.Combine(m_savePath, fileName);

                string json = JsonUtility.ToJson(saveData, true);

                await Task.Run(() => File.WriteAllText(filePath, json));

                m_currentSave = saveData;
                m_currentSlot = slotIndex;

                Debug.Log($"[SaveManager] Saved game to slot {slotIndex} (async)");
                OnGameSaved?.Invoke(saveData);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] Failed to save game: {e.Message}");
                return false;
            }
        }

        public GameSaveData LoadGame(int slotIndex = 0)
        {
            try
            {
                string fileName = GetSaveFileName(slotIndex);
                string filePath = Path.Combine(m_savePath, fileName);

                if (!File.Exists(filePath))
                {
                    Debug.LogWarning($"[SaveManager] Save file not found: {filePath}");
                    return CreateNewSave(slotIndex);
                }

                string json = File.ReadAllText(filePath);
                GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

                if (saveData == null)
                {
                    Debug.LogWarning("[SaveManager] Failed to parse save data, creating new save");
                    return CreateNewSave(slotIndex);
                }

                if (!saveData.Validate())
                {
                    Debug.LogWarning("[SaveManager] Save validation failed, attempting migration");
                    saveData.MigrateToCurrentVersion();
                }

                saveData.MigrateToCurrentVersion();

                m_currentSave = saveData;
                m_currentSlot = slotIndex;

                Debug.Log($"[SaveManager] Loaded save from slot {slotIndex}");
                OnSaveLoaded?.Invoke(saveData);
                return saveData;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] Failed to load game: {e.Message}");
                return CreateNewSave(slotIndex);
            }
        }

        public async Task<GameSaveData> LoadGameAsync(int slotIndex = 0)
        {
            return await Task.Run(() => LoadGame(slotIndex));
        }

        public GameSaveData QuickSave()
        {
            if (m_currentSave == null)
            {
                m_currentSave = new GameSaveData();
            }

            m_currentSave.lastPlayedAt = DateTime.Now;
            return SaveQuickSave(m_currentSave) ? m_currentSave : null;
        }

        public GameSaveData QuickLoad()
        {
            return LoadQuickSave();
        }

        private bool SaveQuickSave(GameSaveData saveData)
        {
            try
            {
                string filePath = Path.Combine(m_savePath, QUICKSAVE_FILE);
                saveData.version = GameSaveData.CURRENT_VERSION;
                string json = JsonUtility.ToJson(saveData, true);
                File.WriteAllText(filePath, json);
                Debug.Log("[SaveManager] Quicksave created");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] Quicksave failed: {e.Message}");
                return false;
            }
        }

        private GameSaveData LoadQuickSave()
        {
            try
            {
                string filePath = Path.Combine(m_savePath, QUICKSAVE_FILE);
                if (!File.Exists(filePath))
                {
                    Debug.LogWarning("[SaveManager] No quicksave found");
                    return null;
                }

                string json = File.ReadAllText(filePath);
                GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);
                saveData.MigrateToCurrentVersion();

                Debug.Log("[SaveManager] Quicksave loaded");
                OnSaveLoaded?.Invoke(saveData);
                return saveData;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] Failed to load quicksave: {e.Message}");
                return null;
            }
        }

        public bool DeleteSave(int slotIndex)
        {
            try
            {
                string fileName = GetSaveFileName(slotIndex);
                string filePath = Path.Combine(m_savePath, fileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    Debug.Log($"[SaveManager] Deleted save slot {slotIndex}");
                    OnSaveDeleted?.Invoke();
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] Failed to delete save: {e.Message}");
                return false;
            }
        }

        public SaveSlotInfo[] GetAllSaveSlots()
        {
            SaveSlotInfo[] slots = new SaveSlotInfo[MAX_SAVES];

            for (int i = 0; i < MAX_SAVES; i++)
            {
                slots[i] = GetSaveSlotInfo(i);
            }

            return slots;
        }

        public SaveSlotInfo GetSaveSlotInfo(int slotIndex)
        {
            SaveSlotInfo info = new SaveSlotInfo
            {
                slotIndex = slotIndex,
                exists = false
            };

            try
            {
                string fileName = GetSaveFileName(slotIndex);
                string filePath = Path.Combine(m_savePath, fileName);

                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

                    if (saveData != null)
                    {
                        info.exists = true;
                        info.saveName = saveData.saveName;
                        info.lastPlayedAt = saveData.lastPlayedAt;
                        info.playTime = saveData.playTime;
                        info.scene = saveData.currentScene;
                        info.currency = saveData.player?.currency ?? 0;
                        info.missionsCompleted = saveData.missions?.totalMissionsCompleted ?? 0;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] Error reading save slot {slotIndex}: {e.Message}");
            }

            return info;
        }

        private GameSaveData CreateNewSave(int slotIndex)
        {
            GameSaveData newSave = new GameSaveData
            {
                saveName = $"Save {slotIndex + 1}",
                currentScene = "OpenWorld_TestDistrict"
            };

            m_currentSave = newSave;
            return newSave;
        }

        private string GetSaveFileName(int slotIndex)
        {
            return $"save_{slotIndex}.json";
        }

        public void UpdatePlayerPosition(Vector3 position, Quaternion rotation)
        {
            if (m_currentSave?.player != null)
            {
                m_currentSave.player.lastPosition = position;
                m_currentSave.player.lastRotation = rotation;
            }
        }

        public void UpdateSpawnPoint(Vector3 position, Quaternion rotation)
        {
            if (m_currentSave?.player != null)
            {
                m_currentSave.player.SetSpawnPoint(position, rotation);
            }
        }

        public void UpdateCheckpoint(string checkpointId)
        {
            if (m_currentSave?.player != null)
            {
                m_currentSave.player.lastCheckpointId = checkpointId;
            }
        }

        public void AddCurrency(int amount)
        {
            if (m_currentSave?.player != null)
            {
                m_currentSave.player.currency += amount;
            }
        }

        public void SpendCurrency(int amount)
        {
            if (m_currentSave?.player != null && m_currentSave.player.currency >= amount)
            {
                m_currentSave.player.currency -= amount;
            }
        }

        public int GetCurrency()
        {
            return m_currentSave?.player?.currency ?? 0;
        }

        public void CompleteMission(string missionId)
        {
            if (m_currentSave?.missions != null)
            {
                m_currentSave.missions.CompleteMission(missionId);
            }
        }

        public bool IsMissionCompleted(string missionId)
        {
            return m_currentSave?.missions?.IsMissionCompleted(missionId) ?? false;
        }

        public void SetCurrentScene(string sceneName)
        {
            if (m_currentSave != null)
            {
                m_currentSave.currentScene = sceneName;
            }
        }

        public void AddPlayTime(float deltaTime)
        {
            if (m_currentSave != null)
            {
                m_currentSave.playTime += deltaTime;
            }
        }

        public SaveSlotInfo GetCurrentSlotInfo()
        {
            return GetSaveSlotInfo(m_currentSlot);
        }
    }

    [System.Serializable]
    public struct SaveSlotInfo
    {
        public int slotIndex;
        public bool exists;
        public string saveName;
        public DateTime lastPlayedAt;
        public float playTime;
        public string scene;
        public int currency;
        public int missionsCompleted;

        public string FormattedPlayTime()
        {
            TimeSpan time = TimeSpan.FromSeconds(playTime);
            return $"{(int)time.TotalHours}:{time.Minutes:D2}:{time.Seconds:D2}";
        }
    }
}
