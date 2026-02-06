using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void LoadQuizGame()
    {
        SceneManager.LoadScene("Quiz game");
    }

    public void LoadRhythmGame()
    {
        SceneManager.LoadScene("Rhythm game");
    }

    public void LoadSimonGame()
    {
        SceneManager.LoadScene("Simon game");
    }

    public void LoadMazeGame()
    {
        SceneManager.LoadScene("Maze game");
    }
    public void BackToProfileSelection()
    {
        SceneManager.LoadScene("ProfileSelectionScene");
    }
}
