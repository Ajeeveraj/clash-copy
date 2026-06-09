using UnityEngine;
using UnityEngine.AI;

public class ArcherAttack : MonoBehaviour
{
    [Header("Targeting")]
    public bool isEnemy = false;

    public float damage = 10f;

    [Header("Combat Stats")]
    public float attackRate = 1.5f;
    public float attackRange = 7.0f; // Much larger range so they stop early

    [Header("Ranged Setup")]
    public GameObject arrowPrefab;
    public Transform arrowSpawnPoint; // An empty GameObject placed near the Archer's bow

    private float nextAttackTime;
    private Transform currentTarget;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.stoppingDistance = attackRange - 0.2f; 
        }
    }

    void Update()
    {
        FindNearestTarget();

        if (currentTarget != null)
        {
            float distance = Vector3.Distance(transform.position, currentTarget.position);

            if (distance <= attackRange)
            {
                // Make the archer turn to face the target before shooting
                transform.LookAt(currentTarget);

                if (Time.time >= nextAttackTime)
                {
                    ShootArrow();
                    nextAttackTime = Time.time + attackRate;
                }
            }
        }
    }

    void ShootArrow()
    {
        if (arrowPrefab != null && arrowSpawnPoint != null)
        {
            // Spawns the arrow exactly where the bow is, facing the same direction as the Archer
            Instantiate(arrowPrefab, arrowSpawnPoint.position, transform.rotation);
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
        else
        {
            currentTarget = null;
        }
    }
}