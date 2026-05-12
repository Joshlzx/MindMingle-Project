using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager1 : MonoBehaviour
{
    [Header("Beatmap")]
    public string mapID;

    [Header("Audio & Game Control")]
    public AudioSource theMusic;
    public bool startPlaying;

    [Header("Scoring")]
    public int currentScore;
    public int scorePerNote = 100;
    public int scorePerGoodNote = 125;
    public int scorePerPerfectNote = 150;

    [Header("Multiplier")]
    public int currentMultiplier;
    public int multiplierTracker;
    public int[] multiplierThresholds;

    [Header("UI")]
    public Text scoreText;
    public Text multiText;

    [Header("Stats")]
    public float totalNotes;
    public float normalHits;
    public float goodHits;
    public float perfectHits;
    public float missHits;

    [Header("Results Screen")]
    public GameObject resultsScreen;
    public Text percentHitText, normalsText, goodsText, perfectsText, missesTexts, rankText, finalScoreText;

    [Header("Pause UI (optional)")]
    public GameObject pausePanel; // assign a pause overlay in the inspector (optional)

    // runtime suspend state
    private bool isSuspended = false;
    private bool wasPlayingBeforeSuspend = false;

    public static GameManager1 instance;

    void Start()
    {
        instance = this;
        scoreText.text = "Score: 0";
        currentMultiplier = 1;

        // Count total notes in scene (NoteObjects must exist in the scene)
        totalNotes = UnityEngine.Object.FindObjectsByType<NoteObject>(FindObjectsSortMode.None).Length;
    }

    void Update()
    {
        // when suspended (lost focus / paused) freeze gameplay logic
        if (isSuspended) return;

        if (!startPlaying)
        {
            // Start game on any key
            if (Input.anyKeyDown)
            {
                startPlaying = true;
                theMusic.Play();
            }
            return;
        }

        // Check if song finished
        if (!theMusic.isPlaying && !resultsScreen.activeInHierarchy)
        {
            ShowResults();
        }
    }

    // Called when the application gains/loses focus (desktop)
    void OnApplicationFocus(bool hasFocus)
    {
        HandleSuspend(!hasFocus);
    }

    // Called when the application is paused/resumed (mobile / some platforms)
    void OnApplicationPause(bool isPaused)
    {
        HandleSuspend(isPaused);
    }

    // Centralized suspend/resume logic
    void HandleSuspend(bool suspend)
    {
        if (suspend == isSuspended) return;

        if (suspend)
        {
            // going out of focus / pausing
            isSuspended = true;

            // record whether music was playing so we can resume later
            if (theMusic != null)
            {
                wasPlayingBeforeSuspend = theMusic.isPlaying;
                if (theMusic.isPlaying) theMusic.Pause();
            }

            // freeze game time so physics, animations and WaitForSeconds stop
            Time.timeScale = 0f;

            if (pausePanel != null) pausePanel.SetActive(true);
        }
        else
        {
            // returning to focus / resuming
            isSuspended = false;

            // restore time scale first so coroutines / Update run normally
            Time.timeScale = 1f;

            if (pausePanel != null) pausePanel.SetActive(false);

            // resume audio if it was playing before suspend
            if (theMusic != null && wasPlayingBeforeSuspend)
            {
                theMusic.UnPause();
            }
        }
    }

    private void ShowResults()
    {
        resultsScreen.SetActive(true);

        normalsText.text = normalHits.ToString();
        goodsText.text = goodHits.ToString();
        perfectsText.text = perfectHits.ToString();
        missesTexts.text = missHits.ToString();

        float totalHit = normalHits + goodHits + perfectHits;
        float percentHit = (totalHit / totalNotes) * 100f;

        percentHitText.text = percentHit.ToString("F1") + "%";
        string rankVal = CalculateRank(percentHit);
        rankText.text = rankVal;
        finalScoreText.text = currentScore.ToString();

        // Save attempt to player profile
        SaveRhythmAttempt(percentHit, rankVal);
    }

    private string CalculateRank(float percentHit)
    {
        if (percentHit == 100f) return "Perfect!";
        if (percentHit > 95f) return "Excellent!";
        if (percentHit > 85f) return "Good";
        if (percentHit > 70f) return "Fair";
        if (percentHit > 55f) return "Average";
        if (percentHit > 40f) return "Poor";
        return "F";
    }

    private void SaveRhythmAttempt(float percentHit, string rankVal)
    {
        var profile = ProfileManager.Instance?.currentProfile;
        if (profile == null)
        {
            Debug.LogWarning("No active profile. Rhythm attempt not saved.");
            return;
        }

        if (profile.rhythmAttempts == null)
            profile.rhythmAttempts = new System.Collections.Generic.List<PlayerProfile.RhythmAttemptData>();

        PlayerProfile.RhythmAttemptData attempt = new PlayerProfile.RhythmAttemptData(
            mapID,
            totalNotes,
            normalHits,
            goodHits,
            perfectHits,
            missHits,
            currentScore,
            rankVal,
            profile.playerName
        );

        profile.rhythmAttempts.Add(attempt);
        ProfileManager.Instance.SaveProfiles();

        Debug.Log($"Rhythm attempt saved | Score: {currentScore} | PercentHit: {percentHit:F1}% | Rank: {rankVal}");
        Debug.Log("Saved attempt mapID: " + mapID);
    }

    #region Note Hit Methods
    public void NoteHit()
    {
        if (currentMultiplier - 1 < multiplierThresholds.Length)
        {
            multiplierTracker++;
            if (multiplierThresholds[currentMultiplier - 1] <= multiplierTracker)
            {
                multiplierTracker = 0;
                currentMultiplier++;
            }
        }

        multiText.text = "Multiplier: x" + currentMultiplier;
        scoreText.text = "Score: " + currentScore;
    }

    public void NormalHit()
    {
        currentScore += scorePerNote * currentMultiplier;
        NoteHit();
        normalHits++;
    }

    public void GoodHit()
    {
        currentScore += scorePerGoodNote * currentMultiplier;
        NoteHit();
        goodHits++;
    }

    public void PerfectHit()
    {
        currentScore += scorePerPerfectNote * currentMultiplier;
        NoteHit();
        perfectHits++;
    }

    public void NoteMissed()
    {
        currentMultiplier = 1;
        multiplierTracker = 0;
        multiText.text = "Multiplier: x" + currentMultiplier;
        missHits++;
    }
    #endregion

    public void LoadRhythmMainMenu()
    {
        SceneManager.LoadScene("RhythmMainMenu");
    }
}