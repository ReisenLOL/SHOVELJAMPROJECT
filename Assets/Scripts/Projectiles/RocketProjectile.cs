using Bremsengine;
using UnityEngine;

public class RocketProjectile : MoveProjectile
{
    public float blastRadius;
    public LayerMask enemyLayers;
    public LayerMask turretLayers;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isEnemyBullet && (other.TryGetComponent(out Building isBuilding) || other.TryGetComponent(out PlayerController isPlayer)))
        {
            Collider2D[] allDamageable = Physics2D.OverlapCircleAll(transform.position, blastRadius, enemyLayers);
            foreach (Collider2D damageable in allDamageable)
            {
                if (damageable.TryGetComponent(out Building damageableBuilding))
                {
                    damageableBuilding.TakeDamage(damage);
                }
                else if (damageable.TryGetComponent(out PlayerController player))
                {
                    player.TakeDamage(damage);
                }
            }
            GeneralManager.FunnyExplosion(transform.position, blastRadius/4);
            Destroy(gameObject);
            return;
        }

        if (!isEnemyBullet && other.TryGetComponent(out EnemyController isEnemy))
        {
            Collider2D[] allDamageable = Physics2D.OverlapCircleAll(transform.position, blastRadius, turretLayers);
            foreach (Collider2D damageable in allDamageable)
            {
                if (damageable.TryGetComponent(out EnemyController damageableEnemy))
                {
                    damageableEnemy.TakeDamage(damage);
                }
            }
            GeneralManager.FunnyExplosion(transform.position, blastRadius/4);
            isEnemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
