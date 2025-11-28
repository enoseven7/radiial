using UnityEngine;

public class HitPoint : MonoBehaviour
{
    public Transform innerRing;
    public Transform outerRing;

    public Transform outerEdge;

    public float angle; // 0–360 degrees

    void Update()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dir = mouse - transform.parent.position;
        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Apply rotation
        float radius = outerEdge.localPosition.magnitude;
        float rad = angle * Mathf.Deg2Rad;

        transform.localPosition = new Vector3(
            Mathf.Cos(rad) * radius,
            Mathf.Sin(rad) * radius,
            0
        );
    }
}
