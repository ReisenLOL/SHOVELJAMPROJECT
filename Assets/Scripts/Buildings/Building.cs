using UnityEngine;

public class Building : MonoBehaviour
{
    public float health;
    public float maxHealth;
    public float fluxCost;
    [HideInInspector] public PlayerController player;
    public string buildingID;

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
