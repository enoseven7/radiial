using UnityEngine;

public class Note : MonoBehaviour
{
    public float spawnRadius;
    public float targetRadius;
    public float angle;
    public float time;      // when it should be hit
    public string dir;      // "in" or "out"

    private AudioSync _audio;
    private bool hit = false;

    public void Init(AudioSync sync)
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

        // Despawn after passing
        if (t > 1.2f && !hit)
            Destroy(gameObject);
    }

    public bool CanHit(float hitPointAngle)
    {
        float angleDiff = Mathf.Abs(Mathf.DeltaAngle(hitPointAngle, angle));
        return angleDiff < 10f;  // 10° tolerance
    }

}
