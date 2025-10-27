using UnityEngine;
using System;

public class GravityManager : MonoBehaviour
{
    public static GravityManager Instance { get; private set; }
    public float gravityStrength = 9.81f;
    [Range(0f,1f)] public float hysteresisDot = 0.995f;
    public Vector3 GravityDir { get; private set; } = Vector3.down;
    public event Action<Vector3> OnGravityChanged;

    void Awake()
    {
        if (Instance != this && Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetGravityDirection(Vector3 worldDown)
    {
        if (worldDown.sqrMagnitude < 0.0001f) return;
        Vector3 desired = worldDown.normalized;
        Vector3 snapped = SnapToAxis(desired);
        if (Vector3.Dot(GravityDir, snapped) >= hysteresisDot) return;
        GravityDir = snapped;
        OnGravityChanged?.Invoke(GravityDir);
    }

    Vector3 SnapToAxis(Vector3 v)
    {
        Vector3 a = new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
        if (a.x >= a.y && a.x >= a.z) return v.x >= 0f ? Vector3.right : Vector3.left;
        if (a.y >= a.x && a.y >= a.z) return v.y >= 0f ? Vector3.up    : Vector3.down;
        return v.z >= 0f ? Vector3.forward : Vector3.back;
    }

    public Vector3 GravityAcceleration => GravityDir * gravityStrength;
}
