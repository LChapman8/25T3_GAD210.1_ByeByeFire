using UnityEngine;

public class Shooting : MonoBehaviour
{
    [Header("Projectile Prefabs")]
    public GameObject co2ProjectilePrefab;
    public GameObject waterProjectilePrefab;

    [Header("Fire Settings")]
    public Transform firePoint;    // Tip of the nozzle
    public Camera cam;             // Main camera

    [Header("Projectile Settings")]
    public float projectileSpeed = 20f;
    public float waterFireRate = 0.1f; // Time between water capsules in stream

    [Header("Colliders to Ignore")]
    public Collider[] ignoreColliders; // Assign player, hose, etc.

    private float nextWaterTime = 0f;

    void Update()
    {
        // Single CO2 shot (LMB)
        if (Input.GetMouseButtonDown(0))
            Shoot(co2ProjectilePrefab, "CO2");

        // Continuous water stream (RMB)
        if (Input.GetMouseButton(1) && Time.time >= nextWaterTime)
        {
            Shoot(waterProjectilePrefab, "Water");
            nextWaterTime = Time.time + waterFireRate;
        }
    }

    void Shoot(GameObject prefab, string type)
    {
        Vector3 shootDir = GetShootDirection();

        // Spawn projectile facing shoot direction
        GameObject proj = Instantiate(prefab, firePoint.position, Quaternion.LookRotation(shootDir));

        // Setup Rigidbody
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        rb.linearVelocity = shootDir * projectileSpeed;
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Ignore collisions with multiple colliders
        Collider projCollider = proj.GetComponent<Collider>();
        if (ignoreColliders != null)
        {
            foreach (Collider col in ignoreColliders)
            {
                if (col != null)
                    Physics.IgnoreCollision(projCollider, col);
            }
        }

        // Assign extinguisher type
        Projectile projectileScript = proj.GetComponent<Projectile>();
        if (projectileScript != null)
            projectileScript.extinguisherType = type;
    }

    Vector3 GetShootDirection()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPoint;

        // Raycast to hit fires or environment
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            // If nothing hit, shoot into distance along camera ray
            targetPoint = ray.origin + ray.direction * 100f; // 100 units forward
        }

        return (targetPoint - firePoint.position).normalized;
    }
}
