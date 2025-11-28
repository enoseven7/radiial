using UnityEngine;

public class AudioSync : MonoBehaviour
{
    public AudioSource source;
    public float offset = 0f;

    public float SongTime => source.time + offset;

    void Start()
    {
        source.playOnAwake = false;
    }

    public void Play()
    {
        source.Play();
    }
}
