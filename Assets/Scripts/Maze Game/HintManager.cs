using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintManager : MonoBehaviour
{
    [Header("Hint Settings")]
    [SerializeField] private float stepDelay = 0.3f;
    [SerializeField] private float visibleTime = 1f;

    [Header("UI")]
    public Button hintButton;  // assign in inspector

    private MazeGenerator mazeGenerator;
    private GameObject playerObject;

    private List<MazeNode> currentHintPath = new List<MazeNode>();

    // Track hints used
    [HideInInspector] public int hintsUsedThisLevel = 0;

    public void SetMazeGenerator(MazeGenerator generator)
    {
        mazeGenerator = generator;
    }

    public void SetPlayer(GameObject player)
    {
        playerObject = player;
    }

    public void ShowHint()
    {
        if (mazeGenerator == null || playerObject == null || mazeGenerator.CurrentGoalNode == null) return;

        // Disable hint button while path is active
        if (hintButton != null)
            hintButton.interactable = false;

        hintsUsedThisLevel++; // increment hints used
        StartCoroutine(ShowHintCoroutine());
    }

    private IEnumerator ShowHintCoroutine()
    {
        currentHintPath.Clear();

        MazeNode startNode = mazeGenerator.GetNodeAtPosition(playerObject.transform.position);
        MazeNode goalNode = mazeGenerator.CurrentGoalNode;

        if (startNode == null || goalNode == null) yield break;

        // BFS to find shortest path
        Queue<MazeNode> queue = new Queue<MazeNode>();
        Dictionary<MazeNode, MazeNode> cameFrom = new Dictionary<MazeNode, MazeNode>();
        HashSet<MazeNode> visited = new HashSet<MazeNode>();

        queue.Enqueue(startNode);
        visited.Add(startNode);

        while (queue.Count > 0)
        {
            MazeNode current = queue.Dequeue();
            if (current == goalNode) break;

            foreach (MazeNode neighbor in mazeGenerator.GetNeighbors(current))
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                    cameFrom[neighbor] = current;
                }
            }
        }

        // Reconstruct path
        MazeNode temp = goalNode;
        while (temp != startNode)
        {
            currentHintPath.Add(temp);
            if (!cameFrom.ContainsKey(temp)) break;
            temp = cameFrom[temp];
        }
        currentHintPath.Add(startNode);
        currentHintPath.Reverse();

        // Gradually highlight nodes
        foreach (MazeNode node in currentHintPath)
        {
            node.Highlight(true);
            yield return new WaitForSeconds(stepDelay);
        }

        yield return new WaitForSeconds(visibleTime);

        // Gradually reset highlight
        foreach (MazeNode node in currentHintPath)
        {
            node.Highlight(false);
            yield return new WaitForSeconds(stepDelay / 2f);
        }

        // Re-enable hint button
        if (hintButton != null)
            hintButton.interactable = true;
    }
}
