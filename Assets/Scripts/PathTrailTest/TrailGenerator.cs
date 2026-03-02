using System.Collections.Generic;
using UnityEngine;

public class TrailGenerator : MonoBehaviour
{
    [Header("References")]
    public GameObject nodePrefab;
    public RectTransform nodeContainer;

    [Header("Settings")]
    public int pairs = 5;                   // number of number/letter pairs
    public float minDistance = 200f;        // minimum distance between nodes
    public float maxDistance = 250f;        // maximum distance between consecutive nodes
    public int maxAttempts = 1000;           // max retries per node
    public float screenMargin = 80f;        // canvas margin
    public float angleStep = 15f;           // degrees to increment if placement fails

    [HideInInspector]
    public List<PathNode> spawnedNodes = new List<PathNode>();
    [HideInInspector]
    public List<string> correctSequence = new List<string>();
    [HideInInspector]
    public float nodeRadius;

    void Start()
    {
        GenerateSequence();
        SpawnCorrectPath();
    }

    void GenerateSequence()
    {
        correctSequence.Clear();
        for (int i = 1; i <= pairs; i++)
        {
            correctSequence.Add(i.ToString());
            correctSequence.Add(((char)(64 + i)).ToString());
        }
    }

    void SpawnCorrectPath()
    {
        spawnedNodes.Clear();

        RectTransform canvasRect = nodeContainer.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        float width = canvasRect.rect.width;
        float height = canvasRect.rect.height;

        RectTransform prefabRect = nodePrefab.GetComponent<RectTransform>();
        nodeRadius = prefabRect.rect.width / 2f; // 26.5 for 53x53 prefab

        float xMin = -width / 2 + screenMargin;
        float xMax = width / 2 - screenMargin;
        float yMin = -height / 2 + screenMargin;
        float yMax = height / 2 - screenMargin;

        List<Vector2> positions = new List<Vector2>();

        float minNodeSpacing = nodeRadius * 3f; // ~80 px
        float safeRadius = nodeRadius * 1.2f;   // ~32 px, path won't pass through nodes

        for (int i = 0; i < correctSequence.Count; i++)
        {
            Vector2 spawnPos = Vector2.zero;
            bool valid = false;
            int attempts = 0;
            float angle = Random.Range(0f, 360f);

            while (!valid && attempts < maxAttempts)
            {
                if (i == 0)
                {
                    spawnPos = new Vector2(Random.Range(xMin, xMax), Random.Range(yMin, yMax));
                }
                else
                {
                    float distance = (minDistance + maxDistance) / 2f; // fixed spacing
                    Vector2 prev = positions[i - 1];
                    spawnPos = prev + new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * distance;
                    spawnPos.x = Mathf.Clamp(spawnPos.x, xMin, xMax);
                    spawnPos.y = Mathf.Clamp(spawnPos.y, yMin, yMax);
                }

                valid = true;

                // 1. Ensure node is not too close to any other node
                foreach (var pos in positions)
                {
                    if (Vector2.Distance(pos, spawnPos) < minNodeSpacing)
                    {
                        valid = false;
                        break;
                    }
                }

                // 2. Check path intersections
                if (valid && i > 0)
                {
                    Vector2 newStart = positions[i - 1];
                    Vector2 newEnd = spawnPos;

                    for (int j = 0; j < positions.Count - 1; j++)
                    {
                        Vector2 segStart = positions[j];
                        Vector2 segEnd = positions[j + 1];

                        if (segStart == newStart || segEnd == newStart) continue;

                        if (LinesIntersect(segStart, segEnd, newStart, newEnd))
                        {
                            valid = false;
                            break;
                        }
                    }
                }

                // 3. Check safe distance from all nodes (line-circle check)
                if (valid && i > 0)
                {
                    Vector2 lineStart = positions[i - 1];
                    Vector2 lineEnd = spawnPos;

                    foreach (var nodePos in positions)
                    {
                        if (nodePos == lineStart) continue; // skip start node
                        if (DistancePointToLineSegment(lineStart, lineEnd, nodePos) < safeRadius)
                        {
                            valid = false;
                            break;
                        }
                    }
                }

                if (!valid)
                    angle += angleStep;

                attempts++;
            }

            if (!valid)
                Debug.LogWarning("Could not place node " + correctSequence[i] + " safely.");

            positions.Add(spawnPos);

            // Instantiate node
            GameObject obj = Instantiate(nodePrefab, nodeContainer);
            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.anchoredPosition = spawnPos;

            PathNode node = obj.GetComponent<PathNode>();
            node.Setup(correctSequence[i]);
            spawnedNodes.Add(node);
        }
    }

    // Helper function: distance from point P to line segment AB
    float DistancePointToLineSegment(Vector2 A, Vector2 B, Vector2 P)
    {
        Vector2 AP = P - A;
        Vector2 AB = B - A;
        float ab2 = AB.sqrMagnitude;
        float ap_ab = Vector2.Dot(AP, AB);
        float t = Mathf.Clamp01(ap_ab / ab2);
        Vector2 closest = A + AB * t;
        return Vector2.Distance(P, closest);
    }

    // Line intersection helpers
    bool LinesIntersect(Vector2 A, Vector2 B, Vector2 C, Vector2 D)
    {
        return (CCW(A, C, D) != CCW(B, C, D)) && (CCW(A, B, C) != CCW(A, B, D));
    }

    bool CCW(Vector2 A, Vector2 B, Vector2 C)
    {
        return (C.y - A.y) * (B.x - A.x) > (B.y - A.y) * (C.x - A.x);
    }
}