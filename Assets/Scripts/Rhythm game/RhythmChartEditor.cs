using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class RhythmChartEditor : MonoBehaviour
{
    [Header("Scroll Settings")]
    public float noteSpeed = 5f;
    public float spawnY = 6f;
    public float hitY = -3f;

    [Header("Beat Settings")]
    public float bpm = 120f;
    public int snapDivision = 4;

    [Header("Audio & Prefabs")]
    public AudioSource music;
    public Transform[] laneSpawnPoints;
    public GameObject[] notePrefabs;

    public string fileName = "song1.json";

    private RhythmChart chart = new RhythmChart();
    private List<GameObject> previewNotes = new List<GameObject>();
    private float travelTime;
    private bool started;

    void Start()
    {
        travelTime = Mathf.Abs(spawnY - hitY) / noteSpeed;
    }

    void Update()
    {
        // Start music
        if (!started && Input.GetKeyDown(KeyCode.Space))
        {
            music.Play();
            started = true;
        }

        if (!started) return;

        // Lane input
        CheckLane(KeyCode.A, 0);
        CheckLane(KeyCode.S, 1);
        CheckLane(KeyCode.D, 2);
        CheckLane(KeyCode.F, 3);

        // Undo last note
        if (Input.GetKeyDown(KeyCode.Backspace)) UndoLastNote();

        // Save chart
        if (Input.GetKeyDown(KeyCode.P)) SaveChart();
    }

    void CheckLane(KeyCode key, int lane)
    {
        if (!Input.GetKeyDown(key)) return;

        float hitTime = SnapToBeat(music.time);
        chart.notes.Add(new NoteData(hitTime, lane));

        float spawnTime = hitTime - travelTime;

        // Spawn preview immediately at spawnY
        Vector3 pos = laneSpawnPoints[lane].position;
        pos.y = spawnY;

        GameObject noteObj = Instantiate(notePrefabs[lane], pos, Quaternion.identity);
        BeatScroller scroller = noteObj.GetComponent<BeatScroller>();
        if (scroller != null)
        {
            scroller.noteSpeed = noteSpeed;
            scroller.spawnY = spawnY;
            scroller.hitY = hitY;
            scroller.spawnTime = spawnTime;
            scroller.music = music;
        }

        previewNotes.Add(noteObj);
    }

    void UndoLastNote()
    {
        if (chart.notes.Count == 0) return;

        chart.notes.RemoveAt(chart.notes.Count - 1);

        GameObject lastPreview = previewNotes[previewNotes.Count - 1];
        Destroy(lastPreview);
        previewNotes.RemoveAt(previewNotes.Count - 1);
    }

    void SaveChart()
    {
        string json = JsonUtility.ToJson(chart, true);
        string path = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllText(path, json);
        Debug.Log("Chart saved at: " + path);
    }

    float SnapToBeat(float time)
    {
        float beatLength = 60f / bpm;
        float snapLength = beatLength / snapDivision;
        return Mathf.Round(time / snapLength) * snapLength;
    }
}