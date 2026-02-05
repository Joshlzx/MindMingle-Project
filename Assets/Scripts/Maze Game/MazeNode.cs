using UnityEngine;

public enum NodeState
{
    Available,
    Current,
    Completed
}

public class MazeNode : MonoBehaviour
{
    [Header("Node Components")]
    [SerializeField] private GameObject[] walls;
    [SerializeField] private MeshRenderer floor;

    private Color originalColor;      // stores default color of this node
    private Color availableColor = Color.red;
    private Color currentColor = Color.purple;    // no yellow spam
    private Color completedColor = Color.white;
    private Color hintColor = Color.yellow; // color for hint path (eye-friendly)

    private void Awake()
    {
        if (floor != null)
        {
            floor.material = new Material(floor.sharedMaterial);
            originalColor = Color.white;
            floor.material.color = originalColor;
        }
    }

   
    public void RemoveWall(int wallIndex)
    {
        if (wallIndex >= 0 && wallIndex < walls.Length)
            walls[wallIndex].SetActive(false);
    }

    public void SetState(NodeState state)
    {
        if (floor == null) return;

        switch (state)
        {
            case NodeState.Available:
                floor.material.color = availableColor;
                break;
            case NodeState.Current:
                floor.material.color = currentColor;
                break;
            case NodeState.Completed:
                floor.material.color = completedColor;
                break;
        }

        originalColor = floor.material.color; // update original color for hint reset
    }

    /// <summary>Highlights this node for hints.</summary>
    public void Highlight(bool on)
    {
        if (floor == null) return;

        if (on)
            floor.material.color = hintColor;
        else
            floor.material.color = originalColor; // reset to the state color (Available/Completed/Current)
    }

    /// <summary>Checks if a wall is active.</summary>
    public bool IsWallActive(int wallIndex)
    {
        if (wallIndex >= 0 && wallIndex < walls.Length)
            return walls[wallIndex].activeSelf;
        return false;
    }
}
