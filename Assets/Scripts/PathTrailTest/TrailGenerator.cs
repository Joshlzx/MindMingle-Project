using System.Collections.Generic;
using UnityEngine;

public class TrailGenerator : MonoBehaviour
{
    [Header("References")]
        public GameObject nodePrefab;
        public RectTransform nodeContainer;

    [Header("Settings")]
    public int pairs = 5;                   // number of number/letter pairs
    public float minDistance = 150f;        // minimum distance between nodes
    public float maxDistance = 250f;        // maximum distance between consecutive nodes
    public int maxAttempts = 200;           // retries per node
    public float screenMargin = 80f;        // margin from canvas edges

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

        // Node size
        RectTransform prefabRect = nodePrefab.GetComponent<RectTransform>();
        nodeRadius = prefabRect.rect.width / 2f;

        // Boundaries
        float xMin = -width / 2 + screenMargin;
        float xMax = width / 2 - screenMargin;
        float yMin = -height / 2 + screenMargin;
        float yMax = height / 2 - screenMargin;

        List<Vector2> correctPositions = new List<Vector2>();

        for (int i = 0; i < correctSequence.Count; i++)
        {
            Vector2 spawnPos = Vector2.zero;
            bool valid = false;
            int attempts = 0;

            while (!valid && attempts < maxAttempts)
            {
                if (i == 0)
                {
                    // First node randomly inside canvas
                    spawnPos = new Vector2(Random.Range(xMin, xMax), Random.Range(yMin, yMax));
                }
                else
                {
                    // Random angle & distance from previous node
                    float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                    float distance = Random.Range(minDistance, maxDistance);
                    spawnPos = correctPositions[i - 1] + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;

                    // Clamp within canvas bounds
                    spawnPos.x = Mathf.Clamp(spawnPos.x, xMin, xMax);
                    spawnPos.y = Mathf.Clamp(spawnPos.y, yMin, yMax);
                }

                // Check overlap with previous nodes
                bool tooClose = false;
                foreach (var pos in correctPositions)
                {
                    if (Vector2.Distance(pos, spawnPos) < minDistance)
                    {
                        tooClose = true;
                        break;
                    }
                }

                // Check line intersections with existing path segments
                bool intersects = false;
                if (i > 1)
                {
                    Vector2 prev = correctPositions[i - 1];
                    for (int j = 0; j < correctPositions.Count - 2; j++)
                    {
                        Vector2 A = correctPositions[j];
                        Vector2 B = correctPositions[j + 1];
                        if (LinesIntersect(A, B, prev, spawnPos))
                        {
                            intersects = true;
                            break;
                        }
                    }
                }

                if (!tooClose && !intersects)
                    valid = true;

                attempts++;
            }

            if (!valid)
                Debug.LogWarning("Could not place node without overlap/intersection. Consider increasing canvas or reducing pairs.");

            correctPositions.Add(spawnPos);

            // Instantiate node
            GameObject obj = Instantiate(nodePrefab, nodeContainer);
            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.anchoredPosition = spawnPos;

            PathNode node = obj.GetComponent<PathNode>();
            node.Setup(correctSequence[i]);
            spawnedNodes.Add(node);
        }
    }

    // Helper: line intersection check
    bool LinesIntersect(Vector2 A, Vector2 B, Vector2 C, Vector2 D)
    {
        return (CCW(A, C, D) != CCW(B, C, D)) &&
               (CCW(A, B, C) != CCW(A, B, D));
    }

    bool CCW(Vector2 A, Vector2 B, Vector2 C)
    {
        return (C.y - A.y) * (B.x - A.x) > (B.y - A.y) * (C.x - A.x);
    }
}