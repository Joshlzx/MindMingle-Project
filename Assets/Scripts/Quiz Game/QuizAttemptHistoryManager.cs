using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuizAttemptHistoryManager : MonoBehaviour
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

        if (profile == null || profile.quizAttempts == null) return;

        foreach (Transform child in attemptsContent)
            Destroy(child.gameObject);

        
        List<PlayerProfile.QuizAttemptData> sortedAttempts = new List<PlayerProfile.QuizAttemptData>(profile.quizAttempts);

      
        sortedAttempts.Sort((a, b) =>
        {
            System.DateTime dateA = System.DateTime.Parse(a.dateTime);
            System.DateTime dateB = System.DateTime.Parse(b.dateTime);
            return dateB.CompareTo(dateA);
        });


        foreach (var attempt in sortedAttempts)
        {
            GameObject entry = Instantiate(attemptEntryPrefab, attemptsContent);

            entry.GetComponent<TextMeshProUGUI>().text =
                $"<b>{attempt.correctAnswers}/{attempt.totalQuestions}</b>  |  " +          // Bold, default color
                $"<b><color=#666666>{attempt.dateTime}</color></b>";                         // Gray + Bold
        }
    }


    void ShowAllPlayersHighscores()
    {
        foreach (Transform child in highscoresContent)
            Destroy(child.gameObject);

        if (ProfileManager.Instance == null) return;

        // Create a temporary list to sort players by best score
        List<(string playerName, PlayerProfile.QuizAttemptData bestAttempt)> highscoreList
            = new List<(string, PlayerProfile.QuizAttemptData)>();

        foreach (var p in ProfileManager.Instance.profiles)
        {
            if (p.quizAttempts == null || p.quizAttempts.Count == 0)
                continue;

            PlayerProfile.QuizAttemptData best = p.quizAttempts[0];

            foreach (var a in p.quizAttempts)
            {
                if (a.correctAnswers > best.correctAnswers)
                    best = a;
            }

            highscoreList.Add((p.playerName, best));
        }

        // SORT by highest score
        highscoreList.Sort((a, b) => b.bestAttempt.correctAnswers.CompareTo(a.bestAttempt.correctAnswers));

        // Spawn UI
        foreach (var entryData in highscoreList)
        {
            GameObject entry = Instantiate(highscoreEntryPrefab, highscoresContent);

            entry.GetComponent<TextMeshProUGUI>().text =
                $"<b><color=#000000>{entryData.playerName}</color></b>" +                          // Black + Bold
                $"  |  <b>{entryData.bestAttempt.correctAnswers}/{entryData.bestAttempt.totalQuestions}</b>" + // Bold
                $"  |  <b><color=#666666>{entryData.bestAttempt.dateTime}</color></b>";            // Gray + Bold
        }
    }


}
