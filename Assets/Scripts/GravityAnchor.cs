using UnityEngine;

public class GravityAnchor : MonoBehaviour
{
    [Tooltip("Offset to avoid z-fighting with the surface.")]
    public float surfaceOffset = 0.01f;

    public void Initialize(Vector3 hitPoint, Vector3 surfaceNormal)
    {
        // position and orient the anchor on the surface
        transform.position = hitPoint + surfaceNormal.normalized * surfaceOffset;
        transform.rotation = Quaternion.LookRotation(-surfaceNormal, Vector3.up);

        // confirm that we hit something
        Debug.Log("GravityAnchor → surface normal = " + surfaceNormal);

        // flip global gravity toward that surface
        if (GravityManager.Instance != null)
            GravityManager.Instance.SetGravityDirection(-surfaceNormal);
        else
            Debug.LogWarning("GravityAnchor → No GravityManager in scene!");
    }

    // optional gizmo for visual debugging
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.08f);
        Gizmos.DrawLine(transform.position, transform.position - transform.forward * 0.3f);
    }
}
