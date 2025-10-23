using UnityEngine;

public class Gun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public Camera fpsCam;

    [Header("Gravity Anchor")]
    public GameObject anchorPrefab;   // assign prefab in Inspector
    private GameObject lastAnchor;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
            Shoot();
    }

    void Shoot()
    {
        if (fpsCam == null)
        {
            Debug.LogWarning("Gun: fpsCam is not assigned.");
            return;
        }

        Ray ray = new Ray(fpsCam.transform.position, fpsCam.transform.forward);
        if (!Physics.Raycast(ray, out RaycastHit hit, range, ~0, QueryTriggerInteraction.Ignore))
        {
            Debug.Log("Gun: Raycast missed.");
            return;
        }

        Debug.Log($"Gun: Hit {hit.collider.name} | normal {hit.normal}");

        // Remove previous anchor if any
        if (lastAnchor != null) Destroy(lastAnchor);

        if (anchorPrefab != null)
        {
            lastAnchor = Instantiate(anchorPrefab);
            var anchor = lastAnchor.GetComponent<GravityAnchor>();
            if (anchor == null) anchor = lastAnchor.AddComponent<GravityAnchor>();
            anchor.Initialize(hit.point, hit.normal);
        }
        else
        {
            // Fallback: still flip gravity
            if (GravityManager.Instance != null)
                GravityManager.Instance.SetGravityDirection(-hit.normal);
        }
    }
}
