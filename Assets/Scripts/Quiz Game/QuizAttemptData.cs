[System.Serializable]
public class QuizAttemptData
{
    public int totalQuestions;
    public int correctAnswers;
    public string dateTime;   // change from DateTime

    public QuizAttemptData(int correct, int total)
    {
        correctAnswers = correct;
        totalQuestions = total;
        dateTime = System.DateTime.Now.ToString("dd MMM yyyy HH:mm");
    }
}
