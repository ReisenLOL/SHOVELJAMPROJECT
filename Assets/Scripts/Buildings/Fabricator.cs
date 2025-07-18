using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using UnityEngine;

public class Fabricator : FluxStorable
{
    public float range;
    public LayerMask buildingsLayer;
    public List<BuildingBlueprint> blueprintsInRange = new();
    private BuildingBlueprint closestBlueprint;
    public float fluxMoveSpeed;
    public GameObject fabricationBeam;
    public GameObject createdBeam;
    protected override void Start()
    {
        base.Start();
        FindFirstObjectByType<PlacementHandler>().placedFabricators.Add(this);
    }
    protected override void Update()
    {
        base.Update();
        FabricateBuildings();
    }

    private void FabricateBuildings()
    {
        if (closestBlueprint)
        {
            float change = fluxMoveSpeed * Time.deltaTime;
            if (connectedGrid.currentFluxStoredTotal > change)
            {
                connectedGrid.DrainFluxTotal(change);
                closestBlueprint.FabricatorStoreFlux(change);
            }
            else if (createdBeam)
            {
                Destroy(createdBeam);
            }
        }
        else if (blueprintsInRange.Count > 0)
        {
            GetClosestBlueprint();
        }
        else if (!closestBlueprint && createdBeam)
        {
            Destroy(createdBeam);
        }
    }
    private void GetClosestBlueprint()
    {
        float distanceToClosestTarget = 1000000f;
        foreach (BuildingBlueprint blueprint in blueprintsInRange.ToList())
        {
            if (!blueprint)
            {
                blueprintsInRange.Remove(blueprint);
                continue;
            }
            float sqrDistance = Vector3.SqrMagnitude(transform.position - blueprint.transform.position);
            if (sqrDistance < distanceToClosestTarget)
            {
                closestBlueprint = blueprint;
                if (createdBeam)
                {
                    Destroy(createdBeam);
                }
                distanceToClosestTarget = sqrDistance;
            }
        }
        if (closestBlueprint)
        {
            createdBeam = Instantiate(fabricationBeam, transform);
            createdBeam.transform.position = transform.position + (closestBlueprint.transform.position - transform.position) / 2;
            createdBeam.transform.localScale = new Vector3(Vector3.Distance(transform.position, closestBlueprint.transform.position), 1, 1);
            createdBeam.transform.Lookat2D(closestBlueprint.transform.position);
        }
    }

    public void FindBlueprints()
    {
        blueprintsInRange.Clear();
        Collider2D[] findBuildings = Physics2D.OverlapCircleAll(transform.position, range, buildingsLayer); ;
        foreach (Collider2D foundBuilding in findBuildings)
        {
            if (foundBuilding.TryGetComponent(out BuildingBlueprint isBlueprint))
            {
                blueprintsInRange.Add(isBlueprint);
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        FindFirstObjectByType<PlacementHandler>().placedFabricators.Remove(this);
    }
}
