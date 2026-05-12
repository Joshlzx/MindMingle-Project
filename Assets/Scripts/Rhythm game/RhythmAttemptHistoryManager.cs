using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class RhythmAttemptHistoryManager : MonoBehaviour
{

    public TMPro.TextMeshProUGUI mapTitle;

    public Transform attemptsContent;
    public GameObject attemptEntryPrefab;

    public Transform highscoresContent;
    public GameObject highscoreEntryPrefab;

    [Header("Beatmap Navigation")]
    public List<string> mapIDs = new List<string>() { 
        "Yue Liang Dai Biao Wo De Xin",
        "Where I Belong",
        "Home",
        "Tian Mi Mi",
        "Chan Mali Chan", 
        "Munnaeru Vaalibaa" };

    private int currentMapIndex = 0;
    private string currentMapID;


    private void Start()
    {
        currentMapID = mapIDs[currentMapIndex];

        ShowCurrentPlayerAttempts();
        ShowAllPlayersHighscores();

        currentMapIndex = GetNextValidMapIndex(-1);

        
        if (currentMapIndex < 0)
            currentMapIndex = 0;

        
        RefreshScoreboard();
    }

    void ShowCurrentPlayerAttempts()
    {
        var profile = ProfileManager.Instance?.currentProfile;
        if (profile == null || profile.rhythmAttempts == null) return;

        
        foreach (Transform child in attemptsContent)
            Destroy(child.gameObject);

        
        List<PlayerProfile.RhythmAttemptData> attemptsSorted = new List<PlayerProfile.RhythmAttemptData>(profile.rhythmAttempts);
        attemptsSorted.Sort((a, b) => b.dateTime.CompareTo(a.dateTime));

        foreach (var attempt in attemptsSorted)
        {
            if (attempt.mapID != currentMapID)
                continue;
            GameObject entry = Instantiate(attemptEntryPrefab, attemptsContent);

            entry.GetComponent<TextMeshProUGUI>().text =
                $"<b><color=#CC00FF>Score: {attempt.finalScore}</color></b> | " +         
                $"<b><color=#FFD700>Rank: {attempt.rank}</color></b> | " +                
                $"<b><color=#00FFFF>Hit: {attempt.percentHit:F1}%</color></b> | " +       
                //$"<b><color=#FFFFFF>Normal: {attempt.normalHits}</color></b> | " +        
                //$"<b><color=#4DA6FF>Good: {attempt.goodHits}</color></b> | " +            
                //$"<b><color=#00FF00>Perfect: {attempt.perfectHits}</color></b> | " +      
                //$"<b><color=#FF0000>Miss: {attempt.missHits}</color></b> | " +            
                $"<b><color=#666666>{attempt.dateTime}</color></b>";                      
        }
    }

    void ShowAllPlayersHighscores()
    {
        
        foreach (Transform child in highscoresContent)
            Destroy(child.gameObject);

        if (ProfileManager.Instance == null) return;

        List<PlayerProfile.RhythmAttemptData> bestAttempts = new List<PlayerProfile.RhythmAttemptData>();

        
        foreach (var p in ProfileManager.Instance.profiles)
        {
            if (p.rhythmAttempts == null || p.rhythmAttempts.Count == 0)
                continue;

           
            var attemptsForMap = p.rhythmAttempts.FindAll(a => a.mapID == currentMapID);
            if (attemptsForMap.Count == 0)
                continue; 

            
            PlayerProfile.RhythmAttemptData best = attemptsForMap[0];
            foreach (var a in attemptsForMap)
            {
                if (a.percentHit > best.percentHit)
                    best = a;
            }

            bestAttempts.Add(best);
        }

        
        bestAttempts.Sort((a, b) => b.percentHit.CompareTo(a.percentHit));

        
        foreach (var attempt in bestAttempts)
        {
            GameObject entry = Instantiate(highscoreEntryPrefab, highscoresContent);

            entry.GetComponent<TextMeshProUGUI>().text =
                $"<b><color=#000000>{attempt.playerName}</color></b> | " +
                $"<b><color=#CC00FF>Score: {attempt.finalScore}</color></b> | " +
                $"<b><color=#FFD700>Rank: {attempt.rank}</color></b> | " +
                $"<b><color=#00FFFF>Hit: {attempt.percentHit:F1}%</color></b> | " +
                //$"<b><color=#FFFFFF>Normal: {attempt.normalHits}</color></b> | " +
                //$"<b><color=#4DA6FF>Good: {attempt.goodHits}</color></b> | " +
                //$"<b><color=#00FF00>Perfect: {attempt.perfectHits}</color></b> | " +
                //$"<b><color=#FF0000>Miss: {attempt.missHits}</color></b> | " +
                $"<b><color=#666666>{attempt.dateTime}</color></b>";
        }
    }
    public void NextMap()
    {
        currentMapIndex++;

        if (currentMapIndex >= mapIDs.Count)
            currentMapIndex = 0;

        currentMapID = mapIDs[currentMapIndex];

        RefreshScoreboard();
    }

    public void PreviousMap()
    {
        currentMapIndex--;

        if (currentMapIndex < 0)
            currentMapIndex = mapIDs.Count - 1;

        currentMapID = mapIDs[currentMapIndex];

        RefreshScoreboard();
    }
    void RefreshScoreboard()
    {
        ShowCurrentPlayerAttempts();
        ShowAllPlayersHighscores();
        mapTitle.text = currentMapID;
    }
    private int GetNextValidMapIndex(int startIndex)
    {
        int count = mapIDs.Count;
        int index = (startIndex + 1) % count;

        while (mapIDs[index] == "RhythmResult" || string.IsNullOrEmpty(mapIDs[index]))
        {
            index = (index + 1) % count;
        }

        return index;
    }
}
