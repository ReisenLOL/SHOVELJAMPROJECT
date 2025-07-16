using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : Unit
{
    [Header("STATS")]
    public int speed;
    private NavMeshAgent agent;
    private PlayerController player;
    private CoreController core;
    public float range;
    
    [Header("[PROJECTILE]")] 
    public Projectile projectile;
    public float projectileSpeed;
    public float projectileDamage;
    public float fireRate;
    public float currentFiringTime;


    [Header("CACHE")] 
    public List<Collider2D> targetList = new();
    private GameObject closestTarget;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = true;
        agent.speed = speed;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        player = FindFirstObjectByType<PlayerController>();
        core = FindFirstObjectByType<CoreController>();
        CreateTargetting();
    }
    void Update()
    {
        Movement();
        GetClosestTarget();
        FireProjectile();
    }

    private void GetClosestTarget()
    {
        float distanceToClosestTarget = 1000000f;
        foreach (Collider2D target in targetList.ToList())
        {
            if (target == null)
            {
                targetList.Remove(target);
                continue;
            }
            float sqrDistance = Vector3.SqrMagnitude(transform.position - target.transform.position);
            if (sqrDistance < distanceToClosestTarget)
            {
                closestTarget = target.gameObject;
                distanceToClosestTarget = sqrDistance;
            }
        }
    }
    private void Movement()
    {
        float distanceToPlayer = Vector3.SqrMagnitude(player.transform.position - transform.position);
        float distanceToCore = Vector3.SqrMagnitude(core.transform.position - transform.position);
        if (distanceToPlayer < distanceToCore)
        {
            agent.SetDestination(player.transform.position);
        }
        else
        {
            agent.SetDestination(core.transform.position);
        }
    }
    private void FireProjectile()
    {
        currentFiringTime += Time.deltaTime;
        if (closestTarget && currentFiringTime >= fireRate)
        {
            Projectile newProjectile = Instantiate(projectile);
            newProjectile.transform.position = transform.position;
            newProjectile.speed = projectileSpeed;
            newProjectile.damage = projectileDamage;
            newProjectile.RotateToTarget(closestTarget.transform.position);
            newProjectile.isEnemyBullet = true;
            currentFiringTime = 0;
        }
    }

    private void CreateTargetting()
    {
        GameObject newTargetDetection = new GameObject();
        newTargetDetection.name = "EnemyTargetting";
        newTargetDetection.transform.position = transform.position;
        newTargetDetection.transform.SetParent(transform);
        newTargetDetection.layer = LayerMask.NameToLayer("EnemyTargetDetection");
        CircleCollider2D newCC2D = newTargetDetection.AddComponent<CircleCollider2D>();
        newCC2D.radius = range;
        newCC2D.isTrigger = true;
        EnemyDetectTargets newEDT = newTargetDetection.AddComponent<EnemyDetectTargets>();
        newEDT.thisEnemy = this;
    }
}
