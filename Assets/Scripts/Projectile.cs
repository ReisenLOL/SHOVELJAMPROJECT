using Core.Extensions;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed;
    public float damage;
    public float bulletLifeTime;
    public bool isEnemyBullet;
    private void Start()
    {
        Destroy(gameObject, bulletLifeTime);
    }
    public void RotateToTarget(Vector3 targetPosition)
    {
        transform.Lookat2D(targetPosition);
    }
}
