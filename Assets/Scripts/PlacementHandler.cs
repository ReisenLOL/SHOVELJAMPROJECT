using System;
using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlacementHandler : MonoBehaviour
{
    public Building[] allBuildings;
    public enum BuildingCategory {Pipe, Flux_Generation, Storage, Turret, Defense}

    [Header("[PLACEMENT CACHE]")]
    public PlayerController player;
    private Vector3 worldPos;
    private Camera cam;
    private Vector3 buildingGridEvenCell;
    private Vector3 buildingGridCenterCell;
    public Building selectedBuilding;
    public GameObject placeholderBuilding;
    private Grid placementGrid;
    public Transform buildingFolder;
    private int rotationAmount;
    private float selectionSize;
    public BoxCollider2D selectionBox;
    public bool isBuilding;
    private Vector3Int pipeStartCell;
    private Vector3 startingPipeWorldPos;
    private bool isDraggingPipe;
    public GameObject showPlaceholderLine;
    private GameObject currentPlaceholderLine;
    [Header("[UI CACHE]")]
    private BuildingCategory currentCategoryOpen;
    public List<Building> placedBuildings = new();
    public Transform buildingUIFrame;
    public Button buttonTemplate;
    public Button categoryButtonTemplate;
    public Transform categoryUIFrame;

    private void Start()
    {
        placementGrid = FindAnyObjectByType<Grid>();
        player = FindFirstObjectByType<PlayerController>();
        cam = Camera.main;
        foreach (BuildingCategory category in Enum.GetValues(typeof(BuildingCategory)))
        {
            Button newButton = Instantiate(categoryButtonTemplate, categoryUIFrame);
            newButton.gameObject.SetActive(true);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = category.ToString().Replace("_", " ");
            newButton.onClick.AddListener(() => SelectCategory(category));
            
        }
        placedBuildings = FindObjectsByType<Building>(FindObjectsSortMode.None).ToList();
        foreach (Building building in placedBuildings)
        {
            building.player = player;
        }
    }
    private void Update()
    {
        PlaceBuildingBlueprint();
    }

    public void SelectCategory(BuildingCategory categorySelected)
    {
        if (!buildingUIFrame.gameObject.activeSelf || currentCategoryOpen != categorySelected)
        {
            foreach (Transform buildingButtons in buildingUIFrame)
            {
                Destroy(buildingButtons.gameObject);
            }
            buildingUIFrame.gameObject.SetActive(true);
            currentCategoryOpen = categorySelected;
            foreach (Building building in allBuildings)
            {
                if (building.buildingCategory == categorySelected)
                {
                    BuildBuildingList(building);
                }
            }
        }
        else
        {
            buildingUIFrame.gameObject.SetActive(false);
        }

    }

    private void BuildBuildingList(Building building)
    {
        Button newButton = Instantiate(buttonTemplate, buildingUIFrame);
        newButton.gameObject.SetActive(true);
        newButton.onClick.AddListener(() => SelectBuilding(building));
        newButton.GetComponentInChildren<TextMeshProUGUI>().text = building.fluxCost.ToString();
        newButton.transform.Find("BuildingSprite").GetComponent<Image>().sprite = building.GetComponent<SpriteRenderer>().sprite;
    }
    private void PlaceBuildingBlueprint()
    {
        if (isBuilding)
        {
            player.canFire = false;
            worldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0;
            buildingGridEvenCell = placementGrid.WorldToCell(worldPos);
            buildingGridCenterCell = placementGrid.GetCellCenterWorld(placementGrid.WorldToCell(worldPos));
            if (selectionSize % 2 == 0)
            {
                placeholderBuilding.transform.position = buildingGridEvenCell;
            }
            else
            {
                placeholderBuilding.transform.position = buildingGridCenterCell;
            }
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
                if (selectedBuilding.draggable)
                {
                    pipeStartCell = placementGrid.WorldToCell(worldPos);
                    currentPlaceholderLine = Instantiate(showPlaceholderLine);
                    currentPlaceholderLine.transform.position = worldPos;
                    startingPipeWorldPos = worldPos;
                    isDraggingPipe = true;
                }
                else
                {
                    HandlePlacement();   
                }
            }
            else if (Input.GetMouseButton(0) && isDraggingPipe)
            {
                currentPlaceholderLine.transform.Lookat2D(worldPos);
                currentPlaceholderLine.transform.position = (placementGrid.GetCellCenterWorld(pipeStartCell) + (worldPos - pipeStartCell) / 2) - new Vector3(0,0.25f,0);
                currentPlaceholderLine.transform.localScale = new Vector3(Vector3.Distance(placementGrid.GetCellCenterWorld(pipeStartCell), worldPos), 1, 1);
            }
            else if (Input.GetMouseButtonUp(0) && isDraggingPipe)
            {
                isDraggingPipe = false;
                Vector3Int endCell = placementGrid.WorldToCell(worldPos);
                if (endCell == pipeStartCell)
                {
                    HandlePlacement();
                    Destroy(currentPlaceholderLine);
                    return;
                }
                PlacePipeLine(pipeStartCell, endCell);
                Destroy(currentPlaceholderLine);
                StopBuilding();
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
        if (hit || Physics2D.OverlapBox(placementGrid.GetCellCenterWorld(placementGrid.WorldToCell(worldPos)), selectionBox.size/2, 0, LayerMask.GetMask("Building")))
        {
            return;
        }
        Building newBuilding = Instantiate(selectedBuilding, buildingFolder);
        if (selectionSize % 2 == 0)
        {
            newBuilding.transform.position = buildingGridEvenCell;
        }
        else
        {
            newBuilding.transform.position = buildingGridCenterCell;
        }
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
            selectionBox = selectedBuilding.GetComponent<BoxCollider2D>();
            selectionSize = selectionBox.size.x;
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
    }
    private void PlacePipeLine(Vector3Int start, Vector3Int end)
    {
        Vector3Int current = start;

        while (current.x != end.x)
        {
            current.x += Math.Sign(end.x - current.x);
            TryPlacePipeAt(current);
        }

        while (current.y != end.y)
        {
            current.y += Math.Sign(end.y - current.y);
            TryPlacePipeAt(current);
        }
    }

    private void TryPlacePipeAt(Vector3Int cell)
    {
        Vector3 pos = placementGrid.GetCellCenterWorld(cell);
        Collider2D hit = Physics2D.OverlapBox(pos, selectionBox.size / 2, 0, LayerMask.GetMask("Building"));
        if (hit != null)
        {
            return;
        }
        Building pipe = Instantiate(selectedBuilding, buildingFolder);
        pipe.transform.rotation = Quaternion.Euler(0,0,rotationAmount);
        pipe.transform.position = pos;
        pipe.player = player;
        pipe.GetComponent<SpriteRenderer>().color = Color.black;
    }
    private void StopBuilding()
    {
        selectedBuilding = null;
        player.canFire = true;
        Destroy(placeholderBuilding);
        isBuilding = false;
    }
}
