using System;
using UnityEngine;

public class EnemyDetectTargets : MonoBehaviour
{
    public EnemyController thisEnemy;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!thisEnemy.targetList.Contains(other))
        {
            thisEnemy.targetList.Add(other);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (thisEnemy.targetList.Contains(other))
        {
            thisEnemy.targetList.Remove(other);
        }
    }
}
