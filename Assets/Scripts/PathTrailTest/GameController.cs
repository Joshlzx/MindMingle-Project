using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class GameController : MonoBehaviour
{
    [Header("References")]
    public TrailGenerator generator;
    public UILineRenderer uiLineRenderer;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI errorText;

    [Header("Result Screen")]
    public PathTrailResultScreen resultScreen; // drag the panel object here

    private int currentIndex = 0;
    private int errors = 0;
    private float timer = 0f;
    private bool gameActive = true;
    private PathNode currentNode = null;

    private List<PathNode> selectedNodes = new List<PathNode>();
    private List<PathNode> currentOptions = new List<PathNode>();
    private List<PathNode> stepWrongNodes = new List<PathNode>();

    private int maxErrorsPerNode = 3; // max errors per node

    private float inputCooldown = 0.6f; 
    private float lastInputTime = 0f;

    void Start()
    {
        // Show timer and error UI
        if (timerText != null) timerText.gameObject.SetActive(true);
        if (errorText != null) errorText.gameObject.SetActive(true);

        // Initialize nodes
        if (generator.spawnedNodes.Count == 0)
            StartCoroutine(InitNextFrame());
        else
            GenerateOptions();
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

        if (Time.time - lastInputTime < inputCooldown) return;

        if (Input.GetKeyDown(KeyCode.Z)) { SelectOption(0); lastInputTime = Time.time; }
        if (Input.GetKeyDown(KeyCode.X)) { SelectOption(1); lastInputTime = Time.time; }
        if (Input.GetKeyDown(KeyCode.C)) { SelectOption(2); lastInputTime = Time.time; }
        if (Input.GetKeyDown(KeyCode.V)) { SelectOption(3); lastInputTime = Time.time; }
    }

    void GenerateOptions()
    {
        currentOptions.Clear();
        string correctValue = generator.correctSequence[currentIndex];
        PathNode correctNode = generator.spawnedNodes.Find(n => n.nodeValue == correctValue);

        currentOptions.Add(correctNode);

        while (currentOptions.Count < 4)
        {
            PathNode randomNode = generator.spawnedNodes[Random.Range(0, generator.spawnedNodes.Count)];
            if (!currentOptions.Contains(randomNode) && randomNode != currentNode)
                currentOptions.Add(randomNode);
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
            if (currentNode != null)
            {
                currentNode.StopCurrentHighlight();
                currentNode.Highlight(Color.green);
            }

            foreach (var wrongNode in stepWrongNodes)
                wrongNode.ResetIndicators();
            stepWrongNodes.Clear();

            currentNode = selected;
            currentNode.StartCurrentHighlight();

            selectedNodes.Add(selected);
            DrawLine();

            currentIndex++;

            if (currentIndex >= generator.correctSequence.Count)
            {
                gameActive = false;
                if (currentNode != null)
                {
                    currentNode.StopCurrentHighlight();
                    currentNode.Highlight(Color.green);
                }
                OnGameFinished();
                return;
            }

            GenerateOptions();
        }
        else
        {
            if (!stepWrongNodes.Contains(selected))
            {
                errors++;
                selected.Highlight(Color.red);
                stepWrongNodes.Add(selected);
            }
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
        int totalNodes = generator.correctSequence.Count;
        int maxErrors = totalNodes * maxErrorsPerNode;
        string grade = GetGrade(errors, maxErrors);

        SaveAttempt(errors, maxErrors, grade);

        // Hide game UI
        timerText.gameObject.SetActive(false);
        errorText.gameObject.SetActive(false);
        uiLineRenderer.gameObject.SetActive(false);
        foreach (var node in generator.spawnedNodes)
            node.gameObject.SetActive(false);

        // Show result panel last
        if (resultScreen != null)
            resultScreen.ShowResult(timer, errors, maxErrors, grade);
        else
            Debug.LogError("ResultScreen reference is missing!");
    }

    string GetGrade(int errorsMade, int maxErrors)
    {
        float percent = 100f * (1f - (float)errorsMade / maxErrors);

        if (percent >= 90f) return "Excellent";  //Grades based on percentage accuracy
        if (percent >= 75f) return "Good";
        if (percent >= 50f) return "Average";
        return "Poor";
    }

    void SaveAttempt(int errorsMade, int maxErrors, string grade)
    {
        var profile = ProfileManager.Instance?.currentProfile;
        if (profile == null) return;

        var attempt = new PlayerProfile.PathTrailAttemptData(
            timer,
            errorsMade,
            profile.playerName,
            maxErrors,
            grade
        );

        profile.pathTrailAttempts.Add(attempt);
        ProfileManager.Instance.SaveProfiles();
    }
}