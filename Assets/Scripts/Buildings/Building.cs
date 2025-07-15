using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Building : MonoBehaviour
{
    public float health;
    public float maxHealth;
    public float fluxCost;
    public string buildingID;
    public string description;
    public bool refreshBuildings;
    private float damageColorChangeSpeed = 4f;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public PlayerController player;
    [HideInInspector] public float currentState;

    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        spriteRenderer.color = Color.red;
        currentState = 0;
        StartCoroutine(DamageAnimation());
        if (health <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator DamageAnimation()
    {
        while (currentState < 1)
        {
            currentState += Time.deltaTime * damageColorChangeSpeed;
            spriteRenderer.color = Color.Lerp(Color.red, Color.white, currentState);
            yield return null;
        }
    }
}
