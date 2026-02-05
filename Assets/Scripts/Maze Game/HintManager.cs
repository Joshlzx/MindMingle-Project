using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    [Header("Hint tings")]
    [SerializeField] private float stepDelay = 0.3f;   // time between highlighting nodes
    [SerializeField] private float visibleTime = 1f;   // how long the path stays visible after fully highlighted

    private MazeGenerator mazeGenerator;
    private GameObject playerObject;

    private List<MazeNode> currentHintPath = new List<MazeNode>();

    // Assign the maze generator so we can get nodes
    public void SetMazeGenerator(MazeGenerator generator)
    {
        mazeGenerator = generator;
    }

    // Assign the current player
    public void SetPlayer(GameObject player)
    {
        playerObject = player;
    }

    // Call this when the player presses the hint button
    public void ShowHint()
    {
        if (mazeGenerator == null || playerObject == null || mazeGenerator.CurrentGoalNode == null) return;

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
            if (!cameFrom.ContainsKey(temp)) yield break; // no path found
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

        // Keep path visible for a short while
        yield return new WaitForSeconds(visibleTime);

        // Gradually reset highlight
        foreach (MazeNode node in currentHintPath)
        {
            node.Highlight(false);
            yield return new WaitForSeconds(stepDelay / 2f); // fade back slightly faster
        }
    }
}
