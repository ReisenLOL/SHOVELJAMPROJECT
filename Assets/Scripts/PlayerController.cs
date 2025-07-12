using System;
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

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        moveDirection.x = Input.GetAxisRaw("Horizontal");
        moveDirection.y = Input.GetAxisRaw("Vertical");
        moveDirection.Normalize();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * moveSpeed;
    }

    protected override void OnKill()
    {
        Debug.Log("playerKilled");
    }
}
