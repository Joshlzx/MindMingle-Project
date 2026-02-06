using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CreateProfilePage : MonoBehaviour
{
    public TMP_InputField nameInput;
    public Button saveButton;
    public Button backButton;

    public static int lastPage = 0;          // stores the page number when returning
    public static int profilesPerPage = 4;   // make static so we can access from CreateProfilePage


    void Start()
    {
        // Add listener to button
        //saveButton.onClick.AddListener(SaveProfile);
    }

    public void SaveProfile()
    {
        string playerName = nameInput.text;
        if (string.IsNullOrEmpty(playerName)) return;

        if (ProfileManager.Instance == null)
        {
            Debug.LogError("ProfileManager instance is null! Did you add it to the scene?");
            return;
        }

        // 1️⃣ Create the new profile
        ProfileManager.Instance.CreateProfile(playerName);
        ProfileManager.Instance.SaveProfiles();

        // 2️⃣ Calculate which page this new profile should appear on
        int newProfileIndex = ProfileManager.Instance.profiles.Count - 1; // last profile
        ProfileSelectManager.lastPage = newProfileIndex / ProfileSelectManager.profilesPerPage;

        // 3️⃣ Go back to ProfileSelectScene
        SceneManager.LoadScene("ProfileSelectionScene");
    }
    public void GoBackToProfileSelect()
    {
        // Simply load the profile selection scene
        SceneManager.LoadScene("ProfileSelectionScene");

    }

}
