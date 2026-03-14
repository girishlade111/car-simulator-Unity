using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace CarSimulator.SaveSystem
{
{
    public static SaveManager Instance { get; private set; }

    private const string SAVE_FOLDER = "Saves";
    private const string SETTINGS_FILE = "settings.json";
    private const int MAX_SAVES = 10;

    private string m_savePath;

    public string SavePath => m_savePath;

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

    private void EnsureSaveDirectory()
    {
        if (!Directory.Exists(m_savePath))
        {
            Directory.CreateDirectory(m_savePath);
        }
    }

    public async Task<bool> SaveGameAsync(GameSaveData saveData, int slotIndex = 0)
    {
        try
        {
            string fileName = GetSaveFileName(slotIndex);
            string filePath = Path.Combine(m_savePath, fileName);

            saveData.lastPlayedAt = DateTime.Now;
            string json = JsonUtility.ToJson(saveData, true);

            await Task.Run(() => File.WriteAllText(filePath, json));

            Debug.Log($"[SaveManager] Saved game to slot {slotIndex}: {filePath}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] Failed to save game: {e.Message}");
            return false;
        }
    }

    public bool SaveGame(GameSaveData saveData, int slotIndex = 0)
    {
        try
        {
            string fileName = GetSaveFileName(slotIndex);
            string filePath = Path.Combine(m_savePath, fileName);

            saveData.lastPlayedAt = DateTime.Now;
            string json = JsonUtility.ToJson(saveData, true);

            File.WriteAllText(filePath, json);

            Debug.Log($"[SaveManager] Saved game to slot {slotIndex}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] Failed to save game: {e.Message}");
            return false;
        }
    }

    public async Task<GameSaveData> LoadGameAsync(int slotIndex = 0)
    {
        try
        {
            string filePath = GetSaveFilePath(slotIndex);

            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"[SaveManager] Save file not found: {filePath}");
                return null;
            }

            string json = await Task.Run(() => File.ReadAllText(filePath));
            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

            Debug.Log($"[SaveManager] Loaded game from slot {slotIndex}");
            return saveData;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] Failed to load game: {e.Message}");
            return null;
        }
    }

    public GameSaveData LoadGame(int slotIndex = 0)
    {
        try
        {
            string filePath = GetSaveFilePath(slotIndex);

            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"[SaveManager] Save file not found: {filePath}");
                return null;
            }

            string json = File.ReadAllText(filePath);
            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

            Debug.Log($"[SaveManager] Loaded game from slot {slotIndex}");
            return saveData;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] Failed to load game: {e.Message}");
            return null;
        }
    }

    public bool DeleteSave(int slotIndex)
    {
        try
        {
            string filePath = GetSaveFilePath(slotIndex);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log($"[SaveManager] Deleted save slot {slotIndex}");
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

    public bool HasSave(int slotIndex)
    {
        string filePath = GetSaveFilePath(slotIndex);
        return File.Exists(filePath);
    }

    public SaveSlotInfo[] GetAllSaveSlots()
    {
        SaveSlotInfo[] slots = new SaveSlotInfo[MAX_SAVES];

        for (int i = 0; i < MAX_SAVES; i++)
        {
            slots[i] = new SaveSlotInfo { slotIndex = i, exists = HasSave(i) };

            if (slots[i].exists)
            {
                GameSaveData data = LoadGame(i);
                if (data != null)
                {
                    slots[i].saveName = data.saveName;
                    slots[i].playTime = data.playTime;
                    slots[i].lastPlayed = data.lastPlayedAt;
                    slots[i].scene = data.currentScene;
                }
            }
        }

        return slots;
    }

    private string GetSaveFileName(int slotIndex)
    {
        return $"save_{slotIndex}.json";
    }

    private string GetSaveFilePath(int slotIndex)
    {
        return Path.Combine(m_savePath, GetSaveFileName(slotIndex));
    }

    public GameSaveData CreateNewSave(string saveName, int slotIndex = 0)
    {
        GameSaveData newSave = new GameSaveData
        {
            saveName = string.IsNullOrEmpty(saveName) ? $"Save {slotIndex + 1}" : saveName
        };

        SaveGame(newSave, slotIndex);
        return newSave;
    }
}

[System.Serializable]
public class SaveSlotInfo
{
    public int slotIndex;
    public bool exists;
    public string saveName;
    public float playTime;
    public DateTime lastPlayed;
    public string scene;
}
}
