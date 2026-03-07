using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PathTrailResultScreen : MonoBehaviour
{
    [Header("UI References")]
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;
    public Button playAgainButton;
    public Button mainMenuButton;

    private void Awake()
    {
        // Always hide panel at game start
        if (resultPanel != null)
            resultPanel.SetActive(false);
    }

    private void Start()
    {
        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(() =>
                SceneManager.LoadScene(SceneManager.GetActiveScene().name));

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(() =>
                SceneManager.LoadScene("PathTrailMenu"));
    }

    
    public void ShowResult(float completionTime, int errorsMade, int maxErrors, string grade)
    {
        if (resultPanel == null || resultText == null)
        {
            Debug.LogError("Result panel or result text is missing!");
            return;
        }

        string dateTime = System.DateTime.Now.ToString("dd MMM yyyy HH:mm");
        resultText.text =
            $"<color=#000000><b>PathTrailing Result</b></color>\n\n" +
            $"<color=#9C27B0>Time: {completionTime:F1} s</color>\n" +
            $"<color=#FF8C00>Errors made: {errorsMade}/{maxErrors}</color>\n" +
            $"<color=#000000>Grade: {grade}</color>\n";
            

        resultPanel.SetActive(true);
        Debug.Log("Result panel displayed!");
    }
}