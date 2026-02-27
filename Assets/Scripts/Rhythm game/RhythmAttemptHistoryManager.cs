using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class RhythmAttemptHistoryManager : MonoBehaviour
{
    public Transform attemptsContent;
    public GameObject attemptEntryPrefab;

    public Transform highscoresContent;
    public GameObject highscoreEntryPrefab;

    private void Start()
    {
        ShowCurrentPlayerAttempts();
        ShowAllPlayersHighscores();
    }

    void ShowCurrentPlayerAttempts()
    {
        var profile = ProfileManager.Instance?.currentProfile;
        if (profile == null || profile.rhythmAttempts == null) return;

        // Clear existing entries
        foreach (Transform child in attemptsContent)
            Destroy(child.gameObject);

        // Sort attempts by latest first
        List<PlayerProfile.RhythmAttemptData> attemptsSorted = new List<PlayerProfile.RhythmAttemptData>(profile.rhythmAttempts);
        attemptsSorted.Sort((a, b) => b.dateTime.CompareTo(a.dateTime));

        foreach (var attempt in attemptsSorted)
        {
            GameObject entry = Instantiate(attemptEntryPrefab, attemptsContent);

            entry.GetComponent<TextMeshProUGUI>().text =
                $"<b><color=#CC00FF>Score: {attempt.finalScore}</color></b> | " +         // Purple + Bold
                $"<b><color=#FFD700>Rank: {attempt.rank}</color></b> | " +                // Gold + Bold
                $"<b><color=#00FFFF>Hit: {attempt.percentHit:F1}%</color></b> | " +       // Cyan + Bold
                $"<b><color=#FFFFFF>Normal: {attempt.normalHits}</color></b> | " +        // White + Bold
                $"<b><color=#4DA6FF>Good: {attempt.goodHits}</color></b> | " +            // Softer Blue + Bold
                $"<b><color=#00FF00>Perfect: {attempt.perfectHits}</color></b> | " +      // Green + Bold
                $"<b><color=#FF0000>Miss: {attempt.missHits}</color></b> | " +            // Red + Bold
                $"<b><color=#666666>{attempt.dateTime}</color></b>";                      // Gray + Bold
        }
    }

    void ShowAllPlayersHighscores()
    {
        // Clear previous entries
        foreach (Transform child in highscoresContent)
            Destroy(child.gameObject);

        if (ProfileManager.Instance == null) return;

        List<PlayerProfile.RhythmAttemptData> bestAttempts = new List<PlayerProfile.RhythmAttemptData>();

        // Collect only the best attempt of each player
        foreach (var p in ProfileManager.Instance.profiles)
        {
            if (p.rhythmAttempts == null || p.rhythmAttempts.Count == 0)
                continue;

            // Find the best attempt for this player by percentHit
            PlayerProfile.RhythmAttemptData best = p.rhythmAttempts[0];

            foreach (var a in p.rhythmAttempts)
            {
                if (a.percentHit > best.percentHit)
                    best = a;
            }

            bestAttempts.Add(best);
        }

        
        bestAttempts.Sort((a, b) => b.percentHit.CompareTo(a.percentHit));

        // Display
        foreach (var attempt in bestAttempts)
        {
            GameObject entry = Instantiate(highscoreEntryPrefab, highscoresContent);

            entry.GetComponent<TextMeshProUGUI>().text =
                $"<b><color=#000000>{attempt.playerName}</color></b> | " +                       // Black + Bold
                $"<b><color=#CC00FF>Score: {attempt.finalScore}</color></b> | " +                // Purple + Bold
                $"<b><color=#FFD700>Rank: {attempt.rank}</color></b> | " +                       // Gold + Bold
                $"<b><color=#00FFFF>Hit: {attempt.percentHit:F1}%</color></b> | " +              // Cyan + Bold
                $"<b><color=#FFFFFF>Normal: {attempt.normalHits}</color></b> | " +               // White + Bold
                $"<b><color=#4DA6FF>Good: {attempt.goodHits}</color></b> | " +                    // Softer Blue + Bold
                $"<b><color=#00FF00>Perfect: {attempt.perfectHits}</color></b> | " +             // Green + Bold
                $"<b><color=#FF0000>Miss: {attempt.missHits}</color></b> | " +                    // Red + Bold
                $"<b><color=#666666>{attempt.dateTime}</color></b>";                               // Gray + Bold
        }
    }
}
