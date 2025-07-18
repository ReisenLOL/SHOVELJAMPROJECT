using Core.Extensions;
using UnityEngine;

public class FluxDistributor : FluxStorable
{
    public bool playerInRange;
    public float range;
    public float fluxMovementSpeed;
    public GameObject distributorBeam;
    public GameObject createdBeam;
    protected override void Start()
    {
        base.Start();
        CreateRange();
    }
    protected override void Update()
    {
        base.Update();
        DistributeFlux();
    }

    private void DistributeFlux()
    {
        if (playerInRange && connectedGrid)
        {
            float change = fluxMovementSpeed * Time.deltaTime;
            createdBeam.transform.position = transform.position + (player.transform.position - transform.position) / 2;
            createdBeam.transform.localScale = new Vector3(Vector3.Distance(transform.position, player.transform.position), 1, 1);
            createdBeam.transform.Lookat2D(player.transform.position);
            if (connectedGrid.currentFluxStoredTotal > change)
            {
                connectedGrid.DrainFluxTotal(change);
                player.StoreFlux(change);
            }
        }
    }

    public void CreateBeam()
    {
        createdBeam = Instantiate(distributorBeam, transform);
    }

    private void CreateRange()
    {
        GameObject newTargetDetection = new GameObject();
        newTargetDetection.name = "DistributorRange";
        newTargetDetection.transform.position = transform.position;
        newTargetDetection.transform.SetParent(transform);
        newTargetDetection.layer = LayerMask.NameToLayer("DistributorRangeDetection");
        CircleCollider2D newCC2D = newTargetDetection.AddComponent<CircleCollider2D>();
        newCC2D.radius = range;
        newCC2D.isTrigger = true;
        DistributorRange newRange = newTargetDetection.AddComponent<DistributorRange>();
        newRange.thisTower = this;

    }
}
