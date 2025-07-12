using System.Collections.Generic;
using UnityEngine;

public class FluxGrid : MonoBehaviour
{
    public HashSet<FluxStorable> connectedBuildings = new();
    public float currentFluxStoredTotal;
    public float fluxStorageMaximumTotal;
    public bool maxCapacityTotal;
    public void StoreFluxTotal(float flux)
    {
        if (!maxCapacityTotal)
        {
            currentFluxStoredTotal += flux;
            if (currentFluxStoredTotal >= fluxStorageMaximumTotal)
            {
                maxCapacityTotal = true;
                currentFluxStoredTotal = fluxStorageMaximumTotal;
            }   
        }
    }
    public void DrainFluxTotal(float flux)
    {
        if (currentFluxStoredTotal > flux)
        {
            currentFluxStoredTotal -= flux;
            if (maxCapacityTotal)
            {
                maxCapacityTotal = false;
            }
        }
    }

    public void AddBuilding(FluxStorable building)
    {
        connectedBuildings.Add(building);
        fluxStorageMaximumTotal += building.fluxMaxCapacity;
        currentFluxStoredTotal += building.currentFlux;
    }
    public void RemoveBuilding(FluxStorable building)
    {
        connectedBuildings.Remove(building);
        fluxStorageMaximumTotal -= building.fluxMaxCapacity;
        currentFluxStoredTotal -= building.currentFlux;
        if (currentFluxStoredTotal < 0)
        {
            currentFluxStoredTotal = 0;
        }
    }
}
