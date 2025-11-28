using UnityEngine;
using System.Collections.Generic;

public class NoteSpawner : MonoBehaviour
{
    public AudioSync sync;
    public GameObject notePrefab;
    public Transform noteContainer;

    public float innerRadius;
    public float outerRadius;

    public Transform innerEdge;
    public Transform outerEdge;

    private List<BeatNote> notes;

    void Start()
    {
        // later: load from JSON
        notes = new List<BeatNote>()
        {
            new BeatNote(1.0f, 45f, "in"),
            new BeatNote(2.0f, 120f, "out"),
            new BeatNote(3.0f, 260f, "in")
        };

        innerRadius = innerEdge.localPosition.magnitude;
        outerRadius = outerEdge.localPosition.magnitude;
    }

    void Update()
    {
        foreach (var n in notes.ToArray())
        {
            if (sync.SongTime + 1f >= n.time)
            {
                SpawnNote(n);
                notes.Remove(n);
            }
        }
    }

    void SpawnNote(BeatNote b)
    {
        var go = Instantiate(notePrefab, noteContainer);
        var n = go.GetComponent<Note>();
        n.angle = b.angle;
        n.time = b.time;
        n.dir = b.dir;

        n.spawnRadius = (b.dir == "in") ? outerRadius : innerRadius;
        n.targetRadius = (b.dir == "in") ? innerRadius : outerRadius;

        n.Init(sync);
    }
}

[System.Serializable]
public class BeatNote
{
    public float time;
    public float angle;
    public string dir;

    public BeatNote(float t, float a, string d)
    {
        time = t; angle = a; dir = d;
    }
}
