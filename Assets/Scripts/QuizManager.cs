using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    public List<QuestionsAndAnswers> QnA;
    public GameObject[] options;
    public int currentQuestion;

    public GameObject Quizpanel;
    public GameObject GoPanel;

    public TMPro.TextMeshProUGUI QuestionTxt;
    public TMPro.TextMeshProUGUI ScoreTxt;

    int TotalQuestions = 0;
    public int score;


    private void Start()
    {

        // break reference to Inspector list
        QnA = new List<QuestionsAndAnswers>(QnA);

        //Random seed every retry
        Random.InitState(System.DateTime.Now.Millisecond); // Ensure the questions are random

        TotalQuestions = QnA.Count;
        GoPanel.SetActive(false);
        ShuffleQuestions();
        currentQuestion = 0;  // set currentQuestion to 0 
        generateQuestion();
    }

    public void retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    IEnumerator WaitForNext()
    {
        yield return new WaitForSeconds(3);
        generateQuestion();
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
    }
    void SetAnswers()
    {
        for (int i = 0; i < options.Length; i++)
        {
            options[i].GetComponent<Image>().color = options[i].GetComponent<AnswerScript>().startColor;
            options[i].GetComponent<AnswerScript>().isCorrect = false;
            options[i].transform.GetChild(0).GetComponent<Image>().sprite = QnA[currentQuestion].Answers[i];

            


            if (QnA[currentQuestion].CorrectAnswer == i+1)
            {
                
                options[i].GetComponent<AnswerScript>().isCorrect = true;
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




}
