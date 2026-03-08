using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class QuizManager : MonoBehaviour
{
    [Header("Quiz Data")]
    public List<QuestionsAndAnswers> QnA;
    public int currentQuestion;
    public int currentThemeID;

    [Header("UI")]
    public GameObject Quizpanel;
    public GameObject GoPanel;
    public TextMeshProUGUI QuestionTxt;
    public TextMeshProUGUI ScoreTxt;
    public GameObject[] options;

    [Header("Score")]
    public int score;

    int TotalQuestions = 0;
    bool canSelect = true;

    void Start()
    {
        // Copy list so inspector data isn't modified
        QnA = new List<QuestionsAndAnswers>(QnA);

        // Random seed
        Random.InitState(System.DateTime.Now.Millisecond);

        TotalQuestions = QnA.Count;

        GoPanel.SetActive(false);

        ShuffleQuestions();
        currentQuestion = 0;

        generateQuestion();
    }

    void Update()
    {
        if (!Quizpanel.activeSelf || !canSelect) return;

        if (Input.GetKeyDown(KeyCode.Z)) SelectAnswer(0);
        if (Input.GetKeyDown(KeyCode.X)) SelectAnswer(1);
        if (Input.GetKeyDown(KeyCode.C)) SelectAnswer(2);
        if (Input.GetKeyDown(KeyCode.V)) SelectAnswer(3);
    }

    void SelectAnswer(int optionIndex)
    {
        if (!canSelect) return;

        canSelect = false;

        AnswerScript ans = options[optionIndex].GetComponent<AnswerScript>();

        if (ans.isCorrect)
        {
            options[optionIndex].GetComponent<Image>().color = Color.green;
            correct();
        }
        else
        {
            options[optionIndex].GetComponent<Image>().color = Color.red;
            wrong();
        }

        StartCoroutine(ReEnableInput());
    }

    IEnumerator ReEnableInput()
    {
        yield return new WaitForSeconds(3f);
        canSelect = true;
    }

    public void correct()
    {
        score++;
        currentQuestion++;

        StartCoroutine(WaitForNext());
    }

    public void wrong()
    {
        ShowCorrectAnswer();
        currentQuestion++;

        StartCoroutine(WaitForNext());
    }

    IEnumerator WaitForNext()
    {
        yield return new WaitForSeconds(3f);
        generateQuestion();
    }

    void generateQuestion()
    {
        if (currentQuestion < QnA.Count)
        {
            QuestionTxt.text = QnA[currentQuestion].Question;
            SetAnswers();
        }
        else
        {
            Debug.Log("Out of Questions");
            GameOver();
        }
    }

    void SetAnswers()
    {
        for (int i = 0; i < options.Length; i++)
        {
            Image img = options[i].GetComponent<Image>();
            AnswerScript ans = options[i].GetComponent<AnswerScript>();

            img.color = ans.startColor;
            ans.isCorrect = false;

            options[i].transform.GetChild(0).GetComponent<Image>().sprite =
                QnA[currentQuestion].Answers[i];

            if (QnA[currentQuestion].CorrectAnswer == i + 1)
            {
                ans.isCorrect = true;
            }
        }
    }

    void ShowCorrectAnswer()
    {
        for (int i = 0; i < options.Length; i++)
        {
            AnswerScript ans = options[i].GetComponent<AnswerScript>();

            if (ans.isCorrect)
            {
                options[i].GetComponent<Image>().color = Color.green;
            }
        }
    }

    void ShuffleQuestions()
    {
        for (int i = 0; i < QnA.Count; i++)
        {
            int randomIndex = Random.Range(i, QnA.Count);

            QuestionsAndAnswers temp = QnA[i];
            QnA[i] = QnA[randomIndex];
            QnA[randomIndex] = temp;
        }
    }

    public void GameOver()
    {
        Quizpanel.SetActive(false);
        GoPanel.SetActive(true);

        ScoreTxt.text = score + "/" + TotalQuestions;
        Debug.Log("Saving attempt | Score: " + score + "/" + TotalQuestions + " | ThemeID: " + currentThemeID);

        // Save attempt
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.SaveQuizAttempt(score, TotalQuestions, currentThemeID);
        }
        else
        {
            Debug.LogWarning("ScoreManager not found. Attempt not saved.");
        }
    }

    public void retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}