using System;
using UnityEngine;

public class DistributorRange : MonoBehaviour
{
    public FluxDistributor thisTower;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!thisTower.playerInRange)
        {
            thisTower.CreateBeam();   
            thisTower.playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        thisTower.playerInRange = false;
        Destroy(thisTower.createdBeam);
    }
}
