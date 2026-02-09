using UnityEngine;
using System;

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

    public void SaveQuizAttempt(int correctAnswers, int totalQuestions)
    {
        var profile = ProfileManager.Instance?.currentProfile;

        if (profile == null)
        {
            Debug.LogWarning("No active profile. Quiz attempt not saved.");
            return;
        }

        PlayerProfile.QuizAttemptData attempt =
            new PlayerProfile.QuizAttemptData(totalQuestions, correctAnswers);

        profile.quizAttempts.Add(attempt);

        profile.totalQuizCorrect += correctAnswers;
        profile.totalQuizQuestions += totalQuestions;

        ProfileManager.Instance.SaveProfiles();

        Debug.Log($"Saved attempt for {profile.playerName}");
    }

}
