using UnityEngine;

public class FluxStorable : Building
{
    public float fluxMaxCapacity;
    public float currentFlux;
    public bool maxCapacity;
    public FluxGrid connectedGrid;
    public void StoreFlux(float flux)
    {
        if (!maxCapacity)
        {
            currentFlux += flux;
            if (currentFlux >= fluxMaxCapacity)
            {
                maxCapacity = true;
                currentFlux = fluxMaxCapacity;
            }   
        }
    }

    public void DrainFlux(float flux)
    {
        if (currentFlux > flux)
        {
            currentFlux -= flux;
        }
        if (maxCapacity)
        {
            maxCapacity = false;
        }
    }
}
