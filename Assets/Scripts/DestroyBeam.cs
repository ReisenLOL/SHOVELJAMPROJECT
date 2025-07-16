using UnityEngine;

public class DestroyBeam : Projectile
{
    private void FixedUpdate()
    {
        rb.linearVelocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Building isBuilding) && !other.TryGetComponent(out CoreController isCore))
        {
            isBuilding.TakeDamage(isBuilding.maxHealth/8f);
            Destroy(gameObject);
        }
    }
}
