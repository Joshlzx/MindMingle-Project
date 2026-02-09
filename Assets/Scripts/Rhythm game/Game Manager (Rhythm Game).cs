using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager1 : MonoBehaviour
{
    public AudioSource theMusic;
    public bool startPlaying;
    public BeatScroller theBS;
    public static GameManager1 instance;

    public int currentScore;
    public int scorePerNote = 100;
    public int scorePerGoodNote = 125;
    public int scorePerPerfectNote = 150;

    public int currentMultiplier;
    public int multiplierTracker;
    public int[] multiplierThresholds;

    public Text scoreText;
    public Text multiText;

    public float totalNotes;
    public float normalHits;
    public float goodHits;
    public float perfectHits;
    public float missHits;

    public GameObject resultsScreen;
    public Text percentHitText, normalsText, goodsText, perfectsText, missesTexts, rankText, finalScoreText;

    void Start()
    {
        instance = this;
        scoreText.text = "Score: 0";
        currentMultiplier = 1;

        totalNotes = Object.FindObjectsByType<NoteObject>(FindObjectsSortMode.None).Length;
    }

    void Update()
    {
        if (!startPlaying)
        {
            if (Input.anyKeyDown)
            {
                startPlaying = true;
                theBS.hasStarted = true;
                theMusic.Play();
            }
        }
        else
        {
            if (!theMusic.isPlaying && !resultsScreen.activeInHierarchy)
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
        }
    }

    string CalculateRank(float percentHit)
    {
        if (percentHit == 100f) return "SS";
        if (percentHit > 95f) return "S";
        if (percentHit > 85f) return "A";
        if (percentHit > 70f) return "B";
        if (percentHit > 55f) return "C";
        if (percentHit > 40f) return "D";
        return "F";
    }

    void SaveRhythmAttempt(float percentHit, string rankVal)
    {
        var profile = ProfileManager.Instance?.currentProfile;
        if (profile == null)
        {
            Debug.LogWarning("No active profile. Rhythm attempt not saved.");
            return;
        }

        // Make sure the list exists
        if (profile.rhythmAttempts == null)
            profile.rhythmAttempts = new System.Collections.Generic.List<PlayerProfile.RhythmAttemptData>();

        PlayerProfile.RhythmAttemptData attempt = new PlayerProfile.RhythmAttemptData(
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
    }

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

    public void LoadRhythmMainMenu()
    {
        SceneManager.LoadScene("RhythmMainMenu");
    }
}
