using UnityEngine;

public class GameManager : MonoBehaviour
{
    public HitPoint hitPoint;
    public AudioSync sync;

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            TryHit("out");
        if (Input.GetKeyDown(KeyCode.X))
            TryHit("in");
    }

    void TryHit(string ring)
    {
        var notes = FindObjectsOfType<Note>();

        foreach (var note in notes)
        {
            if (note.dir != ring) continue;

            float timeDiff = Mathf.Abs(sync.SongTime - note.time);
            if (timeDiff > 0.1f) continue; // 100ms window

            if (!note.CanHit(hitPoint.angle)) continue;

            // HIT SUCCESS
            Debug.Log("HIT " + ring);
            Destroy(note.gameObject);
            return;
        }
    }
}
