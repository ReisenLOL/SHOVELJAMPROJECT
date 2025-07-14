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
            foreach (FluxStorable storable in connectedBuildings)
            {
                if (storable.fluxMaxCapacity != 0 && !storable.maxCapacity && storable.currentFlux + flux < storable.fluxMaxCapacity)
                {
                    storable.StoreFlux(flux);
                    break;
                }
            }
            if (currentFluxStoredTotal >= fluxStorageMaximumTotal)
            {
                maxCapacityTotal = true;
                currentFluxStoredTotal = fluxStorageMaximumTotal;
            }   
        }
    }
    public void DrainFluxTotal(float flux)
    {
        foreach (FluxStorable storable in connectedBuildings)
        {
            if (storable.fluxMaxCapacity != 0 && storable.currentFlux - flux < 0)
            {
                storable.DrainFlux(flux);
                break;
            }
        }
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
