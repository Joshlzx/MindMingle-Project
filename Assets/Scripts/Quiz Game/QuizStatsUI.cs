using TMPro;
using UnityEngine;

public class QuizStatsUI : MonoBehaviour
{
    public TextMeshProUGUI statsText;

    void Start()
    {
        var profile = ProfileManager.Instance.currentProfile;

        if (profile != null)
        {
            statsText.text =
                "Total Correct: " + profile.totalQuizCorrect +
                "\nTotal Questions: " + profile.totalQuizQuestions;
        }
    }
}
