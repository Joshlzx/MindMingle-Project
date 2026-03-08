using System;
using System.Collections.Generic;

[Serializable]
public class RhythmChart
{
    public List<NoteData> notes = new List<NoteData>();
}

[Serializable]
public class NoteData
{
    public float time; // hit time in seconds
    public int lane;   // lane index

    public NoteData(float t, int l)
    {
        time = t;
        lane = l;
    }
}