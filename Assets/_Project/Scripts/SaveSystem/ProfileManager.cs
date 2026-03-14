using System.Collections.Generic;
using UnityEngine;

public class ProfileManager : MonoBehaviour
{
    public static ProfileManager Instance { get; private set; }

    [SerializeField] private int m_currentProfileIndex = 0;
    [SerializeField] private List<PlayerProfile> m_profiles;

    private PlayerProfile m_activeProfile;

    public PlayerProfile ActiveProfile => m_activeProfile;
    public int CurrentProfileIndex => m_currentProfileIndex;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        m_profiles = new List<PlayerProfile>();
        LoadProfiles();
    }

    private void Start()
    {
        if (m_profiles.Count > 0)
        {
            SetActiveProfile(m_currentProfileIndex);
        }
    }

    public void LoadProfiles()
    {
        m_profiles.Clear();

        for (int i = 0; i < 3; i++)
        {
            PlayerProfile profile = new PlayerProfile { profileId = i };
            m_profiles.Add(profile);
        }

        if (m_profiles.Count > 0)
        {
            m_activeProfile = m_profiles[0];
        }
    }

    public void SetActiveProfile(int index)
    {
        if (index < 0 || index >= m_profiles.Count)
        {
            Debug.LogWarning($"[ProfileManager] Invalid profile index: {index}");
            return;
        }

        m_currentProfileIndex = index;
        m_activeProfile = m_profiles[index];
        Debug.Log($"[ProfileManager] Active profile set to: {m_activeProfile.ProfileName}");
    }

    public void UpdateProfileName(string name)
    {
        if (m_activeProfile != null)
        {
            m_activeProfile.ProfileName = name;
        }
    }

    public void UpdatePlayerProgress(PlayerData playerData)
    {
        if (m_activeProfile != null)
        {
            m_activeProfile.PlayerData = playerData;
        }
    }

    public void AddCurrency(int amount)
    {
        if (m_activeProfile != null)
        {
            m_activeProfile.Currency += amount;
        }
    }

    public bool SpendCurrency(int amount)
    {
        if (m_activeProfile != null && m_activeProfile.Currency >= amount)
        {
            m_activeProfile.Currency -= amount;
            return true;
        }
        return false;
    }

    public void UnlockVehicle(string vehicleId)
    {
        if (m_activeProfile != null && !m_activeProfile.UnlockedVehicles.Contains(vehicleId))
        {
            m_activeProfile.UnlockedVehicles.Add(vehicleId);
        }
    }

    public bool HasVehicle(string vehicleId)
    {
        return m_activeProfile != null && m_activeProfile.UnlockedVehicles.Contains(vehicleId);
    }

    public void SaveCurrentProfile()
    {
        if (m_activeProfile == null) return;

        if (SaveManager.Instance != null)
        {
            GameSaveData saveData = new GameSaveData
            {
                saveName = m_activeProfile.ProfileName,
                player = m_activeProfile.PlayerData
            };

            SaveManager.Instance.SaveGame(saveData, m_currentProfileIndex);
        }
    }

    public void LoadCurrentProfile()
    {
        if (SaveManager.Instance != null)
        {
            GameSaveData saveData = SaveManager.Instance.LoadGame(m_currentProfileIndex);
            if (saveData != null && m_activeProfile != null)
            {
                m_activeProfile.PlayerData = saveData.player;
            }
        }
    }

    public PlayerProfile GetProfile(int index)
    {
        if (index >= 0 && index < m_profiles.Count)
        {
            return m_profiles[index];
        }
        return null;
    }

    public int GetProfileCount()
    {
        return m_profiles.Count;
    }
}

[System.Serializable]
public class PlayerProfile
{
    public int profileId;
    public string profileName = "New Player";
    public int currency = 0;
    public PlayerData PlayerData;
    public List<string> UnlockedVehicles;
    public List<string> UnlockedCustomizations;

    public PlayerProfile()
    {
        profileName = "Player " + (profileId + 1);
        currency = 0;
        PlayerData = new PlayerData();
        UnlockedVehicles = new List<string> { "default" };
        UnlockedCustomizations = new List<string>();
    }
}
