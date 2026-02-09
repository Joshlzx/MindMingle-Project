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
                $"Score: {attempt.finalScore} | Rank: {attempt.rank} | Hit: {attempt.percentHit:F1}% | Normal: {attempt.normalHits} | Good: {attempt.goodHits} | Perfect: {attempt.perfectHits} | Miss: {attempt.missHits} | {attempt.dateTime}";
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

        // Sort all best attempts by percentHit descending
        bestAttempts.Sort((a, b) => b.percentHit.CompareTo(a.percentHit));

        // Display
        foreach (var attempt in bestAttempts)
        {
            GameObject entry = Instantiate(highscoreEntryPrefab, highscoresContent);
            entry.GetComponent<TextMeshProUGUI>().text =
                $"{attempt.playerName} | Score: {attempt.finalScore} | Rank: {attempt.rank} | Hit: {attempt.percentHit:F1}% | Normal: {attempt.normalHits} | Good: {attempt.goodHits} | Perfect: {attempt.perfectHits} | Miss: {attempt.missHits} | {attempt.dateTime}";
        }
    }
}
