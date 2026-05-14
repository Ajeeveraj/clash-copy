using UnityEngine;

public class TroopMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Transform targetTower;

    void Start()
    {
        // CHANGE THIS depending on troop side:
        GameObject towerObj = GameObject.FindWithTag("EnemyTower");
        

        if (towerObj != null)
        {
            targetTower = towerObj.transform;
        }
        else
        {
            Debug.LogError("No EnemyPrincessTower found in scene!");
        }
    }

    void Update()
    {
        if (targetTower == null)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetTower.position,
            moveSpeed * Time.deltaTime
        );

        // Face tower
        Vector3 dir = targetTower.position - transform.position;
        dir.y = 0;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
    }
}




