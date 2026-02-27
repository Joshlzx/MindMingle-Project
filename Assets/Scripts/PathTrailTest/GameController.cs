using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using UnityEngine.SceneManagement;
using static PlayerProfile;

public class GameController : MonoBehaviour
{
    [Header("References")]
    public TrailGenerator generator;
    public UILineRenderer uiLineRenderer;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI errorText;
    public TextMeshProUGUI resultText; // For result panel

    [Header("Result Screen")]
    public GameObject resultPanel;
    public Button playAgainButton;
    public Button mainMenuButton;

    private int currentIndex = 0;
    private int errors = 0;
    private float timer = 0f;
    private bool gameActive = true;

    private List<PathNode> selectedNodes = new List<PathNode>();
    private List<PathNode> currentOptions = new List<PathNode>();

    void Start()
    {
        // Start game after nodes are spawned
        if (generator.spawnedNodes.Count == 0)
            StartCoroutine(InitNextFrame());
        else
            GenerateOptions();

        // Setup result panel buttons
        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(PlayAgain);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(BackToMainMenu);

        if (resultPanel != null)
            resultPanel.SetActive(false); // hide at start
    }

    IEnumerator InitNextFrame()
    {
        yield return null;
        GenerateOptions();
    }

    void Update()
    {
        if (!gameActive) return;

        timer += Time.deltaTime;
        timerText.text = $"Time: {timer:F1}";
        errorText.text = $"Errors: {errors}";

        if (Input.GetKeyDown(KeyCode.Z)) SelectOption(0);
        if (Input.GetKeyDown(KeyCode.X)) SelectOption(1);
        if (Input.GetKeyDown(KeyCode.C)) SelectOption(2);
        if (Input.GetKeyDown(KeyCode.V)) Undo();
    }

    void GenerateOptions()
    {
        currentOptions.Clear();

        string correctValue = generator.correctSequence[currentIndex];
        PathNode correctNode = generator.spawnedNodes.Find(n => n.nodeValue == correctValue);
        currentOptions.Add(correctNode);

        while (currentOptions.Count < 3)
        {
            PathNode randomNode = generator.spawnedNodes[Random.Range(0, generator.spawnedNodes.Count)];
            if (!currentOptions.Contains(randomNode))
                currentOptions.Add(randomNode);
        }

        Shuffle(currentOptions);
        HighlightOptions();
    }

    void HighlightOptions()
    {
        foreach (var node in generator.spawnedNodes)
            node.ResetColor();

        for (int i = 0; i < currentOptions.Count; i++)
            currentOptions[i].ShowIndicator(i); // 0=red, 1=blue, 2=yellow
    }

    void SelectOption(int index)
    {
        if (index >= currentOptions.Count) return;

        PathNode selected = currentOptions[index];
        string correctValue = generator.correctSequence[currentIndex];

        if (selected.nodeValue == correctValue)
        {
            selected.Highlight(Color.green);
            selectedNodes.Add(selected);
            DrawLine();

            currentIndex++;

            if (currentIndex >= generator.correctSequence.Count)
            {
                gameActive = false;
                OnGameFinished();
                return;
            }

            GenerateOptions();
        }
        else
        {
            errors++;
            selected.Highlight(Color.red);
        }
    }

    void Undo()
    {
        if (selectedNodes.Count == 0) return;

        PathNode last = selectedNodes[selectedNodes.Count - 1];
        last.ResetColor();

        selectedNodes.RemoveAt(selectedNodes.Count - 1);
        currentIndex--;

        DrawLine();
        GenerateOptions();
    }

    void DrawLine()
    {
        if (selectedNodes.Count == 0)
        {
            uiLineRenderer.Points = new Vector2[0];
            return;
        }

        Vector2[] points = new Vector2[selectedNodes.Count];
        RectTransform lineRect = uiLineRenderer.GetComponent<RectTransform>();

        for (int i = 0; i < selectedNodes.Count; i++)
        {
            Vector2 nodePos = RectTransformUtility.WorldToScreenPoint(null, selectedNodes[i].transform.position);

            if (i > 0)
            {
                Vector2 prevPos = RectTransformUtility.WorldToScreenPoint(null, selectedNodes[i - 1].transform.position);
                Vector2 dir = (nodePos - prevPos).normalized;

                nodePos -= dir * generator.nodeRadius;
                prevPos += dir * generator.nodeRadius;

                points[i - 1] = ScreenToUILocal(prevPos, lineRect);
            }

            points[i] = ScreenToUILocal(nodePos, lineRect);
        }

        uiLineRenderer.Points = points;
    }

    Vector2 ScreenToUILocal(Vector2 screenPos, RectTransform rect)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPos, null, out Vector2 localPoint);
        return localPoint;
    }

    void Shuffle(List<PathNode> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            PathNode temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    void OnGameFinished()
    {
        var profile = ProfileManager.Instance?.currentProfile;
        if (profile != null)
        {
            PathTrailAttemptData attempt = new PathTrailAttemptData(timer, errors, profile.playerName);
            profile.pathTrailAttempts.Add(attempt);
            ProfileManager.Instance.SaveProfiles();

            Debug.Log($"PathTrail Completed! Time: {attempt.completionTime:F1}s, Errors: {attempt.totalErrors}, Date: {attempt.dateTime}");
        }

        // Hide gameplay UI
        timerText.gameObject.SetActive(false);
        errorText.gameObject.SetActive(false);
        uiLineRenderer.gameObject.SetActive(false);
        foreach (var node in generator.spawnedNodes)
            node.gameObject.SetActive(false);

        // Show result panel
        if (resultPanel != null)
            resultPanel.SetActive(true);

        // Display result text
        if (resultText != null && profile != null)
        {
            resultText.text =
            $"<b><color=#000000>COMPLETED!</color></b>\n" +
            $"<b><color=#9C27B0>Time: {timer:F1}s</color></b>\n" +
            $"<b><color=#FF8C00>Errors: {errors}</color></b>";
        }
    }

    void PlayAgain()
    {
        // Hide result panel
        if (resultPanel != null)
            resultPanel.SetActive(false);

        // Reset game state
        currentIndex = 0;
        errors = 0;
        timer = 0f;
        selectedNodes.Clear();
        currentOptions.Clear();
        gameActive = true;

        timerText.gameObject.SetActive(true);
        errorText.gameObject.SetActive(true);
        uiLineRenderer.gameObject.SetActive(true);

        foreach (var node in generator.spawnedNodes)
        {
            node.gameObject.SetActive(true);
            node.ResetColor();
        }

        uiLineRenderer.Points = new Vector2[0];
        timerText.text = "Time: 0.0";
        errorText.text = "Errors: 0";

        GenerateOptions();
    }

    void BackToMainMenu()
    {
        SceneManager.LoadScene("PathTrailMenu");
    }
}