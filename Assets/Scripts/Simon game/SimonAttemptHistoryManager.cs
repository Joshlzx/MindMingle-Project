using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SimonAttemptHistoryManager : MonoBehaviour
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
        if (profile == null || profile.simonAttempts == null) return;

        // Clear existing entries
        foreach (Transform child in attemptsContent)
            Destroy(child.gameObject);

        // Sort attempts by dateTime descending (latest first)
        profile.simonAttempts.Sort((a, b) => System.DateTime.Parse(b.dateTime).CompareTo(System.DateTime.Parse(a.dateTime)));

        // Populate entries
        foreach (PlayerProfile.SimonAttemptData attempt in profile.simonAttempts)
        {
            GameObject entry = Instantiate(attemptEntryPrefab, attemptsContent);

            entry.GetComponent<TextMeshProUGUI>().text =
                $"<b><color=#9C27B0>Level {attempt.levelReached}</color></b>  |  " +   
                $"<b><color=#FF8C00>Hints {attempt.hintsUsed}</color></b>  |  " +      
                $"<b><color=#666666>{attempt.dateTime}</color></b>";                    
        }
    }


    void ShowAllPlayersHighscores()
    {
        foreach (Transform child in highscoresContent)
            Destroy(child.gameObject);

        if (ProfileManager.Instance == null) return;

        // Step 1: Collect best attempt for each player
        List<(string playerName, PlayerProfile.SimonAttemptData bestAttempt)> bestAttempts = new List<(string, PlayerProfile.SimonAttemptData)>();

        foreach (var p in ProfileManager.Instance.profiles)
        {
            if (p.simonAttempts == null || p.simonAttempts.Count == 0)
                continue;

            PlayerProfile.SimonAttemptData best = p.simonAttempts[0];

            foreach (var a in p.simonAttempts)
            {
                if (IsBetterSimonAttempt(a, best))
                    best = a;
            }

            bestAttempts.Add((p.playerName, best));
        }

        // Step 2: Sort all players by best attempt (highest level → progress → fewest hints)
        bestAttempts.Sort((x, y) => CompareSimonAttempts(y.bestAttempt, x.bestAttempt)); // descending order

        // Step 3: Populate UI
        foreach (var entryData in bestAttempts)
        {
            GameObject entry = Instantiate(highscoreEntryPrefab, highscoresContent);

            entry.GetComponent<TextMeshProUGUI>().text =
                $"<b><color=#000000>{entryData.playerName}</color></b>  |  " +                  
                $"<b><color=#9C27B0>Level {entryData.bestAttempt.levelReached}</color></b>  |  " +  
                $"<b><color=#FF8C00>Hints {entryData.bestAttempt.hintsUsed}</color></b>  |  " +     
                $"<b><color=#666666>{entryData.bestAttempt.dateTime}</color></b>";                   
        }
    }

    // Compare two Simon attempts: returns positive if a > b
    int CompareSimonAttempts(PlayerProfile.SimonAttemptData a, PlayerProfile.SimonAttemptData b)
    {
        if (a.levelReached != b.levelReached)
            return a.levelReached.CompareTo(b.levelReached); // higher level wins
        if (a.progressIntoLevel != b.progressIntoLevel)
            return a.progressIntoLevel.CompareTo(b.progressIntoLevel); // further progress wins
        return b.hintsUsed.CompareTo(a.hintsUsed); // fewer hints wins
    }

    bool IsBetterSimonAttempt(PlayerProfile.SimonAttemptData a, PlayerProfile.SimonAttemptData b)
    {
        // Same logic as CompareSimonAttempts but returns bool
        if (a.levelReached > b.levelReached) return true;
        if (a.levelReached < b.levelReached) return false;
        if (a.progressIntoLevel > b.progressIntoLevel) return true;
        if (a.progressIntoLevel < b.progressIntoLevel) return false;
        if (a.hintsUsed < b.hintsUsed) return true;
        return false;
    }

}
