using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlacementHandler : MonoBehaviour
{
    public Building[] allBuildings;
    [Header("[CACHE]")] 
    public Transform buildingUIFrame;
    public Button buttonTemplate;
    public Building selectedBuilding;
    public GameObject placeholderBuilding;
    private Grid placementGrid;
    public Transform buildingFolder;
    private int rotationAmount;
    public bool createPlaceholder;
    public bool isBuilding;
    public PlayerController player;
    private Vector3 worldPos;
    private Camera cam;

    private void Start()
    {
        placementGrid = FindAnyObjectByType<Grid>();
        player = FindFirstObjectByType<PlayerController>();
        cam = Camera.main;
        foreach (Building building in allBuildings)
        {
            Button newButton = Instantiate(buttonTemplate, buildingUIFrame);
            newButton.gameObject.SetActive(true);
            newButton.onClick.AddListener(() => SelectBuilding(building));
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = building.fluxCost.ToString();
            newButton.transform.Find("BuildingSprite").GetComponent<Image>().sprite = building.GetComponent<SpriteRenderer>().sprite;
        }
    }

    private void Update()
    {
        PlaceBuildingBlueprint();
    }

    private void PlaceBuildingBlueprint()
    {
        if (isBuilding)
        {
            worldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0;
            placeholderBuilding.transform.position = placementGrid.GetCellCenterWorld(placementGrid.WorldToCell(worldPos));
            placeholderBuilding.transform.rotation = Quaternion.Euler(0,0,rotationAmount);
            if (Input.GetKeyDown(KeyCode.R))
            {
                rotationAmount += 90;
                if (rotationAmount >= 360)
                {
                    rotationAmount = 0;
                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                HandlePlacement();
            }

            if (Input.GetMouseButtonDown(1))
            {
                StopBuilding();
            }
        }
    }

    private void HandlePlacement()
    {
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 0f, LayerMask.GetMask("Building"));
        if (hit || Physics2D.OverlapBox(placementGrid.GetCellCenterWorld(placementGrid.WorldToCell(worldPos)), (Vector2)selectedBuilding.transform.localScale, 0, LayerMask.GetMask("Building")))
        {
            return;
        }
        Building newBuilding = Instantiate(selectedBuilding, buildingFolder);
        newBuilding.transform.position = placementGrid.GetCellCenterWorld(placementGrid.WorldToCell(worldPos));
        newBuilding.transform.rotation = Quaternion.Euler(0,0,rotationAmount);
        newBuilding.player = player;
        newBuilding.GetComponent<SpriteRenderer>().color = Color.black;
        StopBuilding();
    }
    public void SelectBuilding(Building building)
    {
        if (!isBuilding)
        {
            selectedBuilding = building;
            CreatePlaceholderBuilding();
            isBuilding = true;   
        }
    }
    private void CreatePlaceholderBuilding()
    {
        placeholderBuilding = Instantiate(selectedBuilding.gameObject);
        placeholderBuilding.layer = LayerMask.GetMask("Ignore Raycast");
        placeholderBuilding.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.8f);
        foreach (MonoBehaviour script in placeholderBuilding.GetComponents<MonoBehaviour>())
        {
            Destroy(script);
        }
        createPlaceholder = false;
    }

    private void StopBuilding()
    {
        selectedBuilding = null;
        Destroy(placeholderBuilding);
        isBuilding = false;
    }
}
