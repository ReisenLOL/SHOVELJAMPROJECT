using System;
using UnityEngine;

public class FluxPump : FluxStorable
{
    public float fluxToProduce;
    public float fluxProductionRate;
    public bool canProduce;
    public Transform resourceDetection;

    private void Start()
    {
        canProduce = Physics2D.OverlapCircle(resourceDetection.position, 0.1f, LayerMask.GetMask("Resource"));
    }

    protected override void Update()
    {
        base.Update();
        if (canProduce)
        {
            StoreFlux(fluxToProduce * fluxProductionRate * Time.deltaTime);
        }
    }
}
