using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ProfileManager : MonoBehaviour
{
    public static ProfileManager Instance;

    public List<PlayerProfile> profiles = new List<PlayerProfile>();
    public PlayerProfile currentProfile;

    string savePath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            savePath = Application.persistentDataPath + "/profiles.json";
            LoadProfiles();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CreateProfile(string playerName)
    {
        PlayerProfile newProfile = new PlayerProfile(playerName);
        profiles.Add(newProfile);
        SaveProfiles();
    }

    public void SelectProfile(PlayerProfile profile)
    {
        currentProfile = profile;
    }

    public void SaveProfiles()
    {
        string json = JsonUtility.ToJson(new ProfileListWrapper(profiles), true);
        File.WriteAllText(savePath, json);
    }

    public void LoadProfiles()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            profiles = JsonUtility.FromJson<ProfileListWrapper>(json).profiles;
        }
    }
}

[System.Serializable]
public class ProfileListWrapper
{
    public List<PlayerProfile> profiles;

    public ProfileListWrapper(List<PlayerProfile> profiles)
    {
        this.profiles = profiles;
    }
}
