using UnityEngine;

public class RepulsionRangeHandler : MonoBehaviour
{
    public RepulsionGenerator thisGenerator;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Rigidbody2D hasRigidBody) && !thisGenerator.targetList.Contains(hasRigidBody))
        {
            thisGenerator.targetList.Add(hasRigidBody);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out Rigidbody2D hasRigidBody) && thisGenerator.targetList.Contains(hasRigidBody))
        {
            thisGenerator.targetList.Remove(hasRigidBody);
        }
    }
}
