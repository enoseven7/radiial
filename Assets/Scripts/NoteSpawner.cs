using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class NoteSpawner : MonoBehaviour
{
    public AudioSync sync;
    public GameObject notePrefab;
    public GameObject holdNotePrefab;
    public Transform noteContainer;

    public float innerRadius;
    public float outerRadius;

    public Transform innerEdge;
    public Transform outerEdge;

    private List<BeatNote> notes;

    public HitPoint hitPoint;

    [System.Serializable]
    public struct PathPoint
    {
        public float t;
        public float angle;

        public PathPoint(float t, float angle)
        {
            this.t = t;
            this.angle = angle;
        }
    }

    [System.Serializable]
    public class BeatNote
    {
        public float time;
        public float angle;
        public string dir;
        public string type;

        public float duration;
        public List<PathPoint> path = null;

        public BeatNote(float t, float a, string d)
        {
            type = "tap";
            time = t;
            angle = a;
            dir = d;
            duration = 0f;
            path = null;
        }

        public BeatNote(float t, float duration, string dir, List<PathPoint> path)
        {
            type = "hold";
            time = t;
            this.duration = duration;
            this.dir = dir;

            if (path != null && path.Count > 0)
            {
                angle = path[0].angle;
            }

            this.path = path;
        }
    }

    void Start()
    {
        // later: load from JSON
        notes = new List<BeatNote>()
        {
            new BeatNote(1.0f, 45f, "in"),
            new BeatNote(2.0f, 120f, "out"),
            new BeatNote(3.0f, 260f, "in"),
            new BeatNote(4.0f, 260f, "in"),
            new BeatNote(5.0f, 260f, "in"),
            new BeatNote(6.0f, 260f, "in"),
            new BeatNote(7.0f, 260f, "in"),
            new BeatNote(
                10f,
                1f,
                "in",
                new List<PathPoint>()
                {
                    new PathPoint(0f, 45f),
                    new PathPoint(0.5f, 120f),
                    new PathPoint(1f, 260f)
                }
            )
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
        if (b.type == "hold")
        {
            var go = Instantiate(holdNotePrefab, noteContainer);
            var n = go.GetComponent<HoldNote>();
            n.time = b.time;
            n.dir = b.dir;
            n.path = b.path;
            n.duration = b.duration;

            n.spawnRadius = (b.dir == "in") ? outerRadius : innerRadius;
            n.targetRadius = (b.dir == "in") ? innerRadius : outerRadius;

            n.Init(sync, hitPoint, n.spawnRadius, n.targetRadius);
        }
        else
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
}




