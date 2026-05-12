[System.Serializable]
public class QuizAttemptData
{
    public int totalQuestions;
    public int correctAnswers;
    public string dateTime;  
    public int themeID;       

    public QuizAttemptData(int correct, int total, int theme)
    {
        correctAnswers = correct;
        totalQuestions = total;
        themeID = theme;
        dateTime = System.DateTime.Now.ToString("dd MMM yyyy HH:mm");
    }
}