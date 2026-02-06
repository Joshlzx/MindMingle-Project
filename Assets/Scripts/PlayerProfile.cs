using UnityEngine.UIElements;

[System.Serializable]
public class PlayerProfile
{
    public string playerName;

    // QUIZ STATS
    public int totalQuizQuestions;
    public int totalQuizCorrect;

    public PlayerProfile(string name)
    {
        playerName = name;
        totalQuizQuestions = 0;
        totalQuizCorrect = 0;
    }
}


/* Next coding idea:

- Make a scoreboard page that tracks all their latest progress. Then in that page , have a button that they can press that leads them to a highscore board that shows everyone's highest attempt.
Do this for all the game. 

- For each game , store and display:
Rhythm game - total score, hit percentage and rank
Simon game - the furthest level they reached
Maze game - Furthest  level they reach , and how many times they press hint 
Quiz game - Total number of question and total correct answer

* if you can , add timing and date to each entry too

Current progress: Managed to create ScoreManager that stores Quiz Game's total question and total correct answer */