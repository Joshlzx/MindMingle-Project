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

    private Color originalColor;      
    private Color availableColor = Color.red;
    private Color currentColor = Color.purple;    
    private Color completedColor = Color.white;
    private Color hintColor = Color.yellow; 

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

        originalColor = floor.material.color; 
    }

    
    public void Highlight(bool on)
    {
        if (floor == null) return;

        if (on)
            floor.material.color = hintColor;
        else
            floor.material.color = originalColor; 
    }

    
    public bool IsWallActive(int wallIndex)
    {
        if (wallIndex >= 0 && wallIndex < walls.Length)
            return walls[wallIndex].activeSelf;
        return false;
    }

    
    public GameObject GetWallObject(int index)
    {
        if (walls == null) return null;
        if (index >= 0 && index < walls.Length)
            return walls[index];
        return null;
    }

  
    public void DebugLogWalls()
    {
        if (walls == null)
        {
            Debug.Log($"{name}: walls array is null");
            return;
        }
        for (int i = 0; i < walls.Length; i++)
        {
            var w = walls[i];
            Debug.Log($"{name}: wall[{i}] = {(w!=null?w.name:"null")}, active={(w!=null?w.activeSelf.ToString():"n/a")}");
        }
    }
}
