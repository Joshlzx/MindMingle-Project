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
    public TextMeshProUGUI resultText;

    [Header("Result Screen")]
    public GameObject resultPanel;
    public Button playAgainButton;
    public Button mainMenuButton;

    private int currentIndex = 0;
    private int errors = 0;
    private float timer = 0f;
    private bool gameActive = true;
    private PathNode currentNode = null;

    private List<PathNode> selectedNodes = new List<PathNode>();
    private List<PathNode> currentOptions = new List<PathNode>();
    private List<PathNode> stepWrongNodes = new List<PathNode>();

    void Start()
    {
        if (generator.spawnedNodes.Count == 0)
            StartCoroutine(InitNextFrame());
        else
            GenerateOptions();

        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(PlayAgain);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(BackToMainMenu);

        if (resultPanel != null)
            resultPanel.SetActive(false);
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

        // Key inputs for selecting options
        if (Input.GetKeyDown(KeyCode.Z)) SelectOption(0);
        if (Input.GetKeyDown(KeyCode.X)) SelectOption(1);
        if (Input.GetKeyDown(KeyCode.C)) SelectOption(2);
        if (Input.GetKeyDown(KeyCode.V)) SelectOption(3);
    }

    void GenerateOptions()
    {
        currentOptions.Clear();

        string correctValue = generator.correctSequence[currentIndex];
        PathNode correctNode = generator.spawnedNodes
            .Find(n => n.nodeValue == correctValue);

        currentOptions.Add(correctNode);

        // Add random nodes until we have 4 options
        while (currentOptions.Count < 4)
        {
            PathNode randomNode = generator.spawnedNodes[Random.Range(0, generator.spawnedNodes.Count)];

            // Skip if it's already in options OR if it's the current node
            if (!currentOptions.Contains(randomNode) && randomNode != currentNode)
            {
                currentOptions.Add(randomNode);
            }
        }

        Shuffle(currentOptions);
        HighlightOptions();
    }

    void HighlightOptions()
    {
        foreach (var node in generator.spawnedNodes)
            node.ResetIndicators();

        for (int i = 0; i < currentOptions.Count; i++)
            currentOptions[i].ShowIndicator(i);
    }

    void SelectOption(int index)
    {
        if (index >= currentOptions.Count) return;

        PathNode selected = currentOptions[index];
        string correctValue = generator.correctSequence[currentIndex];

        if (selected.nodeValue == correctValue)
        {
            // Correct selection

            // Stop pulse and mark previous node green
            if (currentNode != null)
            {
                currentNode.StopCurrentHighlight();
                currentNode.Highlight(Color.green);
            }

            // Reset all wrong nodes from this step
            foreach (var wrongNode in stepWrongNodes)
            {
                wrongNode.ResetIndicators();
            }
            stepWrongNodes.Clear();

            // Set new current node and start pulsing
            currentNode = selected;
            currentNode.StartCurrentHighlight();

            // Add to selected nodes for line drawing
            selectedNodes.Add(selected);
            DrawLine();

            currentIndex++;

            // Check if finished
            if (currentIndex >= generator.correctSequence.Count)
            {
                gameActive = false;
                currentNode.StopCurrentHighlight();
                currentNode.Highlight(Color.green);
                OnGameFinished();
                return;
            }

            // Generate options for next step
            GenerateOptions();
        }
        else
        {
            // Wrong selection

            // Only increment errors if this node hasn't been selected wrong before in this step
            if (!stepWrongNodes.Contains(selected))
            {
                errors++;
                selected.Highlight(Color.red);
                stepWrongNodes.Add(selected);
            }

            // Current node keeps pulsing; do NOT generate new options
        }
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
        gameActive = false;

        // SAVE ATTEMPT
        SaveAttempt();

        timerText.gameObject.SetActive(false);
        errorText.gameObject.SetActive(false);
        uiLineRenderer.gameObject.SetActive(false);

        foreach (var node in generator.spawnedNodes)
            node.gameObject.SetActive(false);

        if (resultPanel != null)
            resultPanel.SetActive(true);

        if (resultText != null)
        {
            resultText.text =
                $"<b><color=#000000>COMPLETED!</color></b>\n" +
                $"<b><color=#9C27B0>Time: {timer:F1}s</color></b>\n" +
                $"<b><color=#FF8C00>Errors: {errors}</color></b>";
        }
    }

    void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void BackToMainMenu()
    {
        SceneManager.LoadScene("PathTrailMenu");
    }

    void SaveAttempt()
    {
        if (ProfileManager.Instance == null) return;

        var profile = ProfileManager.Instance.currentProfile;
        if (profile == null) return;

        PlayerProfile.PathTrailAttemptData newAttempt =
            new PlayerProfile.PathTrailAttemptData(
                timer,
                errors,
                profile.playerName
            );

        profile.pathTrailAttempts.Add(newAttempt);

        ProfileManager.Instance.SaveProfiles();
    }
}