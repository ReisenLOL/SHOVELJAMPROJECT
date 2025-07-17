using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class RespawnUIManager : MonoBehaviour
{
    //add ability to rename spawn points

    [Header("UI")] 
    public GameObject GameCanvas;
    public GameObject RespawnCanvas;
    public Button templateButton;
    public Button spawnButton;
    public Transform respawnListUI;

    [Header("CACHE")]
    public RespawnPoint[] respawnPoints;
    private RespawnPoint selectedRespawnPoint;
    private Vector3 camOriginPosition;
    private float currentState;
    private Camera cam;
    private CinemachineCamera cineMachineCam;
    public float cameraMovementSpeed;
    private void Start()
    {
        cam = Camera.main;
        cineMachineCam = FindFirstObjectByType<CinemachineCamera>();
    }

    public void HandlePlayerDeath()
    {
        foreach (Transform child in respawnListUI)
        {
            Destroy(child.gameObject);
        }
        GameCanvas.SetActive(false);
        RespawnCanvas.SetActive(true);
        cineMachineCam.enabled = false;
        spawnButton.gameObject.SetActive(false);
        camOriginPosition = cam.transform.position;
        respawnPoints = FindObjectsByType<RespawnPoint>(FindObjectsSortMode.None);
        if (respawnPoints.Length == 0)
        {
            FindFirstObjectByType<GameManager>().GameOver(false);
        }
        foreach (RespawnPoint respawnPoint in respawnPoints)
        {
            respawnPoint.CheckSpawnable();
            Button newRespawnButton = Instantiate(templateButton, respawnListUI);
            newRespawnButton.gameObject.SetActive(true);
            newRespawnButton.onClick.AddListener(() => SelectSpawnPoint(respawnPoint));
            if (respawnPoint.canRespawn)
            {
                //newRespawnButton.FindFirstObjectByType<Image>().color = Color.white;
            }
            else
            {
                
            }
        }
    }

    public void SelectSpawnPoint(RespawnPoint respawnPointToSelect)
    {
        camOriginPosition = cam.transform.position;
        currentState = 0f;
        selectedRespawnPoint = respawnPointToSelect;
        StartCoroutine(MoveCamera());
        if (respawnPointToSelect.canRespawn)
        {
            spawnButton.gameObject.SetActive(true);
        }
        else
        {
            spawnButton.gameObject.SetActive(false);
        }
    }

    public void DoRespawn()
    {
        GameCanvas.SetActive(true);
        RespawnCanvas.SetActive(false);
        cineMachineCam.enabled = true;
        selectedRespawnPoint.RespawnPlayer();
    }
    private IEnumerator MoveCamera()
    {
        while (currentState < 1)
        {
            currentState += Time.deltaTime * cameraMovementSpeed;
            cam.transform.position = Vector3.Lerp(new Vector3(camOriginPosition.x, camOriginPosition.y, -10), new Vector3(selectedRespawnPoint.transform.position.x, selectedRespawnPoint.transform.position.y, -10), currentState);
            yield return null;   
        }
    }
}
