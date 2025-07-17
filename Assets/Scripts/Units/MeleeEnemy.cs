using System;
using UnityEngine;

public class MeleeEnemy : EnemyController
{
    private bool canSlash;
    protected override void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.enabled = true;
        agent.speed = speed;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        player = FindFirstObjectByType<PlayerController>();
        core = FindFirstObjectByType<CoreController>();
    }
    protected override void Update()
    {
        Movement();
        GetClosestTarget();
        SlashTimer();
    }

    private void SlashTimer()
    {
        currentFiringTime += Time.deltaTime;
        if (currentFiringTime > fireRate)
        {
            canSlash = true;
            currentFiringTime = 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent(out Building isBuilding))
        {
            if (canSlash)
            {
                isBuilding.TakeDamage(projectileDamage);
                audioSource.PlayOneShot(fireSound, volume);
                canSlash = false;
            }

        }
        else if (other.gameObject.TryGetComponent(out PlayerController isPlayer))
        {
            if (canSlash)
            {
                isPlayer.TakeDamage(projectileDamage);
                audioSource.PlayOneShot(fireSound, volume);
                canSlash = false;
            }
        }
    }
}
