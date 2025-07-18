using System;
using UnityEngine;

public class BuildingBlueprint : MonoBehaviour
{
    private Building thisBuilding;
    private float requiredFlux;
    public float currentFlux;
    public PlayerController player;
    public SpriteRenderer spriteRenderer;
    private PlacementHandler placementHandler;
    public float currentState = 0;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        thisBuilding = GetComponent<Building>();
        requiredFlux = thisBuilding.fluxCost;
        player = thisBuilding.player;
        thisBuilding.enabled = false;
        placementHandler = FindFirstObjectByType<PlacementHandler>();
    }

    public void BlueprintStoreFlux()
    {
        float amountToChange = player.fluxMoveSpeed * Time.deltaTime;
        if (player.fluxStored > amountToChange)
        {
            player.DrainFlux(amountToChange);
            currentFlux += amountToChange;
            currentState = currentFlux / requiredFlux;
            spriteRenderer.color = Color.Lerp(Color.black, Color.white, currentState);
            if (currentFlux >= requiredFlux)
            { 
                OnBuilt();
            }
        } 
    }

    public void FabricatorStoreFlux(float flux)
    {
        currentFlux += flux;
        currentState = currentFlux / requiredFlux;
        spriteRenderer.color = Color.Lerp(Color.black, Color.white, currentState);
        if (currentFlux >= requiredFlux)
        { 
            OnBuilt();
        }
    }
    public void BlueprintDrainFlux()
    {
        if (currentFlux > 0)
        {
            float amountToChange = player.fluxMoveSpeed * Time.deltaTime;
            currentState = currentFlux / requiredFlux;
            player.StoreFlux(amountToChange);
            currentFlux -= amountToChange;
            spriteRenderer.color = Color.Lerp(Color.black, Color.white, currentState);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnBuilt()
    {
        thisBuilding.enabled = true;
        placementHandler.placedBuildings.Add(thisBuilding);
        foreach (Building building in placementHandler.placedBuildings)
        {
            building.OnRefresh();
        }
        thisBuilding.OnBuilt();
        Destroy(this);
    }
}
