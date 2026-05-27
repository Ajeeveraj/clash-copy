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
        if (agent == null) { Debug.LogError("No NavMeshAgent!"); return; }

        agent.speed = moveSpeed;
        agent.stoppingDistance = 1.2f;

        FindClosestTower();
    }

    void FindClosestTower()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag(enemyTowerTag);
        float closestDist = Mathf.Infinity;
        targetTower = null;

        foreach (GameObject tower in towers)
        {
            TowerHealth th = tower.GetComponent<TowerHealth>();
            if (th != null && th.isDestroyed) continue;

            float dist = Vector3.Distance(transform.position, tower.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                targetTower = tower.transform;
            }
        }

        if (targetTower != null)
            agent.SetDestination(targetTower.position);
        else
            Debug.LogError("No enemy tower found!");
    }

    void Update()
    {
        if (agent == null || targetTower == null) return;

        TowerHealth th = targetTower.GetComponent<TowerHealth>();
        if (th != null && th.isDestroyed) FindClosestTower();

        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Vector3 dir = agent.velocity;
            dir.y = 0;
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }
}



