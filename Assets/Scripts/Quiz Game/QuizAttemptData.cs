[System.Serializable]
public class QuizAttemptData
{
    public int totalQuestions;
    public int correctAnswers;
    public string dateTime;   // "dd MMM yyyy HH:mm"
    public int themeID;       // New: 0, 1, 2 for your 3 themes

    public QuizAttemptData(int correct, int total, int theme)
    {
        correctAnswers = correct;
        totalQuestions = total;
        themeID = theme;
        dateTime = System.DateTime.Now.ToString("dd MMM yyyy HH:mm");
    }
}