using System;
using UnityEngine;

public class MoveProjectile : MonoBehaviour
{
    public float speed;
    public float damage;
    private Rigidbody2D rb;

    private void FixedUpdate()
    {
        rb.linearVelocity = Vector2.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Building isBuilding))
        {
            isBuilding.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent(out Unit isUnit))
        {
            isUnit.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
