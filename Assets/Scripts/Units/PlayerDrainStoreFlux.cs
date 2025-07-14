using System;
using TMPro;
using UnityEngine;

public class PlayerDrainStoreFlux : MonoBehaviour
{
    public PlayerController player;
    public LayerMask buildingLayers;
    public Vector3 worldPos;
    public Camera cam;
    
    [Header("INFO PANEL")]
    public GameObject infoPanel;
    public GameObject fluxSliderUI;
    public Transform fluxSlider;
    public TextMeshProUGUI buildingNameText;
    public TextMeshProUGUI buildingDescriptionText;
    public TextMeshProUGUI fluxAmountText;
    
    

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        worldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0;
        if (Input.GetMouseButton(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 0f, buildingLayers);
            if (hit && hit.collider.TryGetComponent(out BuildingBlueprint isBlueprint))
            {
                isBlueprint.BlueprintStoreFlux();
            }
        }
        if (Input.GetKey(KeyCode.E))
        {
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 0f, buildingLayers);
            if (hit && hit.collider.TryGetComponent(out BuildingBlueprint isBlueprint))
            {
                isBlueprint.BlueprintDrainFlux();
            }
            if (hit && hit.collider.TryGetComponent(out FluxStorable isFluxStorable))
            {
                isFluxStorable.PlayerDrainFlux();
            }
        }
        if (Input.GetMouseButton(1))
        {
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 0f, buildingLayers);
            if (hit)
            {
                hit.collider.gameObject.SetActive(false);
            }
        }

        if (Input.GetKey(KeyCode.Tab))
        {
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 0f, buildingLayers);
            if (hit && hit.collider.TryGetComponent(out Building isBuilding))
            {
                infoPanel.SetActive(true);
                buildingNameText.text = isBuilding.buildingID;
                buildingDescriptionText.text = isBuilding.description;
                if (hit.collider.TryGetComponent(out FluxStorable isFluxStorable))
                {
                    fluxSliderUI.SetActive(true);
                    fluxSlider.localScale = new Vector3(isFluxStorable.currentFlux/isFluxStorable.fluxMaxCapacity, fluxSlider.localScale.y);
                    fluxAmountText.text = MathF.Round(isFluxStorable.currentFlux) + "/" + MathF.Round(isFluxStorable.fluxMaxCapacity);
                }
            }
            if (!hit)
            {
                infoPanel.SetActive(false);
            }
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            infoPanel.SetActive(false);
        }
    }
}
