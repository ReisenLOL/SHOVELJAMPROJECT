using System;
using System.Collections.Generic;
using UnityEngine;

public class FluxStorable : Building
{
    public float fluxMaxCapacity;
    public float currentFlux;
    public bool maxCapacity;
    public bool hasInterface;
    public FluxGrid connectedGrid;
    public Transform[] detectPipes;
    public List<FluxStorable> connectedPipes = new();

    protected virtual void Update()
    {
        if (refreshBuildings && connectedPipes.Count != detectPipes.Length)
        {
            FindPipes();
            refreshBuildings = false; 
        }
    }
    public void StoreFlux(float flux)
    {
        if (!maxCapacity)
        {
            currentFlux += flux;
            if (connectedGrid)
            {
                connectedGrid.StoreFluxTotal(flux);
            }
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
            if (connectedGrid)
            {
                connectedGrid.DrainFluxTotal(flux);
            }
            if (maxCapacity)
            {
                maxCapacity = false;
            }
        }
    }
    public void PlayerStoreFlux()
    {
        float change = player.fluxMoveSpeed * Time.deltaTime;
        if (hasInterface && player.fluxStored > change)
        {
            player.DrainFlux(change);
            StoreFlux(change);   
        }
    }
    public void PlayerDrainFlux()
    {
        float change = player.fluxMoveSpeed * Time.deltaTime;
        if (hasInterface && (currentFlux > change || connectedGrid && connectedGrid.currentFluxStoredTotal > change))
        {
            player.StoreFlux(change);
            DrainFlux(change);   
        }
    }
    protected virtual void FindPipes()
    {
        foreach (Transform detectPipe in detectPipes)
        {
            Collider2D detectedBuilding = Physics2D.OverlapCircle(detectPipe.position, 0.1f, LayerMask.GetMask("Building"));
            if (detectedBuilding && detectedBuilding.TryGetComponent(out Pipe isPipe) && !connectedPipes.Contains(isPipe))
            {
                connectedPipes.Add(isPipe);
                if (connectedGrid && isPipe.connectedGrid)
                {
                    connectedGrid.MergeGrids(isPipe.connectedGrid);
                }
                else if (connectedGrid && !isPipe.connectedGrid)
                {
                    isPipe.connectedGrid = connectedGrid;
                    connectedGrid.AddBuilding(isPipe);
                }
                else if (!connectedGrid && isPipe.connectedGrid)
                {
                    connectedGrid = isPipe.connectedGrid;
                    connectedGrid.AddBuilding(this);
                }
                else if (!connectedGrid && !isPipe.connectedGrid)
                {
                    GameObject newFluxGrid = new GameObject();
                    FluxGrid fluxGrid = newFluxGrid.AddComponent<FluxGrid>();
                    connectedGrid = fluxGrid;
                    isPipe.connectedGrid = fluxGrid;
                    connectedGrid.AddBuilding(this);
                    connectedGrid.AddBuilding(isPipe);
                }
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (connectedGrid)
        {
            connectedGrid.RemoveBuilding(this);
            PlacementHandler placementHandler = FindFirstObjectByType<PlacementHandler>();
            placementHandler.placedBuildings.Remove(this);
            foreach (FluxStorable connectedPipe in connectedPipes)
            {
                if (connectedPipe.connectedPipes.Contains(this))
                {
                    connectedPipe.connectedPipes.Remove(this);
                }
            }
            foreach (Building building in placementHandler.placedBuildings)
            {
                building.refreshBuildings = true;
            }
        }
    }
}
