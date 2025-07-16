using System;
using UnityEngine;

public class FluxPump : FluxStorable
{
    public float fluxToProduce;
    public float fluxProductionRate;
    public bool canProduce;
    public Transform resourceDetection;

    protected override void Start()
    {
        base.Start();
        if (resourceDetection)
        {
            Collider2D detectGeyser = Physics2D.OverlapCircle(resourceDetection.position, 0.1f, LayerMask.GetMask("Resource"));
            canProduce = detectGeyser;   
            if (canProduce)
            {
                if (detectGeyser.TryGetComponent(out Geyser geyser))
                {
                    geyser.hasPump = true;
                    FindFirstObjectByType<WaveManager>().UpdateSpecificGeyserStatus(geyser);
                }
            }
        }
    }

    protected override void Update()
    {
        base.Update();
        if (canProduce)
        {
            ProduceFlux();
        }
    }

    protected virtual void ProduceFlux()
    {
        if (!maxCapacity)
        {
            StoreFlux(fluxToProduce * fluxProductionRate * Time.deltaTime);
        }
        else if (connectedGrid)
        {
            connectedGrid.StoreFluxTotal(fluxToProduce * fluxProductionRate * Time.deltaTime);
        }
    }
}
