using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class Shooting : MonoBehaviour
{
    [Header("Projectile Prefabs")]
    public GameObject co2ProjectilePrefab;
    public GameObject waterProjectilePrefab;

    [Header("Fire Settings")]
    public Transform firePoint;
    public Camera cam;

    [Header("Projectile Settings")]
    public float projectileSpeed = 20f;
    public float waterFireRate = 0.1f;

    [Header("Colliders to Ignore")]
    public Collider[] ignoreColliders;

    private float nextWaterTime = 0f;

    // Reference to your canvas's GraphicRaycaster
    public GraphicRaycaster canvasRaycaster;
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;

    void Awake()
    {
        if (EventSystem.current != null)
            eventSystem = EventSystem.current;
    }

    void Update()
    {
        if (!GameManager.Instance.gameStarted || GameManager.Instance.gameEnded)
            return;

        // Only block shooting if mouse is over an actual interactable UI element
        if (IsPointerOverUI())
            return;

        if (Input.GetMouseButtonDown(0))
            Shoot(co2ProjectilePrefab, Fire.ExtinguisherType.CO2);

        if (Input.GetMouseButton(1) && Time.time >= nextWaterTime)
        {
            Shoot(waterProjectilePrefab, Fire.ExtinguisherType.Water);
            nextWaterTime = Time.time + waterFireRate;
        }
    }

    bool IsPointerOverUI()
    {
        if (canvasRaycaster == null || eventSystem == null)
            return false;

        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        canvasRaycaster.Raycast(pointerEventData, results);

        foreach (var result in results)
        {
            // Only block shooting if it's over a Button or other selectable UI
            if (result.gameObject.GetComponent<Button>() != null)
                return true;
        }

        return false;
    }

    void Shoot(GameObject prefab, Fire.ExtinguisherType type)
    {
        Vector3 shootDir = GetShootDirection();

        GameObject proj = Instantiate(prefab, firePoint.position, Quaternion.LookRotation(shootDir));

        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = shootDir * projectileSpeed;
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        Collider projCollider = proj.GetComponent<Collider>();
        if (projCollider != null && ignoreColliders != null)
        {
            foreach (Collider col in ignoreColliders)
                if (col != null)
                    Physics.IgnoreCollision(projCollider, col);
        }

        Projectile projectileScript = proj.GetComponent<Projectile>();
        if (projectileScript != null)
            projectileScript.extinguisherType = type;
    }

    Vector3 GetShootDirection()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.origin + ray.direction * 100f;

        return (targetPoint - firePoint.position).normalized;
    }
}
