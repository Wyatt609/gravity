using UnityEngine;
using System;

public class GravityManager : MonoBehaviour
{
    public static GravityManager Instance { get; private set; }

    [Tooltip("How strong gravity pulls (m/s²).")]
    public float gravityStrength = 9.81f;

    public Vector3 GravityDir { get; private set; } = Vector3.down;
    public event Action<Vector3> OnGravityChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetGravityDirection(Vector3 worldDown)
    {
        if (worldDown.sqrMagnitude < 0.0001f) return;

        Vector3 newDir = worldDown.normalized;
        if (Vector3.Dot(newDir, GravityDir) > 0.9999f) return; // ignore tiny differences

        GravityDir = newDir;
        Debug.Log("GravityManager → GravityDir set to: " + GravityDir);

        OnGravityChanged?.Invoke(GravityDir);
    }

    public Vector3 GravityAcceleration => GravityDir * gravityStrength;
}
