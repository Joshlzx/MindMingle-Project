using UnityEngine;
using System.Collections.Generic;

public class ProfileManager : MonoBehaviour
{
    public static ProfileManager Instance { get; private set; }

    public List<PlayerProfile> profiles = new List<PlayerProfile>();
    public PlayerProfile currentProfile;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadProfiles(); // Load profiles when game starts
    }

    public void CreateProfile(string playerName)
    {
        PlayerProfile newProfile = new PlayerProfile(playerName);
        profiles.Add(newProfile);
        currentProfile = newProfile;
        SaveProfiles(); // save automatically when created
    }

    public void DeleteProfile(PlayerProfile profile)
    {
        if (profiles.Contains(profile))
        {
            profiles.Remove(profile);
            if (currentProfile == profile)
                currentProfile = null;
            SaveProfiles(); // save automatically when deleted
        }
    }

    public void SelectProfile(PlayerProfile profile)
    {
        currentProfile = profile;
        // optional: save selected profile
    }

    // --- Saving & Loading ---
    public void SaveProfiles()
    {
        string json = JsonUtility.ToJson(new PlayerProfileListWrapper(profiles));
        PlayerPrefs.SetString("profiles", json);
        PlayerPrefs.Save();
    }

    public void LoadProfiles()
    {
        if (PlayerPrefs.HasKey("profiles"))
        {
            string json = PlayerPrefs.GetString("profiles");
            PlayerProfileListWrapper wrapper = JsonUtility.FromJson<PlayerProfileListWrapper>(json);
            profiles = wrapper.profiles ?? new List<PlayerProfile>();
        }
    }

    // JSON wrapper because JsonUtility can't serialize List<T> directly
    [System.Serializable]
    private class PlayerProfileListWrapper
    {
        public List<PlayerProfile> profiles;
        public PlayerProfileListWrapper(List<PlayerProfile> profiles)
        {
            this.profiles = profiles;
        }
    }
}
