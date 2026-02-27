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
    public void LoadQuizScoreboard()
    {
        SceneManager.LoadScene("QuizAttemptHistoryScene");
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void LoadQuizMainMenu()
    {
        SceneManager.LoadScene("QuizMainMenu");
    }

    public void LoadSimonScoreboard()
    {
        SceneManager.LoadScene("SimonScoreboard");
    }
    public void LoadRhythmScoreboard()
    {
        SceneManager.LoadScene("RhythmScoreboard");
    }
    public void LoadRhythmMainMenu()
    {
        SceneManager.LoadScene("RhythmMainMenu");
    }

    public void LoadMazeScoreboard()
    {
        SceneManager.LoadScene("MazeScoreboard");
    }
    public void LoadMazeMainMenu()
    {
        SceneManager.LoadScene("MazeMainMenu");
    }

    public void LoadPathTrailTest()
    {
        SceneManager.LoadScene("PathTrailTest");
    }

    public void PathTrailMenu()
    {
        SceneManager.LoadScene("PathTrailMenu");
    }

    public void LoadPathTrailScoreboard()
    {
        SceneManager.LoadScene("PathTrailScoreboard");
    }
}
