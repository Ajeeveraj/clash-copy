using UnityEngine;

public class TroopMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;

    [Header("Tower References")]
    public Transform leftPrincessTower;
    public Transform rightPrincessTower;

    private Transform targetTower;

    void Start()
    {
        // Decide lane based on X position
        if (transform.position.x < 0)
            targetTower = leftPrincessTower;
        else
            targetTower = rightPrincessTower;
    }

    void Update()
    {
        if (targetTower == null) return;

        // Move toward the chosen tower
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetTower.position,
            moveSpeed * Time.deltaTime
        );

        // Rotate to face the tower
        Vector3 direction = targetTower.position - transform.position;
        direction.y = 0;

        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);
    }

    public Transform GetTargetTower()
    {
        return targetTower;
    }

    public void StopMoving()
    {
        moveSpeed = 0f;
    }
}



