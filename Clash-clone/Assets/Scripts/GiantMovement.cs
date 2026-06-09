using UnityEngine;
using UnityEngine.AI;

public class GiantMovement : MonoBehaviour
{
    [Header("Targeting")]
    public bool isEnemy = false;
    public float searchRate = 0.5f;

    private NavMeshAgent agent;
    private Transform targetTower;
    private float nextSearchTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (Time.time >= nextSearchTime)
        {
            FindNearestTower();
            nextSearchTime = Time.time + searchRate;
        }

        if (targetTower != null && agent.isActiveAndEnabled)
        {
            agent.SetDestination(targetTower.position);
        }
    }

    void FindNearestTower()
    {
        // Automatically assigns the correct tag based on the checkbox
        string targetTag = isEnemy ? "PlayerTower" : "EnemyTower";

        GameObject[] towers = GameObject.FindGameObjectsWithTag(targetTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestTower = null;

        foreach (GameObject tower in towers)
        {
            float distanceToTower = Vector3.Distance(transform.position, tower.transform.position);
            if (distanceToTower < shortestDistance)
            {
                shortestDistance = distanceToTower;
                nearestTower = tower;
            }
        }

        if (nearestTower != null)
        {
            targetTower = nearestTower.transform;
        }
    }
}