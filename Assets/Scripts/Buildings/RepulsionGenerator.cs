using System.Collections.Generic;
using UnityEngine;

public class RepulsionGenerator : FluxStorable
{
    public float fireRate;
    private float currentFiringTime;
    public float fluxFiringCost;
    public List<Rigidbody2D> targetList = new();
    public float knockbackForce;
    public float range;

    protected override void Update()
    {
        base.Update();
        RepulseEnemies();
    }

    private void RepulseEnemies()
    {
        currentFiringTime += Time.deltaTime;
        if (targetList.Count > 0 && currentFiringTime >= fireRate && connectedGrid && (currentFlux >= fluxFiringCost || connectedGrid.currentFluxStoredTotal >= fluxFiringCost))
        {
            foreach (Rigidbody2D target in targetList)
            {
                Vector3 knockbackDirection = target.transform.position - transform.position;
                target.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode2D.Impulse);
            }
            connectedGrid.DrainFluxTotal(fluxFiringCost);
            currentFiringTime = 0;
        }
    }
    private void CreateTargetting()
    {
        GameObject newTargetDetection = new GameObject();
        newTargetDetection.name = "RepulsionTargetting";
        newTargetDetection.transform.position = transform.position;
        newTargetDetection.transform.SetParent(transform);
        newTargetDetection.layer = LayerMask.NameToLayer("TurretTargetDetection");
        CircleCollider2D newCC2D = newTargetDetection.AddComponent<CircleCollider2D>();
        newCC2D.radius = range;
        newCC2D.isTrigger = true;
        RepulsionRangeHandler newRange = newTargetDetection.AddComponent<RepulsionRangeHandler>();
        newRange.thisGenerator = this;
    }
}
