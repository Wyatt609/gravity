using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 3f;
    public float waitAtEnds = 0.6f;
    public float startPhase = 0f;
    Vector3 a;
    Vector3 b;
    float t;
    int dir = 1;
    float wait;

    void Awake()
    {
        a = pointA.position;
        b = pointB.position;
        t = Mathf.Clamp01((Mathf.Sin(startPhase) + 1f) * 0.5f);
        transform.position = Vector3.Lerp(a, b, t);
        gameObject.tag = "MovingPlatform";
    }

    void Update()
    {
        if (wait > 0f) { wait -= Time.deltaTime; return; }
        float dist = Vector3.Distance(a, b);
        if (dist < 0.001f) return;
        float step = (speed / dist) * Time.deltaTime * dir;
        float nt = t + step;
        if (nt > 1f) { nt = 1f; dir = -1; wait = waitAtEnds; }
        else if (nt < 0f) { nt = 0f; dir = 1; wait = waitAtEnds; }
        transform.position = Vector3.Lerp(a, b, nt);
        t = nt;
    }
}

