using UnityEngine;
using UnityEngine.UI;
using TMPro;

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


    bool levelInProgress = false;

    void Start()
    {
        UpdateLevelText();
        StartLevel();
    }

    void StartLevel()
    {
        levelInProgress = true;

        Vector2Int mazeSize = GetMazeSizeForLevel();
        mazeGenerator.GenerateNewMaze(mazeSize, this);

        UpdateLevelText(); // Update UI when new level starts

        Debug.Log("Starting Level " + currentLevel);
    }

    public void OnLevelComplete()
    {
        if (!levelInProgress) return;
        levelInProgress = false;

        Debug.Log("Level " + currentLevel + " Complete!");
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
}
