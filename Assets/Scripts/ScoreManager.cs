using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveQuizStats(int correctAnswers, int totalQuestions)
    {
        if (ProfileManager.Instance == null || ProfileManager.Instance.currentProfile == null)
        {
            Debug.LogWarning("No active profile. Quiz stats not saved.");
            return;
        }

        PlayerProfile profile = ProfileManager.Instance.currentProfile;

        profile.totalQuizCorrect += correctAnswers;
        profile.totalQuizQuestions += totalQuestions;

        ProfileManager.Instance.SaveProfiles();

        Debug.Log("Quiz stats saved to profile: " + profile.playerName);
    }
}
