using UnityEngine;

public class TurretTargetting : MonoBehaviour
{
    public TurretController thisTurret;
    private void OnTriggerEnter2D(Collider2D other)
    {
        thisTurret.targetList.Add(other);
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (thisTurret.targetList.Contains(other))
        {
            thisTurret.targetList.Remove(other);
        }
    }
}
