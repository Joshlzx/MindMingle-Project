using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static PlayerProfile;

public class PathTrailResultScreen : MonoBehaviour
{
    [Header("UI References")]
    public GameObject resultPanel;      // The panel to show results
    public TextMeshProUGUI resultText;  // Text to show time/errors/date
    public Button playAgainButton;      // Button to replay
    public Button mainMenuButton;       // Button to go back

    private float lastCompletionTime;
    private int lastErrors;

    private void Start()
    {
        // Hook up button clicks
        playAgainButton.onClick.AddListener(PlayAgain);
        mainMenuButton.onClick.AddListener(BackToMainMenu);

        // Hide panel initially
        resultPanel.SetActive(false);
    }

    /// <summary>
    /// Call this to show the result screen after the game ends
    /// </summary>
    public void ShowResult(float completionTime, int totalErrors)
    {
        lastCompletionTime = completionTime;
        lastErrors = totalErrors;

        // Save attempt to player profile
        SaveAttempt(completionTime, totalErrors);

        // Update UI text
        string dateTime = System.DateTime.Now.ToString("dd MMM yyyy HH:mm");
        resultText.text =
            $"<b>Path Trail Result</b>\n\n" +
            $"<color=#9C27B0>Time: {completionTime:F1} s</color>\n" +
            $"<color=#FF8C00>Errors: {totalErrors}</color>\n" +
            $"<color=#666666>Date: {dateTime}</color>";

        // Show panel
        resultPanel.SetActive(true);
    }

    void SaveAttempt(float completionTime, int totalErrors)
    {
        var profile = ProfileManager.Instance?.currentProfile;
        if (profile == null)
        {
            Debug.LogWarning("No active profile, cannot save PathTrail attempt.");
            return;
        }

        PlayerProfile.PathTrailAttemptData attempt = new PlayerProfile.PathTrailAttemptData(
            completionTime,
            totalErrors,
            profile.playerName
        );

        profile.pathTrailAttempts.Add(attempt);
        ProfileManager.Instance.SaveProfiles();
    }

    void PlayAgain()
    {
        // Reload current PathTrail game scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void BackToMainMenu()
    {
        // Change "MainMenu" to your actual main menu scene name
        SceneManager.LoadScene("MainMenu");
    }
}