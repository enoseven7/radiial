using UnityEngine;

public class Note : MonoBehaviour
{
    public float spawnRadius;
    public float targetRadius;
    public float angle;
    public float time;      // when it should be hit
    public string dir;      // "in" or "out"

    public AudioSync _audio;
    private bool hit = false;

    public Transform approachCircle;
    public float preempt = 1.0f;

    public bool judged = false;

    public void Init(AudioSync sync, HitPoint hp = null, float spawnR = 0, float targetR = 0)
    {
        _audio = sync;
    }

    void Update()
    {
        float t = (_audio.SongTime - time + 1f);
        // +1f gives notes 1 second travel time – tweak later

        if (t < 0) return;

        // Move radially
        float radius = Mathf.Lerp(spawnRadius, targetRadius, t);

        float rad = angle * Mathf.Deg2Rad;

        transform.localPosition = new Vector3(
            Mathf.Cos(rad) * radius,
            Mathf.Sin(rad) * radius,
            0
        );
        UpdateApproachCircle(t);
        // Despawn after passing
        if (t > 1.2f && !hit && !judged)
        {
            judged = true;
            GameManager.Instance.ApplyMiss(this);
            Destroy(gameObject);
        }
            
    }

    public bool CanHit(float hitPointAngle)
    {
        float angleDiff = Mathf.Abs(Mathf.DeltaAngle(hitPointAngle, angle));
        return angleDiff < 10f;  // 10° tolerance
    }

    private void UpdateApproachCircle(float t)
    {
        float scale = Mathf.Lerp(0.2f, 0.095f, t);
        approachCircle.localScale = new Vector3(scale, scale, 1f);
    }

    public virtual void HitPerfect()
    {
        GameManager.Instance.ApplyJudgement("PERFECT", this);
        judged = true;
        Destroy(gameObject);
    }

    public virtual void Miss()
    {
        GameManager.Instance.ApplyMiss(this);
        judged = true;
        Destroy(gameObject);
    }

}
