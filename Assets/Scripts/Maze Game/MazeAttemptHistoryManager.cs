using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MazeAttemptHistoryManager : MonoBehaviour
{
    public Transform attemptsContent;
    public GameObject attemptEntryPrefab;

    public Transform highscoresContent;
    public GameObject highscoreEntryPrefab;

    private void OnEnable()
    {
        ShowCurrentPlayerAttempts();
        ShowAllPlayersHighscores();
    }

    void ShowCurrentPlayerAttempts()
    {
        var profile = ProfileManager.Instance?.currentProfile;
        if (profile == null || profile.mazeAttempts == null) return;

        foreach (Transform child in attemptsContent)
            Destroy(child.gameObject);

        List<PlayerProfile.MazeAttemptData> attempts =
            new List<PlayerProfile.MazeAttemptData>(profile.mazeAttempts);

        // 🔹 Sort by most recent attempt
        attempts.Sort((a, b) =>
        {
            System.DateTime dateA = System.DateTime.Parse(a.dateTime);
            System.DateTime dateB = System.DateTime.Parse(b.dateTime);
            return dateB.CompareTo(dateA);
        });

        foreach (var attempt in attempts)
        {
            GameObject entry = Instantiate(attemptEntryPrefab, attemptsContent);

            entry.GetComponent<TextMeshProUGUI>().text =
                $"Level: {attempt.levelReached} | Hints: {attempt.totalHintsUsed} | {attempt.dateTime}";
        }
    }


    void ShowAllPlayersHighscores()
    {
        foreach (Transform child in highscoresContent)
            Destroy(child.gameObject);

        if (ProfileManager.Instance == null) return;

        List<PlayerProfile.MazeAttemptData> bestAttempts =
            new List<PlayerProfile.MazeAttemptData>();

        foreach (var p in ProfileManager.Instance.profiles)
        {
            if (p.mazeAttempts == null || p.mazeAttempts.Count == 0)
                continue;

            PlayerProfile.MazeAttemptData best = p.mazeAttempts[0];

            foreach (var a in p.mazeAttempts)
            {
                if (a.levelReached > best.levelReached ||
                   (a.levelReached == best.levelReached && a.totalHintsUsed < best.totalHintsUsed))
                {
                    best = a;
                }
            }

            bestAttempts.Add(best);
        }

        bestAttempts.Sort((a, b) => b.levelReached.CompareTo(a.levelReached));

        foreach (var attempt in bestAttempts)
        {
            GameObject entry = Instantiate(highscoreEntryPrefab, highscoresContent);

            entry.GetComponent<TextMeshProUGUI>().text =
                $"{attempt.playerName} | Level: {attempt.levelReached} | Hints: {attempt.totalHintsUsed} | {attempt.dateTime}";
        }
    }
}
