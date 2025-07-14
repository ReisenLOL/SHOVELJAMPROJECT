using System;
using Core.Extensions;
using UnityEngine;

public class MoveProjectile : MonoBehaviour
{
    public float speed;
    public float damage;
    public Rigidbody2D rb;
    public bool isEnemyBullet;
    public float bulletLifeTime;
    private void Start()
    {
        Destroy(gameObject, bulletLifeTime);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isEnemyBullet && other.TryGetComponent(out Building isBuilding))
        {
            isBuilding.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
        if (!isEnemyBullet && other.TryGetComponent(out EnemyController isEnemy))
        {
            isEnemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
        if (isEnemyBullet && other.TryGetComponent(out PlayerController isPlayer))
        {
            isPlayer.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    public void RotateToTarget(Vector3 targetPosition)
    {
        transform.Lookat2D(targetPosition);
    }
}
