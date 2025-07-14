using System;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerController : Unit
{
    private Vector3 moveDirection;
    private Rigidbody2D rb;
    
    [Header("[STATS]")] 
    public float moveSpeed;

    public float fluxMoveSpeed;
    public float fluxStored;
    public float fluxMaxCapacity;
    public bool atMaxCapacity;

    [Header("[CACHE]")] 
    public Transform fluxStoredBar;
    public TextMeshProUGUI fluxStoredText;
    public CinemachineCamera cam;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        UpdateFluxBar();
    }

    private void Update()
    {
        moveDirection.x = Input.GetAxisRaw("Horizontal");
        moveDirection.y = Input.GetAxisRaw("Vertical");
        moveDirection.Normalize();
        CameraZoom();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * moveSpeed;
    }
    public void StoreFlux(float flux)
    {
        if (!atMaxCapacity)
        {
            fluxStored += flux;
            if (fluxStored >= fluxMaxCapacity)
            {
                atMaxCapacity = true;
                fluxStored = fluxMaxCapacity;
            }   
            UpdateFluxBar();
        }
    }
    public void DrainFlux(float flux)
    {
        if (fluxStored > flux)
        {
            fluxStored -= flux;
            if (atMaxCapacity)
            {
                atMaxCapacity = false;
            }
            UpdateFluxBar();
        }
    }
    private void UpdateFluxBar()
    {
        fluxStoredBar.localScale = new Vector3(fluxStoredBar.localScale.x, fluxStored/fluxMaxCapacity, 0);
        fluxStoredText.text = MathF.Round(fluxStored) + "/" + MathF.Round(fluxMaxCapacity);
    }
    protected override void OnKill()
    {
        Debug.Log("playerKilled");
    }

    private void CameraZoom()
    {
        if (Input.mouseScrollDelta.y != 0f)
        {
            cam.Lens.OrthographicSize -= Input.mouseScrollDelta.y / 1.5f;
            if (cam.Lens.OrthographicSize < 0f)
            {
                cam.Lens.OrthographicSize = 1f;
            }
        }
    }
}
