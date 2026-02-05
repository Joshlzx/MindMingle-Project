using UnityEngine;

public class Goal : MonoBehaviour
{
    LevelManager levelManager;
    bool completed = false;

    public void SetLevelManager(LevelManager manager)
    {
        levelManager = manager;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (completed) return;

        if (other.CompareTag("Player"))
        {
            completed = true;
            levelManager.OnLevelComplete();
        }
    }
}
