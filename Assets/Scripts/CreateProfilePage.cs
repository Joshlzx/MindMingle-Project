using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CreateProfilePage : MonoBehaviour
{
    public TMP_InputField nameInput;
    public Button saveButton;
    public Button backButton;

    public static int lastPage = 0;         
    public static int profilesPerPage = 4;   


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

        
        ProfileManager.Instance.CreateProfile(playerName);
        ProfileManager.Instance.SaveProfiles();

        
        int newProfileIndex = ProfileManager.Instance.profiles.Count - 1; // last profile
        ProfileSelectManager.lastPage = newProfileIndex / ProfileSelectManager.profilesPerPage;

        
        SceneManager.LoadScene("ProfileSelectionScene");
    }
    public void GoBackToProfileSelect()
    {
        
        SceneManager.LoadScene("ProfileSelectionScene");

    }

}
