using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static PlayerProfile;

public class PathTrailAttemptHistoryManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform attemptsContent; // Container for current player's attempts
    public GameObject attemptEntryPrefab;

    public Transform highscoresContent; // Container for all players' best attempts
    public GameObject highscoreEntryPrefab;

    private void Start()
    {
        ShowCurrentPlayerAttempts();
        ShowAllPlayersHighscores();
    }

    void ShowCurrentPlayerAttempts()
    {
        var profile = ProfileManager.Instance?.currentProfile;
        if (profile == null || profile.pathTrailAttempts == null) return;

        // Clear existing entries
        foreach (Transform child in attemptsContent)
            Destroy(child.gameObject);

        // Sort by dateTime descending (latest first)
        profile.pathTrailAttempts.Sort((a, b) => System.DateTime.Parse(b.dateTime)
                                                    .CompareTo(System.DateTime.Parse(a.dateTime)));

        // Populate UI entries
        foreach (PathTrailAttemptData attempt in profile.pathTrailAttempts)
        {
            GameObject entry = Instantiate(attemptEntryPrefab, attemptsContent);
            entry.GetComponent<TextMeshProUGUI>().text =
                $"<b><color=#9C27B0>Time: {attempt.completionTime:F1}s</color></b>  |  " +
                $"<b><color=#FF8C00>Errors: {attempt.totalErrors}</color></b>  |  " +
                $"<b><color=#666666>{attempt.dateTime}</color></b>";
        }
    }

    void ShowAllPlayersHighscores()
    {
        foreach (Transform child in highscoresContent)
            Destroy(child.gameObject);

        if (ProfileManager.Instance == null) return;

        // Step 1: Collect best attempt per player (fewest errors → fastest time)
        List<(string playerName, PathTrailAttemptData bestAttempt)> bestAttempts = new List<(string, PathTrailAttemptData)>();

        foreach (var p in ProfileManager.Instance.profiles)
        {
            if (p.pathTrailAttempts == null || p.pathTrailAttempts.Count == 0)
                continue;

            PathTrailAttemptData best = p.pathTrailAttempts[0];

            foreach (var a in p.pathTrailAttempts)
            {
                if (IsBetterPathTrailAttempt(a, best))
                    best = a;
            }

            bestAttempts.Add((p.playerName, best));
        }

        // Step 2: Sort: fewest errors first → tie-breaker: fastest completion
        bestAttempts.Sort((x, y) => ComparePathTrailAttempts(x.bestAttempt, y.bestAttempt));

        // Step 3: Populate UI
        foreach (var entryData in bestAttempts)
        {
            GameObject entry = Instantiate(highscoreEntryPrefab, highscoresContent);

            entry.GetComponent<TextMeshProUGUI>().text =
                $"<b><color=#000000>{entryData.playerName}</color></b>  |  " +
                $"<b><color=#9C27B0>Time: {entryData.bestAttempt.completionTime:F1}s</color></b>  |  " +
                $"<b><color=#FF8C00>Errors: {entryData.bestAttempt.totalErrors}</color></b>  |  " +
                $"<b><color=#666666>{entryData.bestAttempt.dateTime}</color></b>";
        }
    }

    // Compare attempts: fewer errors = better, tie → faster time wins
    int ComparePathTrailAttempts(PathTrailAttemptData a, PathTrailAttemptData b)
    {
        int errorCompare = a.totalErrors.CompareTo(b.totalErrors); // fewer errors first
        if (errorCompare != 0) return errorCompare;

        return a.completionTime.CompareTo(b.completionTime); // faster time wins if errors equal
    }

    bool IsBetterPathTrailAttempt(PathTrailAttemptData a, PathTrailAttemptData b)
    {
        if (a.totalErrors < b.totalErrors) return true;
        if (a.totalErrors > b.totalErrors) return false;
        return a.completionTime < b.completionTime; // tie-breaker
    }
}