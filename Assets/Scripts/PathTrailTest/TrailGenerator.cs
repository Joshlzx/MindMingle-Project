using System.Collections.Generic;
using UnityEngine;

public class TrailGenerator : MonoBehaviour
{
    [Header("References")]
    public GameObject nodePrefab;
    public RectTransform nodeContainer;

    [Header("Settings")]
    public int pairs = 5;
    public float minDistance = 200f;
    public float maxDistance = 250f;
    public int maxAttempts = 1000;
    public float screenMargin = 80f;
    public float angleStep = 15f;

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
        nodeRadius = prefabRect.rect.width / 2f;

        float containerScale = nodeContainer.lossyScale.x;
        float nodeDiameterScaled = prefabRect.rect.width * containerScale;
        int totalNodes = correctSequence.Count;

        // usable area inside margins (before you compute xMin/xMax)
        float usableWidth = Mathf.Max(1f, width - 2f * screenMargin - nodeDiameterScaled);
        float usableHeight = Mathf.Max(1f, height - 2f * screenMargin - nodeDiameterScaled);
        float usableArea = usableWidth * usableHeight;
        float cellSize = Mathf.Sqrt(usableArea / Mathf.Max(1, totalNodes));

        // compute recommended distances
        float padding = 8f * containerScale;
        float minBasedOnNode = nodeDiameterScaled + padding;
        float suggestedMin = Mathf.Max(minBasedOnNode, 0.8f * cellSize);
        float suggestedMax = Mathf.Max(suggestedMin + 20f, 1.2f * cellSize);

        // clamp attempts
        int suggestedAttempts = Mathf.Clamp(totalNodes * 500, 1000, 10000);

        // If inspector settings are smaller than suggested, override (log so you can see)
        if (minDistance < suggestedMin * 0.9f)
        {
            Debug.Log($"TrailGenerator: overriding minDistance {minDistance} -> {suggestedMin:F0}");
            minDistance = suggestedMin;
        }
        if (maxDistance < minDistance + 1f)
        {
            Debug.Log($"TrailGenerator: overriding maxDistance {maxDistance} -> {suggestedMax:F0}");
            maxDistance = suggestedMax;
        }
        if (maxAttempts < 100)
        {
            Debug.Log($"TrailGenerator: adjusting maxAttempts {maxAttempts} -> {suggestedAttempts}");
            maxAttempts = suggestedAttempts;
        }

        // set conservative runtime spacing (use this when checking collisions)
        float runtimeMinNodeSpacing = Mathf.Max(nodeDiameterScaled + padding, nodeRadius * 2f + 8f);

        float xMin = -width / 2 + screenMargin;
        float xMax = width / 2 - screenMargin;
        float yMin = -height / 2 + screenMargin;
        float yMax = height / 2 - screenMargin;

        List<Vector2> positions = new List<Vector2>();

        float minNodeSpacing = nodeRadius * 3f;
        float safeRadius = nodeRadius * 1.2f;

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
                    float distance = (minDistance + maxDistance) / 2f;
                    Vector2 prev = positions[i - 1];
                    spawnPos = prev + new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * distance;
                    spawnPos.x = Mathf.Clamp(spawnPos.x, xMin, xMax);
                    spawnPos.y = Mathf.Clamp(spawnPos.y, yMin, yMax);
                }

                valid = true;


                foreach (var pos in positions)
                {
                    if (Vector2.Distance(pos, spawnPos) < minNodeSpacing)
                    {
                        valid = false;
                        break;
                    }
                }


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


                if (valid && i > 0)
                {
                    Vector2 lineStart = positions[i - 1];
                    Vector2 lineEnd = spawnPos;

                    foreach (var nodePos in positions)
                    {
                        if (nodePos == lineStart) continue;
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


            GameObject obj = Instantiate(nodePrefab, nodeContainer);
            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.anchoredPosition = spawnPos;

            PathNode node = obj.GetComponent<PathNode>();
            node.Setup(correctSequence[i]);
            spawnedNodes.Add(node);
        }
    }


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


    bool LinesIntersect(Vector2 A, Vector2 B, Vector2 C, Vector2 D)
    {
        return (CCW(A, C, D) != CCW(B, C, D)) && (CCW(A, B, C) != CCW(A, B, D));
    }

    bool CCW(Vector2 A, Vector2 B, Vector2 C)
    {
        return (C.y - A.y) * (B.x - A.x) > (B.y - A.y) * (C.x - A.x);
    }
}
