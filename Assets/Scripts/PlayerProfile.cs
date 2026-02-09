using System;
using System.Collections.Generic;

[System.Serializable]
public class PlayerProfile
{
    public string playerName;

    // Overall quiz totals
    public int totalQuizQuestions;
    public int totalQuizCorrect;

    // Quiz attempt history
    public List<QuizAttemptData> quizAttempts = new List<QuizAttemptData>();

    // Simon attempt history
    public List<SimonAttemptData> simonAttempts = new List<SimonAttemptData>();

    // Rhythm attempt history
    public List<RhythmAttemptData> rhythmAttempts = new List<RhythmAttemptData>();

    public List<MazeAttemptData> mazeAttempts = new List<MazeAttemptData>();

    public PlayerProfile(string name)
    {
        playerName = name;
        totalQuizQuestions = 0;
        totalQuizCorrect = 0;
    }

    [System.Serializable]
    public class QuizAttemptData
    {
        public string dateTime;
        public int totalQuestions;
        public int correctAnswers;

        public QuizAttemptData(int total, int correct)
        {
            totalQuestions = total;
            correctAnswers = correct;
            dateTime = DateTime.Now.ToString("dd MMM yyyy HH:mm");
        }
    }

    [System.Serializable]
    public class SimonAttemptData
    {
        public int levelReached;
        public int hintsUsed;
        public int progressIntoLevel;
        public string dateTime;

        public SimonAttemptData(int level, int hints, int progress)
        {
            levelReached = level;
            hintsUsed = hints;
            progressIntoLevel = progress;
            dateTime = DateTime.Now.ToString("dd MMM yyyy HH:mm");
        }
    }

    [System.Serializable]
    public class RhythmAttemptData
    {
        public float totalNotes;
        public float normalHits;
        public float goodHits;
        public float perfectHits;
        public float missHits;
        public int finalScore;
        public string rank;
        public string dateTime;
        public string playerName;

        // ✅ Add this property so you can access percentHit directly
        public float percentHit => ((normalHits + goodHits + perfectHits) / totalNotes) * 100f;

        public RhythmAttemptData(
            float totalNotes,
            float normalHits,
            float goodHits,
            float perfectHits,
            float missHits,
            int finalScore,
            string rank,
            string playerName
        )
        {
            this.totalNotes = totalNotes;
            this.normalHits = normalHits;
            this.goodHits = goodHits;
            this.perfectHits = perfectHits;
            this.missHits = missHits;
            this.finalScore = finalScore;
            this.rank = rank;
            this.playerName = playerName;
            this.dateTime = System.DateTime.Now.ToString("dd MMM yyyy HH:mm");
        }
    }

    [System.Serializable]
    public class MazeAttemptData
    {
        public string playerName;
        public int levelReached;
        public int totalHintsUsed;
        public string dateTime;

        public MazeAttemptData(string name, int level, int hints)
        {
            playerName = name;
            levelReached = level;
            totalHintsUsed = hints;
            dateTime = DateTime.Now.ToString("dd MMM yyyy HH:mm");
        }
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