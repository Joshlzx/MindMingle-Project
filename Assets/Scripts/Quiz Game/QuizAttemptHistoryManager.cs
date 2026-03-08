using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuizAttemptHistoryManager : MonoBehaviour
{
    public Transform attemptsContent;
    public GameObject attemptEntryPrefab;
    public Transform highscoresContent;
    public GameObject highscoreEntryPrefab;

    [Header("Theme Settings")]
    public TextMeshProUGUI themeNameText;
    public List<string> themeNames;
    public int currentThemeID = 1;

    private void Start() => UpdateUI();

    public void UpdateUI()
    {
        ShowCurrentPlayerAttempts(currentThemeID);
        ShowAllPlayersHighscores(currentThemeID);

        if (themeNameText != null && themeNames.Count > currentThemeID)
            themeNameText.text = themeNames[currentThemeID];
    }

    void ShowCurrentPlayerAttempts(int themeID)
    {
        var profile = ProfileManager.Instance?.currentProfile;
        if (profile == null) return;

        foreach (Transform child in attemptsContent)
            Destroy(child.gameObject);

        var themeAttempts = profile.quizAttempts.FindAll(a => a.themeID == themeID);
        themeAttempts.Sort((a, b) => DateTime.Parse(b.dateTime).CompareTo(DateTime.Parse(a.dateTime)));

        foreach (var attempt in themeAttempts)
        {
            var entry = Instantiate(attemptEntryPrefab, attemptsContent);
            entry.GetComponent<TextMeshProUGUI>().text =
                $"<b>{attempt.correctAnswers}/{attempt.totalQuestions}</b> | " +
                $"<b><color=#666666>{attempt.dateTime}</color></b>";
        }
    }

    void ShowAllPlayersHighscores(int themeID)
    {
        foreach (Transform child in highscoresContent)
            Destroy(child.gameObject);

        if (ProfileManager.Instance == null) return;

        var highscoreList = new List<(string, PlayerProfile.QuizAttemptData)>();
        foreach (var p in ProfileManager.Instance.profiles)
        {
            var attempts = p.quizAttempts.FindAll(a => a.themeID == themeID);
            if (attempts.Count == 0) continue;

            var best = attempts[0];
            foreach (var a in attempts) if (a.correctAnswers > best.correctAnswers) best = a;

            highscoreList.Add((p.playerName, best));
        }

        highscoreList.Sort((a, b) => b.Item2.correctAnswers.CompareTo(a.Item2.correctAnswers));

        foreach (var entryData in highscoreList)
        {
            var entry = Instantiate(highscoreEntryPrefab, highscoresContent);
            entry.GetComponent<TextMeshProUGUI>().text =
                $"<b><color=#000000>{entryData.Item1}</color></b> | " +
                $"<b>{entryData.Item2.correctAnswers}/{entryData.Item2.totalQuestions}</b> | " +
                $"<b><color=#666666>{entryData.Item2.dateTime}</color></b>";
        }
    }

    public void NextTheme()
    {
        currentThemeID = (currentThemeID + 1) % themeNames.Count;
        UpdateUI();
    }

    public void PreviousTheme()
    {
        currentThemeID = (currentThemeID - 1 + themeNames.Count) % themeNames.Count;
        UpdateUI();
    }
}