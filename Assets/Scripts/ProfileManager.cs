using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ProfileManager : MonoBehaviour
{
    public static ProfileManager Instance { get; private set; }

    public List<PlayerProfile> profiles = new List<PlayerProfile>();
    public PlayerProfile currentProfile;

    private const string ProfilesKey = "profiles";
    private const string CurrentProfileKey = "currentProfile";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadProfiles();
        LoadCurrentProfile();
    }

    #region Profile Operations
    public void CreateProfile(string playerName)
    {
        PlayerProfile newProfile = new PlayerProfile(playerName);
        profiles.Add(newProfile);
        currentProfile = newProfile;
        SaveProfiles();
        SaveCurrentProfile();
    }

    public void DeleteProfile(PlayerProfile profile)
    {
        if (profiles.Contains(profile))
        {
            profiles.Remove(profile);
            if (currentProfile == profile)
                currentProfile = null;

            SaveProfiles();
            SaveCurrentProfile();
        }
    }

    public void SelectProfile(PlayerProfile profile)
    {
        currentProfile = profile;
        SaveCurrentProfile();
    }
    #endregion

    #region Saving & Loading
    public void SaveProfiles()
    {
        // Wrap list into a serializable wrapper
        PlayerProfileListWrapper wrapper = new PlayerProfileListWrapper(profiles);
        string json = JsonUtility.ToJson(wrapper, true); // prettyPrint for easier debugging
        PlayerPrefs.SetString(ProfilesKey, json);
        PlayerPrefs.Save();

        Debug.Log("Profiles saved: " + json);
    }

    public void LoadProfiles()
    {
        if (PlayerPrefs.HasKey(ProfilesKey))
        {
            string json = PlayerPrefs.GetString(ProfilesKey);
            PlayerProfileListWrapper wrapper = JsonUtility.FromJson<PlayerProfileListWrapper>(json);
            profiles = wrapper?.profiles ?? new List<PlayerProfile>();
            Debug.Log($"Loaded {profiles.Count} profiles from PlayerPrefs.");
        }
        else
        {
            profiles = new List<PlayerProfile>();
        }
    }

    public void SaveCurrentProfile()
    {
        if (currentProfile != null)
        {
            string json = JsonUtility.ToJson(currentProfile);
            PlayerPrefs.SetString(CurrentProfileKey, json);
            PlayerPrefs.Save();
        }
    }

    public void LoadCurrentProfile()
    {
        if (PlayerPrefs.HasKey(CurrentProfileKey))
        {
            string json = PlayerPrefs.GetString(CurrentProfileKey);
            currentProfile = JsonUtility.FromJson<PlayerProfile>(json);
            Debug.Log("Loaded current profile: " + currentProfile.playerName);
        }
    }
    #endregion

    #region JSON Wrapper
    [System.Serializable]
    private class PlayerProfileListWrapper
    {
        public List<PlayerProfile> profiles;
        public PlayerProfileListWrapper(List<PlayerProfile> profiles)
        {
            this.profiles = profiles;
        }
    }
    #endregion
}
