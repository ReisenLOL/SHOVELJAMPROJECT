using System;
using UnityEngine;

public class Building : MonoBehaviour
{
    public float health;
    public float maxHealth;
    public float fluxCost;
    [HideInInspector] public PlayerController player;
    public string buildingID;
    public bool refreshBuildings;

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    protected virtual void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            gameObject.SetActive(false);
        }
    }
}
