using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [Header("References")]
    public HintManager hintManager;

    [Header("Prefabs")]
    public MazeNode nodePrefab;
    public GameObject playerPrefab;
    public GameObject goalPrefab;

    [Header("Settings")]
    public float playerSpawnHeight = 1f;

    private GameObject currentPlayer;
    private GameObject currentGoal;
    private List<MazeNode> nodes = new List<MazeNode>();

    // Expose current player for LevelManager if needed
    public GameObject CurrentPlayer => currentPlayer;

    // The MazeNode that represents the goal
    public MazeNode CurrentGoalNode { get; private set; }

    public void GenerateNewMaze(Vector2Int size, LevelManager levelManager)
    {
        // Destroy old maze
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        if (currentPlayer != null) Destroy(currentPlayer);
        if (currentGoal != null) Destroy(currentGoal);

        nodes.Clear();
        CurrentGoalNode = null;

        // Create nodes
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3 nodePos = new Vector3(x - (size.x / 2f), 0, y - (size.y / 2f));
                MazeNode newNode = Instantiate(nodePrefab, nodePos, Quaternion.identity, transform);
                nodes.Add(newNode);
            }
        }

        // Generate maze paths
        GenerateMazePaths(size);

        // Spawn player at top-left
        MazeNode startNode = nodes[0];
        Vector3 spawnPos = startNode.transform.position;
        spawnPos.y = playerSpawnHeight;
        currentPlayer = Instantiate(playerPrefab, spawnPos, Quaternion.identity);

        // Spawn goal at bottom-right
        MazeNode goalNode = nodes[nodes.Count - 1];
        Vector3 goalPos = goalNode.transform.position;
        goalPos.y = 0.5f;
        currentGoal = Instantiate(goalPrefab, goalPos, Quaternion.identity);

        // Assign goal node for hints
        CurrentGoalNode = goalNode;

        // Assign LevelManager to Goal script
        Goal goalScript = currentGoal.GetComponent<Goal>();
        if (goalScript != null)
            goalScript.SetLevelManager(levelManager);

        // Setup HintManager
        if (hintManager != null)
        {
            hintManager.SetMazeGenerator(this);
            hintManager.SetPlayer(currentPlayer);
            
        }

        // Debug
        Debug.Log("Start Node: " + startNode.name);
        Debug.Log("Goal Node: " + CurrentGoalNode.name);
    }

    // Return closest node to a world position
    public MazeNode GetNodeAtPosition(Vector3 position)
    {
        float minDist = float.MaxValue;
        MazeNode closestNode = null;

        Vector3 pos2D = new Vector3(position.x, 0, position.z);

        foreach (MazeNode node in nodes)
        {
            Vector3 nodePos2D = new Vector3(node.transform.position.x, 0, node.transform.position.z);
            float dist = Vector3.Distance(pos2D, nodePos2D);
            if (dist < minDist)
            {
                minDist = dist;
                closestNode = node;
            }
        }

        return closestNode;
    }

    // Return neighbors without walls blocking
    public List<MazeNode> GetNeighbors(MazeNode node)
    {
        List<MazeNode> neighbors = new List<MazeNode>();

        foreach (MazeNode other in nodes)
        {
            if (other == node) continue;

            Vector3 diff = other.transform.position - node.transform.position;

            if (Mathf.Abs(diff.x - 1) < 0.1f && Mathf.Abs(diff.z) < 0.1f && !node.IsWallActive(0) && !other.IsWallActive(1))
                neighbors.Add(other);
            if (Mathf.Abs(diff.x + 1) < 0.1f && Mathf.Abs(diff.z) < 0.1f && !node.IsWallActive(1) && !other.IsWallActive(0))
                neighbors.Add(other);
            if (Mathf.Abs(diff.z - 1) < 0.1f && Mathf.Abs(diff.x) < 0.1f && !node.IsWallActive(2) && !other.IsWallActive(3))
                neighbors.Add(other);
            if (Mathf.Abs(diff.z + 1) < 0.1f && Mathf.Abs(diff.x) < 0.1f && !node.IsWallActive(3) && !other.IsWallActive(2))
                neighbors.Add(other);
        }

        return neighbors;
    }

    // DFS maze generation
    void GenerateMazePaths(Vector2Int size)
    {
        List<MazeNode> currentPath = new List<MazeNode>();
        List<MazeNode> completedNodes = new List<MazeNode>();

        currentPath.Add(nodes[0]);
        currentPath[0].SetState(NodeState.Current);

        while (completedNodes.Count < nodes.Count)
        {
            List<int> possibleNextNodes = new List<int>();
            List<int> possibleDirections = new List<int>();

            int currentIndex = nodes.IndexOf(currentPath[currentPath.Count - 1]);
            int currentX = currentIndex / size.y;
            int currentY = currentIndex % size.y;

            if (currentX < size.x - 1 && !completedNodes.Contains(nodes[currentIndex + size.y]) && !currentPath.Contains(nodes[currentIndex + size.y]))
            { possibleNextNodes.Add(currentIndex + size.y); possibleDirections.Add(1); }
            if (currentX > 0 && !completedNodes.Contains(nodes[currentIndex - size.y]) && !currentPath.Contains(nodes[currentIndex - size.y]))
            { possibleNextNodes.Add(currentIndex - size.y); possibleDirections.Add(2); }
            if (currentY < size.y - 1 && !completedNodes.Contains(nodes[currentIndex + 1]) && !currentPath.Contains(nodes[currentIndex + 1]))
            { possibleNextNodes.Add(currentIndex + 1); possibleDirections.Add(3); }
            if (currentY > 0 && !completedNodes.Contains(nodes[currentIndex - 1]) && !currentPath.Contains(nodes[currentIndex - 1]))
            { possibleNextNodes.Add(currentIndex - 1); possibleDirections.Add(4); }

            if (possibleNextNodes.Count > 0)
            {
                int chosen = Random.Range(0, possibleNextNodes.Count);
                MazeNode nextNode = nodes[possibleNextNodes[chosen]];

                switch (possibleDirections[chosen])
                {
                    case 1: nextNode.RemoveWall(1); currentPath[currentPath.Count - 1].RemoveWall(0); break;
                    case 2: nextNode.RemoveWall(0); currentPath[currentPath.Count - 1].RemoveWall(1); break;
                    case 3: nextNode.RemoveWall(3); currentPath[currentPath.Count - 1].RemoveWall(2); break;
                    case 4: nextNode.RemoveWall(2); currentPath[currentPath.Count - 1].RemoveWall(3); break;
                }

                currentPath.Add(nextNode);
                nextNode.SetState(NodeState.Current);
            }
            else
            {
                MazeNode finishedNode = currentPath[currentPath.Count - 1];
                finishedNode.SetState(NodeState.Completed);
                completedNodes.Add(finishedNode);
                currentPath.RemoveAt(currentPath.Count - 1);
            }
        }
    }
}
