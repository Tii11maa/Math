using UnityEngine;

public class CannonController : MonoBehaviour
{
    [Header("Cannon References")]
    public Transform cannonBase;     // Rotates on Y axis
    public Transform cannonBarrel;   // Rotates on X axis
    public Transform target;         // Target sphere
    public GameObject projectilePrefab; // Projectile prefab

    [Header("Settings")]
    public float baseRotateSpeed = 2f;
    public float barrelRotateSpeed = 2f;
    public float fireCooldown = 1.5f;
    public float projectileForce = 20f;
    public float fireThreshold = 0.98f; // Dot product threshold

    private float fireTimer = 0f;

    void Update()
    {
        if (target == null) return;

        // Direction vector to target
        Vector3 direction = (target.position - cannonBarrel.position).normalized;

        // --- Base rotation (Y axis only) ---
        Vector3 baseDirection = new Vector3(direction.x, 0, direction.z);
        if (baseDirection.sqrMagnitude > 0.001f)
        {
            Quaternion baseTargetRotation = Quaternion.LookRotation(baseDirection);
            cannonBase.rotation = Quaternion.Slerp(
                cannonBase.rotation,
                baseTargetRotation,
                Time.deltaTime * baseRotateSpeed
            );
        }

        // --- Barrel rotation (X axis only) ---
        Vector3 localTarget = cannonBase.InverseTransformPoint(target.position);
        Vector3 flatTarget = new Vector3(0, localTarget.y, localTarget.z);
        if (flatTarget.sqrMagnitude > 0.001f)
        {
            Quaternion barrelTargetRotation = Quaternion.LookRotation(flatTarget);
            cannonBarrel.localRotation = Quaternion.Slerp(
                cannonBarrel.localRotation,
                barrelTargetRotation,
                Time.deltaTime * barrelRotateSpeed
            );
        }

        // --- Firing logic ---
        fireTimer -= Time.deltaTime;
        float dot = Vector3.Dot(cannonBarrel.forward, direction);

        if (dot >= fireThreshold && fireTimer <= 0f)
        {
            FireProjectile(direction);
            fireTimer = fireCooldown;
        }
    }

    void FireProjectile(Vector3 direction)
    {
        GameObject projectile = Instantiate(
            projectilePrefab,
            cannonBarrel.position + cannonBarrel.forward * 1f, // spawn at barrel tip
            Quaternion.identity
        );

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero; // reset
            rb.AddForce(direction * projectileForce, ForceMode.VelocityChange);
        }
    }
}
