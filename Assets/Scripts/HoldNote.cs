using UnityEngine;
using System.Collections.Generic;

public class HoldNote : Note
{
    public float duration;
    public List<NoteSpawner.PathPoint> path;

    public Transform head;

    private float lockTime;
    private bool lockedToRing = false;
    private bool holding = false;
    private bool started = false;
    private bool finished = false;

    private HitPoint hitPoint;

    public GameObject bodySegmentPrefab;
    public int maxBodySegments = 30;

    public float approachTime = 0.35f;


    public Transform bodyRoot;

    private List<Transform> segments = new List<Transform>();
    private List<Vector3> pointHistory = new List<Vector3>();
    public void Init(AudioSync sync, HitPoint hp, float spawnR, float targetR)
    {
        _audio = sync;
        hitPoint = hp;
        spawnRadius = spawnR;
        targetRadius = targetR;

        pointHistory.Clear();
        foreach(var seg in segments)
        {
            Destroy(seg.gameObject);
        }
        segments.Clear();
    }

    private void Update()
    {
        if (judged) return;
        float songTime = _audio.SongTime;

        if (songTime < time)
        {
            float tPre = 1f - Mathf.Clamp01((time - songTime) / preempt);
            UpdateHeadPosition(tPre);
            return;
        }

        float tHold = Mathf.Clamp01((songTime - time) / duration);

        if (!lockedToRing)
        {
            UpdateHeadBeforeLock(songTime);

            if (HeadReachedRing())
            {
                lockedToRing = true;
                lockTime = songTime;
            }
        }
        else
        {
            UpdateHeadAfterLock(songTime);
        }

        if (lockedToRing)
        {
            AddFuturePreview(songTime);
        }
        RecordPoint(head.localPosition);
        UpdateBodySegments();

        

        if(!started)
        {
            if (holding)
            {
                float diff = Mathf.Abs(songTime - time);
                if (diff <= GameManager.Instance.hitWindow.good)
                {
                    started = true;
                }
                else if (songTime > time + GameManager.Instance.hitWindow.good)
                {
                    Miss();
                    return;
                }
            }
            return;
        }

        if(lockedToRing)
        {
            float expectedAngle = EvaluateAnglePath(tHold);
            float angleDiff = Mathf.Abs(Mathf.DeltaAngle(hitPoint.angle, expectedAngle));

            if (angleDiff > 10f)
            {
                Miss();
                return;
            }
        
        }

        if (!holding)
        {
            Miss();
            return;
        }

        if (!finished && tHold >= 1f)
        {
            finished = true;
            HitPerfect();
        }
    }


    void UpdateHeadBeforeLock(float currentTime)
    {
        float progress = Mathf.Clamp01((currentTime - time) / (approachTime));
        float angle = EvaluateAnglePath(progress);

        float radius = Mathf.Lerp(spawnRadius, targetRadius, progress);

        head.localPosition = Polar(radius, angle);
        Debug.Log("Before lock Target radius : " + targetRadius.ToString());
    }

    void UpdateHeadAfterLock(float currentTime)
    {
        float progress = Mathf.Clamp01((currentTime - lockTime) / duration);
        float angle = EvaluateAnglePath(progress);

        head.localPosition = Polar(targetRadius, angle);
        Debug.Log("After lock Target radius : " + targetRadius.ToString());
    }

    void UpdateHeadPosition(float tPre)
    {
        float angle = path[0].angle;
        float radius = Mathf.Lerp(spawnRadius, targetRadius, tPre);
        head.localPosition = Polar(radius, angle);
        
    }

    bool HeadReachedRing()
    {
        float r = head.localPosition.magnitude;

        if (dir == "in")
            return r <= targetRadius + 0.05f;

        return r >= targetRadius - 0.05f;
    }

    float EvaluateAnglePath(float tNorm)
    {
        if (path.Count == 1)
        {
            return path[0].angle;
        }

        for (int i = 0; i < path.Count - 1; i++)
        {
            var a = path[i];
            var b = path[i + 1];

            if (tNorm >= a.t && tNorm <= b.t)
            {
                float p = (tNorm - a.t) / (b.t - a.t);
                return Mathf.LerpAngle(a.angle, b.angle, p);
            }
        }

        return path[path.Count - 1].angle;
    }

    Vector3 Polar(float radius,  float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius, 0);
    }

    public void SetHolding(bool state)
    {
        holding = state;
    }

    void RecordPoint(Vector3 pos)
    {
        if (pos.magnitude < 0.1f)
            return;

        pointHistory.Add(pos);

        if(pointHistory.Count > maxBodySegments)
        {
            pointHistory.RemoveAt(0);
        }
    }

    void UpdateBodySegments()
    {
        while (segments.Count < pointHistory.Count)
        {
            var seg = Instantiate(bodySegmentPrefab, bodyRoot);
            segments.Add(seg.transform);
        }

        for (int i = 0; i < pointHistory.Count; i++)
        {
            segments[i].localPosition = pointHistory[i];
        }
    }

    void AddFuturePreview(float currentTime)
    {
        float previewLength = 0.5f;
        float step = previewLength / 10f;

        for (float dt = 0; dt < previewLength; dt += step)
        {
            float futureT = Mathf.Clamp01((currentTime - time + dt) / duration);
            float futureAngle = EvaluateAnglePath(futureT);
            Vector3 futurePos = Polar(targetRadius, futureAngle);
            pointHistory.Add(futurePos);
        }

        while (pointHistory.Count > maxBodySegments)
        {
            pointHistory.RemoveAt(0); 
        }
    }

}
