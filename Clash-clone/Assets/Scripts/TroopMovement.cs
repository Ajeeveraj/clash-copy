using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
public class TroopMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public string enemyTowerTag = "EnemyTower";

    private NavMeshAgent agent;
    private Transform targetTower;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError($"{name}: No NavMeshAgent component found! Add one.");
            return;
        }

        agent.speed = moveSpeed;
        agent.stoppingDistance = 1.2f;

        GameObject towerObj = GameObject.FindWithTag(enemyTowerTag);
        if (towerObj != null)
        {
            targetTower = towerObj.transform;
            agent.SetDestination(targetTower.position);
        }
        else
        {
            Debug.LogError("No EnemyTower found in scene!");
        }
    }

    void Update()
    {
        if (agent == null || targetTower == null) return;

        // Face movement direction
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Vector3 dir = agent.velocity;
            dir.y = 0;
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }
}




