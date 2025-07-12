using UnityEngine;

public class FluxPump : FluxStorable
{
    public float fluxToProduce;
    public float fluxProductionRate;
    public bool canProduce;
    void Update()
    {
        if (canProduce)
        {
            StoreFlux(fluxToProduce * fluxProductionRate * Time.deltaTime);   
        }
    }
}
