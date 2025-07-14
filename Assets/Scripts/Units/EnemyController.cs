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
    private Core core;

    [Header("CACHE")] 
    public List<Collider2D> targetList = new();
    private GameObject closestTarget;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        player = FindFirstObjectByType<PlayerController>();
        core = FindFirstObjectByType<Core>();
    }
    void Update()
    {
        Movement();
        GetClosestTarget();
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
}
