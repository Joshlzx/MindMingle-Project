using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ProfileSelectManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform profileSlotsContainer;
    public GameObject profileSlotPrefab;
    public TMP_Text pageNumberText; 

    public Button prevButton;
    public Button nextButton;

    [Header("Delete Confirmation")]
    public GameObject deleteConfirmPanel;
    public Button deleteYesButton;
    public Button deleteNoButton;

    [Header("Settings")]
    public static int profilesPerPage = 4;

    private int currentPage = 0;
    private List<PlayerProfile> profiles;
    private PlayerProfile profileToDelete;

    public static int lastPage = 0;          // stores the page number when returning
    

    private void Start()
    {
        profiles = ProfileManager.Instance.profiles;

        // Jump to last page if returning from new profile
        currentPage = lastPage;

        prevButton.onClick.AddListener(() => ChangePage(-1));
        nextButton.onClick.AddListener(() => ChangePage(1));

        RefreshPage();
    }

    private void RefreshPage()
    {
        // Clear old buttons
        foreach (Transform child in profileSlotsContainer)
            Destroy(child.gameObject);

        int startIndex = currentPage * profilesPerPage;
        int profilesOnThisPage = 0; // count actual profiles

        // Count number of actual profiles on this page
        for (int i = 0; i < profilesPerPage; i++)
        {
            int profileIndex = startIndex + i;
            if (profileIndex < profiles.Count)
                profilesOnThisPage++;
        }

        bool createNewAdded = false;

        // Instantiate UI slots
        for (int i = 0; i < profilesPerPage; i++)
        {
            int profileIndex = startIndex + i;
            PlayerProfile profile = null;

            if (profileIndex < profiles.Count)
            {
                profile = profiles[profileIndex]; // existing profile
            }
            else if (!createNewAdded)
            {
                profile = null; // first empty slot → Create New Profile
                createNewAdded = true;
            }
            // else: blank slot

            GameObject slotGO = Instantiate(profileSlotPrefab, profileSlotsContainer);
            ProfileSlotUI slotUI = slotGO.GetComponent<ProfileSlotUI>();
            slotUI.Init(profile, this);

            // Blank slots beyond Create New
            if (profile == null && !createNewAdded)
            {
                slotUI.slotText.text = "";
                slotUI.deleteButton.gameObject.SetActive(false);
                slotUI.button.onClick.RemoveAllListeners();
            }
        }

        // Enable prev button if currentPage > 0
        prevButton.interactable = currentPage > 0;

 

        // Calculate total pages including an extra page if needed for "Create New Profile"
        int totalProfiles = profiles.Count;
        int totalPages = Mathf.CeilToInt((float)(totalProfiles + 1) / profilesPerPage); // +1 for "Create New"

        // Always at least 1 page
        if (totalPages == 0) totalPages = 1;

        // Enable next if there are more pages (including empty slots)
        nextButton.interactable = (currentPage + 1) < totalPages;

        // Display current page (1-based)
        if (pageNumberText != null)
            pageNumberText.text = $"Page {currentPage + 1} / {totalPages}";
    }






    private void ChangePage(int delta)
    {
        currentPage += delta;
        RefreshPage();
    }

    public void OnSlotClicked(ProfileSlotUI slotUI)
    {
        if (slotUI.profile == null)
        {

            // Before loading CreateProfileScene, store the current page
            lastPage = currentPage;

            // Go to Create Profile scene
            SceneManager.LoadScene("CreateProfileScene");
        }
        else
        {
            // Select profile and go to main menu
            ProfileManager.Instance.SelectProfile(slotUI.profile);
            SceneManager.LoadScene("MainMenu");
        }
    }

    #region Delete Profile
    public void ConfirmDelete(PlayerProfile profile)
    {
        profileToDelete = profile;
        deleteConfirmPanel.SetActive(true);

        deleteYesButton.onClick.RemoveAllListeners();
        deleteYesButton.onClick.AddListener(DeleteConfirmed);

        deleteNoButton.onClick.RemoveAllListeners();
        deleteNoButton.onClick.AddListener(DeleteCancelled);
    }

    private void DeleteConfirmed()
    {
        if (profileToDelete != null)
        {
            ProfileManager.Instance.DeleteProfile(profileToDelete);
            ProfileManager.Instance.SaveProfiles();
            profileToDelete = null;
        }

        deleteConfirmPanel.SetActive(false);
        RefreshPage();
    }

    private void DeleteCancelled()
    {
        profileToDelete = null;
        deleteConfirmPanel.SetActive(false);
    }
    #endregion
}
