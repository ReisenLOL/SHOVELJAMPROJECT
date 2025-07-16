using System;
using UnityEngine;

public class Pipe : FluxStorable
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        if (refreshBuildings && connectedPipes.Count != detectPipes.Length)
        {
            FindPipes();
            refreshBuildings = false;
        }
    }

    protected override void FindPipes()
    {
        foreach (Transform detectPipe in detectPipes)
        {
            Collider2D detectedBuilding = Physics2D.OverlapCircle(detectPipe.position, 0.1f);
            if (detectedBuilding && detectedBuilding.TryGetComponent(out FluxStorable isFluxStorable) && !connectedPipes.Contains(isFluxStorable))
            {
                connectedPipes.Add(isFluxStorable);
                if (connectedGrid && isFluxStorable.connectedGrid)
                {
                    connectedGrid.MergeGrids(isFluxStorable.connectedGrid);
                }
                else if (connectedGrid && !isFluxStorable.connectedGrid)
                {
                    isFluxStorable.connectedGrid = connectedGrid;
                    connectedGrid.AddBuilding(isFluxStorable);
                }
                else if (!connectedGrid && isFluxStorable.connectedGrid)
                {
                    connectedGrid = isFluxStorable.connectedGrid;
                    connectedGrid.AddBuilding(this);
                }
                else if (!connectedGrid && !isFluxStorable.connectedGrid)
                {
                    GameObject newFluxGrid = new GameObject();
                    FluxGrid fluxGrid = newFluxGrid.AddComponent<FluxGrid>();
                    connectedGrid = fluxGrid;
                    isFluxStorable.connectedGrid = fluxGrid;
                    connectedGrid.AddBuilding(this);
                    connectedGrid.AddBuilding(isFluxStorable);
                }
            }
        }
    }
}
