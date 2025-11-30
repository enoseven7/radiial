using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public HitPoint hitPoint;
    public AudioSync sync;

    [System.Serializable]
    public class HitWindow
    {
        public float perfect = 0.10f;
        public float great = 0.3f;
        public float good = 0.5f;
    }

    public int combo = 0;
    public int maxCombo = 0;
    public int score = 0;
    public int perfectScore = 300;
    public int greatScore = 100;
    public int goodScore = 50;

    public HitWindow hitWindow;

    public TMP_Text comboText;
    public TMP_Text scoreText;

    public GameObject judgementPrefab;
    public Transform judgementContainer;
    public float judgementOffset = 0.5f;

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

        if (Input.GetKeyDown(KeyCode.Z))
            SetHolding("out", true);
        if (Input.GetKeyUp(KeyCode.Z))
            SetHolding("out", false);

        if (Input.GetKeyDown(KeyCode.X))
            SetHolding("in", true);
        if (Input.GetKeyUp(KeyCode.X))
            SetHolding("in", false);

    }

    void SetHolding(string ring, bool hold)
    {
        var notes = FindObjectsOfType<HoldNote>();
        foreach (var n in notes)
        {
            if (n.dir == ring)
                n.SetHolding(hold);
        }
    }


    void TryHit(string ring)
    {
        var notes = FindObjectsOfType<Note>();

        foreach (var note in notes)
        {
            if (note.dir != ring) continue;

            float timeDiff = Mathf.Abs(sync.SongTime - note.time);

            Debug.Log(timeDiff);

            string judgement;

            if (timeDiff <= hitWindow.perfect)
                judgement = "PERFECT";
            else if (timeDiff <= hitWindow.great)
                judgement = "GREAT";
            else if (timeDiff <= hitWindow.good)
                judgement = "GOOD";
            else
                judgement = "MISS";

            if (!note.CanHit(hitPoint.angle)) continue;

            ApplyJudgement(judgement, note);
            // HIT SUCCESS
            Debug.Log("HIT " + ring);
            Destroy(note.gameObject);
            return;
        }
    }

    public void ApplyJudgement(string j, Note lastHitNote)
    {
        lastHitNote.judged = true;
        switch (j)
        {
            case ("PERFECT"):
                score += perfectScore;
                combo++;
                break;
            case ("GREAT"):
                score += greatScore;
                combo++;
                break;
            case ("GOOD"):
                score += greatScore;
                combo++;
                break;
            default:
                combo = 0;
                break;
        }

        SpawnJudgement(j, lastHitNote);

        comboText.text = combo.ToString();
        scoreText.text = score.ToString();
        maxCombo = Mathf.Max(maxCombo, combo);
    }

    void SpawnJudgement(string j, Note note)
    {
        Vector2 basePos = note.transform.position;

        Vector2 randomoffset = new Vector2(
            Random.Range(-judgementOffset, judgementOffset),
            Random.Range(-judgementOffset, judgementOffset));

        Vector2 spawnPos = basePos + randomoffset;

        GameObject go = Instantiate(judgementPrefab, spawnPos, Quaternion.identity, judgementContainer);
        var tmp = go.GetComponent<TMP_Text>();
        tmp.text = j;

        StartCoroutine(FadeAndDestroy(go));
    }

    IEnumerator FadeAndDestroy(GameObject obj)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        float time = 0f;
        float duration = 0.4f;

        while (time < duration)
        {
            time += Time.deltaTime;
            cg.alpha = 1f - (time / duration);
            obj.transform.localPosition += Vector3.up * 0.002f;
            yield return null;
        }

        Destroy(obj);
    }

    public void ApplyMiss(Note note)
    {
        combo = 0;
        SpawnJudgement("MISS", note);
        Debug.Log("MISS");
    }
}
