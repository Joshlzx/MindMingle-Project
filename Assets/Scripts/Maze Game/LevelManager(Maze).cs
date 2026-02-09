using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    [Header("Level Settings")]
    public int currentLevel = 1;
    public float levelDelay = 1f;

    [Header("Maze Settings")]
    public MazeGenerator mazeGenerator;
    public Vector2Int startMazeSize = new Vector2Int(4, 4);
    public int levelsPerIncrease = 2;
    public int maxMazeSize = 10;

    [Header("UI")]
    public TMP_Text levelText;
    public LevelCompleteUI levelCompleteUI;
    public HintManager hintManager;

    // Track hints per level
    public List<LevelHintData> levelHintHistory = new List<LevelHintData>();

    private int totalHintsUsedThisRun = 0;
    private bool levelInProgress = false;
    private bool runActive = false;

    void Start()
    {
        StartNewRun();
    }

    void StartNewRun()
    {
        levelHintHistory.Clear();
        totalHintsUsedThisRun = 0;
        currentLevel = 1;
        runActive = true;

        UpdateLevelText();
        StartLevel();
    }

    void StartLevel()
    {
        levelInProgress = true;

        if (hintManager != null)
            hintManager.hintsUsedThisLevel = 0;

        Vector2Int mazeSize = GetMazeSizeForLevel();
        mazeGenerator.GenerateNewMaze(mazeSize, this);

        UpdateLevelText();
        Debug.Log("Starting Level " + currentLevel);
    }

    public void OnLevelComplete()
    {
        if (!levelInProgress) return;
        levelInProgress = false;

        int hintsUsed = hintManager != null ? hintManager.hintsUsedThisLevel : 0;

        levelHintHistory.Add(new LevelHintData(currentLevel, hintsUsed));
        totalHintsUsedThisRun += hintsUsed;

        Debug.Log($"Level {currentLevel} complete. Hints: {hintsUsed}");

        // Save checkpoint progress
        SaveCheckpointProgress();

        Invoke(nameof(NextLevel), levelDelay);

        if (levelCompleteUI != null)
            levelCompleteUI.Show();
    }

    void NextLevel()
    {
        currentLevel++;
        UpdateLevelText();
        StartLevel();
    }

    Vector2Int GetMazeSizeForLevel()
    {
        int increase = (currentLevel - 1) / levelsPerIncrease;
        int sizeX = Mathf.Min(startMazeSize.x + increase, maxMazeSize);
        int sizeY = Mathf.Min(startMazeSize.y + increase, maxMazeSize);
        return new Vector2Int(sizeX, sizeY);
    }

    void UpdateLevelText()
    {
        if (levelText != null)
            levelText.text = "Level: " + currentLevel;
    }

    // 🔹 SAVE PROGRESS EVERY LEVEL (checkpoint)
    void SaveCheckpointProgress()
    {
        var profile = ProfileManager.Instance?.currentProfile;
        if (profile == null) return;

        if (profile.mazeAttempts == null)
            profile.mazeAttempts = new List<PlayerProfile.MazeAttemptData>();

        PlayerProfile.MazeAttemptData last = null;
        if (profile.mazeAttempts.Count > 0)
            last = profile.mazeAttempts[profile.mazeAttempts.Count - 1];

        // Update last checkpoint if same run
        if (last != null && last.levelReached < currentLevel)
        {
            last.levelReached = currentLevel;
            last.totalHintsUsed = totalHintsUsedThisRun;
            last.dateTime = System.DateTime.Now.ToString("dd MMM yyyy HH:mm");
        }
        else
        {
            profile.mazeAttempts.Add(
                new PlayerProfile.MazeAttemptData(
                    profile.playerName,
                    currentLevel,
                    totalHintsUsedThisRun
                )
            );
        }

        ProfileManager.Instance.SaveProfiles();
    }

    // 🔹 FINAL ATTEMPT WHEN PLAYER PRESSES BACK / EXIT
    public void ExitMaze()
    {
        FinalizeAttempt();
        SceneManager.LoadScene("MazeMainMenu");
    }

    void FinalizeAttempt()
    {
        if (!runActive) return;
        runActive = false;

        SaveCheckpointProgress();

        Debug.Log("Maze run finalized.");
    }

    [System.Serializable]
    public class LevelHintData
    {
        public int level;
        public int hintsUsed;

        public LevelHintData(int lvl, int hints)
        {
            level = lvl;
            hintsUsed = hints;
        }
    }
}
