using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Goal Reached!");
            GameComplete();
        }
    }

    void GameComplete()
    {
        // Pause game
        Time.timeScale = 0f;


        Debug.Log("GAME COMPLETE!");

        // Later you can:
        // - Show UI
        // - Load next level
        // - Show score
    }
}