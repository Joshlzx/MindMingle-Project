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

    // NEW: percentage of notes hit
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
