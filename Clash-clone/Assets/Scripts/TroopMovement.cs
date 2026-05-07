using UnityEngine;

public class TroopMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Transform targetTower;

    void Start()
    {
        // Automatically find the Princess Tower
        GameObject towerObj = GameObject.FindWithTag("PrincessTower");

        if (towerObj != null)
        {
            targetTower = towerObj.transform;
        }
        else
        {
            Debug.LogError("No PrincessTower found in scene!");
        }
    }

    void Update()
    {
        if (targetTower == null)
            return;

        // Move toward tower
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetTower.position,
            moveSpeed * Time.deltaTime
        );

        // Optional: face the tower
        Vector3 dir = targetTower.position - transform.position;
        dir.y = 0;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
    }
}




