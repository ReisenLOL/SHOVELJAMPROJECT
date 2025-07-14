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
        if (player.fluxStored > 0)
        {
            float amountToChange = player.fluxMoveSpeed * Time.deltaTime;
            currentState = currentFlux / requiredFlux;
            player.DrainFlux(amountToChange);
            currentFlux += amountToChange;
            spriteRenderer.color = Color.Lerp(Color.black, Color.white, currentState);
            if (currentFlux >= requiredFlux)
            {
                thisBuilding.enabled = true;
                placementHandler.placedBuildings.Add(thisBuilding);
                foreach (Building building in placementHandler.placedBuildings)
                {
                    building.refreshBuildings = true;
                }
                Destroy(this);
            }
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
}
