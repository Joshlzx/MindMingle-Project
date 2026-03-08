using UnityEngine;

public class BeatScroller : MonoBehaviour
{
    [HideInInspector] public float noteSpeed;
    [HideInInspector] public float spawnY;
    [HideInInspector] public float hitY;
    [HideInInspector] public float spawnTime;
    [HideInInspector] public AudioSource music;

    void Update()
    {
        if (music == null) return;

        float elapsed = music.time - spawnTime;

        // Move note from spawnY to hitY based on elapsed time
        transform.position = new Vector3(
            transform.position.x,
            spawnY - elapsed * noteSpeed,
            transform.position.z
        );
    }
}