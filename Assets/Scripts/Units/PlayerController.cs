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

    [Header("[PROJECTILE]")] 
    public bool canFire;
    public Projectile projectile;
    public float projectileSpeed;
    public float projectileDamage;
    public float fireRate;
    public float fireFluxCost;
    public float currentFiringTime;
    public AudioClip fireSound;

    [Header("[DESTROY BEAM PROJECTILE]")] 
    public DestroyBeam destroyBeam;
    public AudioClip destroyBeamFire;
    
    [Header("[CACHE]")] 
    public Transform fluxStoredBar;
    public TextMeshProUGUI fluxStoredText;
    public CinemachineCamera cam;
    public Camera gameCam;
    private RespawnUIManager respawnUIManager;
    public Transform healthBar;
    public AudioSource audioSource;
    private void Start()
    {
        gameCam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        respawnUIManager = FindFirstObjectByType<RespawnUIManager>();
        UpdateFluxBar();
    }

    private void Update()
    {
        moveDirection.x = Input.GetAxisRaw("Horizontal");
        moveDirection.y = Input.GetAxisRaw("Vertical");
        moveDirection.Normalize();
        CameraZoom();
        FireProjectile();
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
        if (fluxStored > flux && fluxStored > 0)
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
        respawnUIManager.HandlePlayerDeath();
        gameObject.SetActive(false);
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

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        UpdateHealthBar();
    }

    private void OnEnable()
    {
        UpdateHealthBar();
    }

    private void FireProjectile()
    {
        currentFiringTime += Time.deltaTime;
        if (canFire && Input.GetMouseButton(0) && currentFiringTime >= fireRate && fluxStored >= fireFluxCost)
        {
            CreateProjectile(projectile, projectileSpeed, projectileDamage, fireFluxCost);
            audioSource.PlayOneShot(fireSound, 0.4f);
        }
        else if (canFire && Input.GetKeyDown(KeyCode.G))
        {
            CreateProjectile(destroyBeam, 12f, 0f, 0f);
            audioSource.PlayOneShot(destroyBeamFire);
        }
    }

    private void CreateProjectile(Projectile projectileToCreate, float speed, float damage, float cost)
    {
        Vector3 worldPos = gameCam.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0;
        Projectile newProjectile = Instantiate(projectileToCreate);
        newProjectile.transform.position = transform.position;
        newProjectile.speed = speed;
        newProjectile.damage = damage;
        newProjectile.RotateToTarget(worldPos);
        DrainFlux(cost);
        currentFiringTime = 0;
    }

    private void UpdateHealthBar()
    {
        healthBar.localScale = new Vector3(healthBar.localScale.x, health/maxHealth);
    }
}
