using UnityEngine;

public class HitPoint : MonoBehaviour
{
    public Transform innerRing;
    public Transform outerRing;

    public Transform outerEdge;
    public Transform innerEdge;

    public bool isOuter;

    public float angle; // 0–360 degrees

    void Update()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dir = mouse - transform.parent.position;
        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        float radius;
        // Apply rotation
        if (isOuter)
            radius = outerEdge.localPosition.magnitude;
        else
            radius = innerEdge.localPosition.magnitude;

            float rad = angle * Mathf.Deg2Rad;

        transform.localPosition = new Vector3(
            Mathf.Cos(rad) * radius,
            Mathf.Sin(rad) * radius,
            0
        );
    }
}
