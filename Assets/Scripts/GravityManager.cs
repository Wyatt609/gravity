using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GravityManager : MonoBehaviour
{
    public static GravityManager Instance { get; private set; }
    public float gravityStrength = 9.81f;
    public Vector3 GravityDir { get; private set; } = Vector3.down;
    public event Action<Vector3> OnGravityChanged;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        Physics.gravity = GravityDir * gravityStrength;
    }

    void OnDestroy()
    {
        if (Instance == this) SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetGravityDirection(Vector3.down);
    }

    public void SetGravityDirection(Vector3 worldDown)
    {
        if (worldDown.sqrMagnitude < 1e-6f) return;
        Vector3 snapped = SnapToAxis(worldDown.normalized);
        if (snapped == GravityDir) return;
        GravityDir = snapped;
        Physics.gravity = GravityDir * gravityStrength;
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
