using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PlayerProfile;

public class GameManager : MonoBehaviour
{

    [Header("Game Setup")]
    [SerializeField] private int numRows = 1;
    [SerializeField] private int numCols = 4;
    private int numTiles;
    private Tile[] tile;

    [Header("Game Objects")]
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Transform gameArea;
    [SerializeField] private GameObject playButton;

    [Header("Audio Setup")]
    [SerializeField] private float duration = 0.2f;
    [SerializeField] private AudioSource audioSource;

    [Header("UI Elements")]
    [SerializeField] private TMPro.TextMeshProUGUI levelText;
    [SerializeField] private TMPro.TextMeshProUGUI movesLeftText;
    [SerializeField] private UnityEngine.UI.Button replayButton;
    [SerializeField] private GameObject scoreboardButton;

    private int currentLevel = 1;
    private int hintsUsed = 0;
    private int progressIntoLevel = 0;


    enum GameMode
    {
        None,
        Menu,
        Listening,
        Playing
    }

    private GameMode gameMode = GameMode.None;

    // For tracking the level
    private List<int> levelTiles;
    private int currentIndex = 0;

    void Start()
    {
        // numTiles is global as we'll use it in lots of places.
        numTiles = numRows * numCols;
        tile = new Tile[numTiles];

        // Create the grid of tiles.
        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numCols; col++)
            {
                int index = (row * numCols) + col;

                // Instantiate the tile objects.
                tile[index] = Instantiate(tilePrefab, gameArea);
                tile[index].Init(this, index, Color.HSVToRGB((float)index / numTiles, 0.8f, 0.9f));

                // Center the tiles in the game area.
                float rowStart = (numRows / 2f) - 0.5f;
                float colStart = (-numCols / 2f) + 0.5f;
                tile[index].transform.localPosition = new Vector3(colStart + col, rowStart - row, 0f);
            }
        }

        // Scale the tiles to fit our vertical space (6 units)
        // (If there are too many cols they'll go off the edge).
        //Changed from 6f to 4f for resizing
        float scale = 4f / numRows; 
        gameArea.localScale = Vector3.one * scale;

        // Start in the menu game mode, with flashing lights and no sound.
        gameMode = GameMode.Menu;
        StartCoroutine(MenuTileAnimation());

        // Disable replay button at start
        replayButton.onClick.AddListener(ReplayCurrentPattern);
        replayButton.gameObject.SetActive(false);

        UpdateLevelText();
        UpdateMovesLeftText();
    }

    void Update()
    {
        if (gameMode != GameMode.Playing) return;

        if (Input.GetKeyDown(KeyCode.Z))
            PlayLightAndTone(0);

        if (Input.GetKeyDown(KeyCode.X))
            PlayLightAndTone(1);

        if (Input.GetKeyDown(KeyCode.C))
            PlayLightAndTone(2);

        if (Input.GetKeyDown(KeyCode.V))
            PlayLightAndTone(3);
    }

    private IEnumerator MenuTileAnimation()
    {
        while (gameMode == GameMode.Menu)
        {
            // Light a random tile.
            yield return FlashTile(Random.Range(0, numTiles));
            // Wait before flashing the next one.
            yield return new WaitForSeconds(duration);
        }
    }

    private void UpdateLevelText()
    {
        if (levelText != null)
            levelText.text = $"Level: {currentLevel}";
    }


    private void UpdateMovesLeftText()
    {
        if (movesLeftText != null)
        {
            int movesLeft = Mathf.Max(levelTiles.Count - currentIndex, 0);
            movesLeftText.text = $"Moves Left: {movesLeft}";
        }
    }


    private IEnumerator FlashTile(int index)
    {
        tile[index].TurnOn();
        yield return new WaitForSeconds(duration);
        tile[index].TurnOff();
    }

    public void PlayLightAndTone(int index)
    {
        if (gameMode != GameMode.Playing) return;

        StartCoroutine(FlashTile(index));

        if (index == levelTiles[currentIndex])
        {
            PlayTone(index);
            currentIndex++;
            progressIntoLevel = currentIndex; // track how far into pattern
            UpdateMovesLeftText();

            if (currentIndex == levelTiles.Count)
            {
                levelTiles.Add(Random.Range(0, numTiles)); // increase level
                currentLevel++;
                UpdateLevelText();
                StartCoroutine(PlaySequence());
            }
        }
        else
        {
            SaveSimonResult();
            // Game over
            Debug.Log($"You got to level {levelTiles.Count - 2}");
            gameMode = GameMode.Menu;
            playButton.SetActive(true);
            scoreboardButton.SetActive(true);
            replayButton.gameObject.SetActive(false);
            PlayErrorTone();
        }
    }



    public void ReplayCurrentPattern()
    {
        if (gameMode == GameMode.Playing || gameMode == GameMode.Listening)
        {
            hintsUsed++; // count hint usage
            StartCoroutine(PlaySequence());
        }
    }

    private void PlayErrorTone()
    {
        // Play a longer low pitched sound.
        audioSource.pitch = 0.5f;
        double currentTime = AudioSettings.dspTime;
        audioSource.PlayScheduled(currentTime);
        audioSource.SetScheduledEndTime(currentTime + 3 * duration);
    }

    private void PlayTone(int index)
    {
        // Adjust pitch to create unique sound for each tile.
        if (numTiles > 1)
        {
            audioSource.pitch = Mathf.Lerp(0.5f, 2.0f, index / (numTiles - 1f));
        }

        // Schedule the tone to play.
        double currentTime = AudioSettings.dspTime;
        audioSource.PlayScheduled(currentTime);
        audioSource.SetScheduledEndTime(currentTime + duration);
    }

    public void Play()
    {
        // Hide the play button.
        playButton.SetActive(false);
        scoreboardButton.SetActive(false);
        replayButton.gameObject.SetActive(false);

        // Stop the lights flashing from the menu.
        StopCoroutine(MenuTileAnimation());

        currentLevel = 1;
        UpdateLevelText();

        // Clear out the old level data, start with three lights.
        levelTiles = new() {
      Random.Range(0, numTiles),
      Random.Range(0, numTiles),
      Random.Range(0, numTiles)
    };

        // Play the game light sequence.
        StartCoroutine(PlaySequence());

    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator PlaySequence()
    {
        gameMode = GameMode.Listening;
        replayButton.gameObject.SetActive(false); // disable while showing sequence

        yield return new WaitForSeconds(2f);

        foreach (int index in levelTiles)
        {
            PlayTone(index);
            yield return FlashTile(index);
            yield return new WaitForSeconds(duration);
        }

        currentIndex = 0;
        gameMode = GameMode.Playing;

        replayButton.gameObject.SetActive(true); // enable after sequence
        UpdateMovesLeftText();
    }
    void SaveSimonResult()
    {
        var profile = ProfileManager.Instance?.currentProfile;

        if (profile == null)
        {
            Debug.LogWarning("No active profile. Simon result not saved.");
            return;
        }

        SimonAttemptData attempt = new SimonAttemptData(
            currentLevel,
            hintsUsed,
            progressIntoLevel
        );

        profile.simonAttempts.Add(attempt);
        ProfileManager.Instance.SaveProfiles();

        Debug.Log("Simon attempt saved.");
    }

    public void LoadScoreboard()
    {
        SceneManager.LoadScene("SimonScoreboard");
    }


}
