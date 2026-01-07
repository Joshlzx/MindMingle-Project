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
        TotalQuestions = QnA.Count;
        GoPanel.SetActive(false);
        generateQuestion();
    }

    public void retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    IEnumerator WaitForNext()
    {
        yield return new WaitForSeconds(1);
        generateQuestion();
    }
    
    public void correct()
    {
        //when you are right 
        score += 1;
        QnA.RemoveAt(currentQuestion);
        //generateQuestion();
        StartCoroutine(WaitForNext());


    }

    public void wrong()
    {
        // When you answer wrong 
        QnA.RemoveAt(currentQuestion);
        //generateQuestion();
        StartCoroutine(WaitForNext());

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

    void generateQuestion() 
    {
        if (QnA.Count > 0) 
        {
            currentQuestion = Random.Range(0, QnA.Count);

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
