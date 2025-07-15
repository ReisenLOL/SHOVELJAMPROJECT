using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurretController : FluxStorable
{
    [Header("[PROJECTILE]")] 
    public MoveProjectile projectile;
    public float range;
    public float projectileSpeed;
    public float projectileDamage;
    public float fireRate;
    public float currentFiringTime;
    public float fluxFiringCost;
    
    [Header("CACHE")] 
    public List<Collider2D> targetList = new();
    private GameObject closestTarget;

    protected override void Start()
    {
        base.Start();
        CreateTargetting();
    }

    protected override void Update()
    {
        base.Update();
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
    private void FireProjectile()
    {
        currentFiringTime += Time.deltaTime;
        if (closestTarget && currentFiringTime >= fireRate && (currentFlux >= fluxFiringCost || connectedGrid.currentFluxStoredTotal >= fluxFiringCost))
        {
            MoveProjectile newProjectile = Instantiate(projectile);
            newProjectile.transform.position = transform.position;
            newProjectile.speed = projectileSpeed;
            newProjectile.damage = projectileDamage;
            newProjectile.RotateToTarget(closestTarget.transform.position);
            newProjectile.isEnemyBullet = false;
            if (currentFlux >= fluxFiringCost)
            {
                DrainFlux(fluxFiringCost);
            }
            else
            {
                connectedGrid.DrainFluxTotal(fluxFiringCost);   
            }
            currentFiringTime = 0;
        }
    }

    private void CreateTargetting()
    {
        GameObject newTargetDetection = new GameObject();
        newTargetDetection.name = "TurretTargetting";
        newTargetDetection.transform.position = transform.position;
        newTargetDetection.transform.SetParent(transform);
        newTargetDetection.layer = LayerMask.NameToLayer("TurretTargetDetection");
        CircleCollider2D newCC2D = newTargetDetection.AddComponent<CircleCollider2D>();
        newCC2D.radius = range;
        newCC2D.isTrigger = true;
        TurretTargetting newEDT = newTargetDetection.AddComponent<TurretTargetting>();
        newEDT.thisTurret = this;
    }
}
