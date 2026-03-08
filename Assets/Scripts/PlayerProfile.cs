using System;
using System.Collections.Generic;

[System.Serializable]
public class PlayerProfile
{
    public string playerName;

    // Overall quiz totals
    public int totalQuizQuestions;
    public int totalQuizCorrect;

    // All attempts for different games
    public List<QuizAttemptData> quizAttempts = new List<QuizAttemptData>();
    public List<SimonAttemptData> simonAttempts = new List<SimonAttemptData>();
    public List<RhythmAttemptData> rhythmAttempts = new List<RhythmAttemptData>();
    public List<MazeAttemptData> mazeAttempts = new List<MazeAttemptData>();
    public List<PathTrailAttemptData> pathTrailAttempts = new List<PathTrailAttemptData>();

    public PlayerProfile(string name)
    {
        playerName = name;
        totalQuizQuestions = 0;
        totalQuizCorrect = 0;
    }

    #region Quiz Attempt
    [System.Serializable]
    public class QuizAttemptData
    {
        public string dateTime;
        public int totalQuestions;
        public int correctAnswers;
        public int themeID; // NEW: stores the theme

        // Constructor with theme
        public QuizAttemptData(int total, int correct, int theme)
        {
            totalQuestions = total;
            correctAnswers = correct;
            themeID = theme;
            dateTime = DateTime.Now.ToString("dd MMM yyyy HH:mm");
        }
    }
    #endregion

    #region Simon Attempt
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
    #endregion

    #region Rhythm Attempt
    [System.Serializable]
    public class RhythmAttemptData
    {
        public string mapID;
        public float totalNotes;
        public float normalHits;
        public float goodHits;
        public float perfectHits;
        public float missHits;
        public int finalScore;
        public string rank;
        public string dateTime;
        public string playerName;

        public float percentHit => ((normalHits + goodHits + perfectHits) / totalNotes) * 100f;

        public RhythmAttemptData(
            string mapID,
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
            this.mapID = mapID;
            this.totalNotes = totalNotes;
            this.normalHits = normalHits;
            this.goodHits = goodHits;
            this.perfectHits = perfectHits;
            this.missHits = missHits;
            this.finalScore = finalScore;
            this.rank = rank;
            this.playerName = playerName;
            this.dateTime = DateTime.Now.ToString("dd MMM yyyy HH:mm");
        }
    }
    #endregion

    #region Maze Attempt
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
    #endregion

    #region PathTrail Attempt
    [System.Serializable]
    public class PathTrailAttemptData
    {
        public float completionTime;
        public int totalErrors;
        public int totalNodes;
        public string grade;
        public string dateTime;
        public string playerName;

        public PathTrailAttemptData(float time, int errors, string playerName, int totalNodes, string grade)
        {
            completionTime = time;
            totalErrors = errors;
            this.playerName = playerName;
            this.totalNodes = totalNodes;
            this.grade = grade;
            dateTime = DateTime.Now.ToString("dd MMM yyyy HH:mm");
        }
    }
    #endregion
}