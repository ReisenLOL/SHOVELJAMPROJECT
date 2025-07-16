using Unity.Cinemachine;
using UnityEngine;

public class RespawnPoint : FluxStorable
{
    public float requiredFluxToRespawn;
    public bool canRespawn;

    public void RespawnPlayer()
    {
        if (canRespawn)
        {
            player.gameObject.SetActive(true);
            player.transform.position = transform.position;  
            player.health = player.maxHealth;
            connectedGrid.DrainFluxTotal(requiredFluxToRespawn);
        }
    }

    public void CheckSpawnable()
    {
        if (connectedGrid && connectedGrid.currentFluxStoredTotal >= requiredFluxToRespawn)
        {
            canRespawn = true;
        }
    }
}
