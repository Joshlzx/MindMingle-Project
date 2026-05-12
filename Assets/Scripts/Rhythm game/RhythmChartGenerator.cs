using UnityEngine;
using System.IO;

public class RhythmChartGenerator : MonoBehaviour
{
    [Header("Chart Settings")]
    public string fileName = "song1.json";

    [Header("Prefabs & Lanes")]
    public GameObject[] notePrefabs;
    public Transform[] laneSpawnPoints;
    public Transform noteHolder;

    [Header("Scroll Settings")]
    public float noteSpeed = 5f;
    public float spawnY = 6f;
    public float hitY = -3f;

    [Header("Audio")]
    public AudioSource music; 

    public void GenerateNotes()
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (!File.Exists(path))
        {
            Debug.LogError("Chart file not found at: " + path);
            return;
        }

        RhythmChart chart = JsonUtility.FromJson<RhythmChart>(File.ReadAllText(path));
        float travelTime = Mathf.Abs(spawnY - hitY) / noteSpeed;

        
        float[] laneRotations = { 180f, 90f, -90f, 0f };

        foreach (var note in chart.notes)
        {
            Vector3 pos = laneSpawnPoints[note.lane].position;
            pos.y = spawnY;

            
            Quaternion rotation = Quaternion.Euler(0f, 0f, laneRotations[note.lane]);

            GameObject noteObj = Instantiate(
                notePrefabs[note.lane],
                pos,
                rotation,
                noteHolder
            );

            BeatScroller scroller = noteObj.GetComponent<BeatScroller>();
            if (scroller != null)
            {
                scroller.noteSpeed = noteSpeed;
                scroller.spawnY = spawnY;
                scroller.hitY = hitY;
                scroller.spawnTime = note.time - travelTime;
                scroller.music = music;
            }
        }

        Debug.Log("Chart generated! Notes are ready, music not played.");
    }
}

//How to use the charter: Go into the game and whenever youre ready, press P to start the music and charting. Use ASDF to place down the notes.
//The script will look for a JSON file in the persistent data path with the name specified in the fileName variable (default is "song1.json").
//Make sure to create this JSON file with the appropriate structure to define your rhythm chart.
//Then in RhythmChartGenerator in hierarchy, press "Generate Notes From Chart" to place down the notes that you've charted.