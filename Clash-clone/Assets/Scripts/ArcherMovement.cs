using UnityEngine;
using UnityEngine.AI;

public class ArcherMovement : MonoBehaviour
{
    [Header("Targeting")]
    public bool isEnemy = false;
    public float searchRate = 0.5f;

    private NavMeshAgent agent;
    private Transform currentTarget;
    private float nextSearchTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (Time.time >= nextSearchTime)
        {
            FindNearestTarget();
            nextSearchTime = Time.time + searchRate;
        }

        if (currentTarget != null && agent.isActiveAndEnabled)
        {
            agent.SetDestination(currentTarget.position);
        }
    }

    void FindNearestTarget()
    {
        string targetTag = isEnemy ? "PlayerTower" : "EnemyTower";
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
        
        float shortestDistance = Mathf.Infinity;
        GameObject nearestTarget = null;

        foreach (GameObject target in targets)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestTarget = target;
            }
        }

        if (nearestTarget != null)
        {
            currentTarget = nearestTarget.transform;
        }
    }
}