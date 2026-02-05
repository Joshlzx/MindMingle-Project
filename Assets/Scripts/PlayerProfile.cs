using System.Collections.Generic;

[System.Serializable]
public class PlayerProfile
{
    public string playerName;
    public string playerID;

    public Dictionary<string, int> highScores = new Dictionary<string, int>();
    public Dictionary<string, string> ranks = new Dictionary<string, string>(); // for rhythm game

    public PlayerProfile(string name)
    {
        playerName = name;
        playerID = System.Guid.NewGuid().ToString();
    }
}