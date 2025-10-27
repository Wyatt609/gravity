using UnityEngine;

public class GravityAnchor : MonoBehaviour
{
    public float surfaceOffset = 0.01f;

    public void Initialize(Vector3 hitPoint, Vector3 surfaceNormal)
    {
        transform.position = hitPoint + surfaceNormal.normalized * surfaceOffset;
        transform.rotation = Quaternion.LookRotation(-surfaceNormal, Vector3.up);
    }
}
