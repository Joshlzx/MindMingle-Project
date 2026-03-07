using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static PlayerProfile;

public class PathTrailAttemptHistoryManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform attemptsContent;      // Container for current player's attempts
    public GameObject attemptEntryPrefab;

    public Transform highscoresContent;    // Container for all players' best attempts
    public GameObject highscoreEntryPrefab;

    private void Start()
    {
        ShowCurrentPlayerAttempts();
        ShowAllPlayersHighscores();
    }

    // -------------------- Current Player Attempts --------------------
    void ShowCurrentPlayerAttempts()
    {
        var profile = ProfileManager.Instance?.currentProfile;

        foreach (Transform child in attemptsContent)
            Destroy(child.gameObject);

        if (profile == null || profile.pathTrailAttempts == null) return;

        // Sort: valid grades first by grade → errors → time, null/empty grades at the bottom
        profile.pathTrailAttempts.Sort((a, b) =>
        {
            bool aNull = string.IsNullOrEmpty(a.grade);
            bool bNull = string.IsNullOrEmpty(b.grade);

            if (aNull && bNull) return 0;
            if (aNull) return 1;  // null goes below
            if (bNull) return -1;

            return ComparePathTrailAttemptsByGrade(a, b);
        });

        foreach (var attempt in profile.pathTrailAttempts)
        {
            GameObject entry = Instantiate(attemptEntryPrefab, attemptsContent);
            Color gradeColor = string.IsNullOrEmpty(attempt.grade) ? Color.gray : GetGradeColor(attempt.grade);

            entry.GetComponent<TextMeshProUGUI>().text =
                $"<b><color=#9C27B0>Time: {attempt.completionTime:F1}s</color></b>  |  " +
                $"<b><color=#FF8C00>Errors: {attempt.totalErrors}/{attempt.totalNodes}</color></b>  |  " +
                $"<b><color=#{ColorUtility.ToHtmlStringRGB(gradeColor)}>Grade: {attempt.grade}</color></b>  |  " +
                $"<b><color=#666666>{attempt.dateTime}</color></b>";
        }
    }

    // -------------------- All Players Highscores --------------------
    void ShowAllPlayersHighscores()
    {
        foreach (Transform child in highscoresContent)
            Destroy(child.gameObject);

        if (ProfileManager.Instance == null) return;

        var bestAttempts = new List<(string playerName, PathTrailAttemptData bestAttempt)>();

        foreach (var p in ProfileManager.Instance.profiles)
        {
            PathTrailAttemptData best = null;
            if (p.pathTrailAttempts != null && p.pathTrailAttempts.Count > 0)
            {
                best = p.pathTrailAttempts[0];
                foreach (var a in p.pathTrailAttempts)
                {
                    if (IsBetterPathTrailAttempt(a, best)) best = a;
                }
            }

            bestAttempts.Add((p.playerName, best));
        }

        // Sort: valid grades first by grade → errors → time, null/empty grades last
        bestAttempts.Sort((x, y) =>
        {
            bool xNull = x.bestAttempt == null || string.IsNullOrEmpty(x.bestAttempt.grade);
            bool yNull = y.bestAttempt == null || string.IsNullOrEmpty(y.bestAttempt.grade);

            if (xNull && yNull) return 0;
            if (xNull) return 1;
            if (yNull) return -1;

            return ComparePathTrailAttemptsByGrade(x.bestAttempt, y.bestAttempt);
        });

        foreach (var entryData in bestAttempts)
        {
            GameObject entry = Instantiate(highscoreEntryPrefab, highscoresContent);

            if (entryData.bestAttempt == null)
            {
                // no attempt data, just show player name
                entry.GetComponent<TextMeshProUGUI>().text =
                    $"<b><color=#000000>{entryData.playerName}</color></b>";
                continue;
            }

            Color gradeColor = string.IsNullOrEmpty(entryData.bestAttempt.grade) ? Color.gray : GetGradeColor(entryData.bestAttempt.grade);

            entry.GetComponent<TextMeshProUGUI>().text =
                $"<b><color=#000000>{entryData.playerName}</color></b>  |  " +
                $"<b><color=#9C27B0>Time: {entryData.bestAttempt.completionTime:F1}s</color></b>  |  " +
                $"<b><color=#FF8C00>Errors: {entryData.bestAttempt.totalErrors}/{entryData.bestAttempt.totalNodes}</color></b>  |  " +
                $"<b><color=#{ColorUtility.ToHtmlStringRGB(gradeColor)}>Grade: {entryData.bestAttempt.grade}</color></b>  |  " +
                $"<b><color=#666666>{entryData.bestAttempt.dateTime}</color></b>";
        }
    }

    // -------------------- Helper Methods --------------------
    Color GetGradeColor(string grade)
    {
        switch (grade)
        {
            case "Excellent": return new Color(0f, 0.6f, 0f);
            case "Good": return new Color(0f, 0.4f, 0.8f);
            case "Average": return new Color(1f, 0.65f, 0f);
            case "Poor": return new Color(0.8f, 0f, 0f);
            default: return Color.gray; // Null/invalid grade
        }
    }

    int ComparePathTrailAttemptsByGrade(PathTrailAttemptData a, PathTrailAttemptData b)
    {
        List<string> gradeOrder = new List<string> { "Excellent", "Good", "Average", "Poor" };

        int gradeCompare = gradeOrder.IndexOf(a.grade).CompareTo(gradeOrder.IndexOf(b.grade));
        if (gradeCompare != 0) return gradeCompare;

        int errorCompare = a.totalErrors.CompareTo(b.totalErrors);
        if (errorCompare != 0) return errorCompare;

        return a.completionTime.CompareTo(b.completionTime);
    }

    bool IsBetterPathTrailAttempt(PathTrailAttemptData a, PathTrailAttemptData b)
    {
        List<string> gradeOrder = new List<string> { "Excellent", "Good", "Average", "Poor" };

        int gradeCompare = gradeOrder.IndexOf(a.grade).CompareTo(gradeOrder.IndexOf(b.grade));
        if (gradeCompare != 0) return gradeCompare < 0;

        if (a.totalErrors != b.totalErrors) return a.totalErrors < b.totalErrors;

        return a.completionTime < b.completionTime;
    }
}