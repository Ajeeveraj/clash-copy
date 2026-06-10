using UnityEngine;
using UnityEngine.AI; // Required to freeze the NavMeshAgent

public class ArcherAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 5f;
    public float attackCooldown = 1.2f;
    public int damage = 20;
    public bool isEnemy = false; // Check this in the Inspector if it's an enemy archer

    [Header("References")]
    public GameObject arrowPrefab;
    public Transform arrowSpawnPoint;

    private NavMeshAgent agent;
    private Transform currentTarget;
    private float nextAttackTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        FindNearestTarget();

        // If we have a target and it is within our attack range
        if (currentTarget != null && Vector3.Distance(transform.position, currentTarget.position) <= attackRange)
        {
            // 1. Plant feet and stop moving (Clash Royale style)
            agent.isStopped = true;

            // 2. Rotate to face the target
            FaceTarget();

            // 3. Handle weapon cooldown and shooting
            if (Time.time >= nextAttackTime)
            {
                ShootArrow();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
        else
        {
            // No target in range? Resume walking down the lane
            if (agent != null)
            {
                agent.isStopped = false;
            }
        }
    }

    void FindNearestTarget()
    {
        string towerTag = isEnemy ? "PlayerTower" : "EnemyTower";
        string troopTag = isEnemy ? "Troop" : "EnemyTroop";

        GameObject[] towers = GameObject.FindGameObjectsWithTag(towerTag);
        GameObject[] troops = GameObject.FindGameObjectsWithTag(troopTag);
        
        float shortestDistance = Mathf.Infinity;
        GameObject nearestTarget = null;

        // Check towers
        foreach (GameObject target in towers)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestTarget = target;
            }
        }

        // Check troops
        foreach (GameObject target in troops)
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

    void FaceTarget()
    {
        Vector3 direction = (currentTarget.position - transform.position).normalized;
        direction.y = 0; // Lock Y axis so the archer doesn't tilt up or down awkwardly
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void ShootArrow()
    {
        if (arrowPrefab != null && arrowSpawnPoint != null && currentTarget != null)
        {
            GameObject newArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, transform.rotation);
            
            ProjectileFlight flightScript = newArrow.GetComponent<ProjectileFlight>();
            if (flightScript != null)
            {
                flightScript.damage = this.damage; 
                flightScript.target = this.currentTarget; 
                
            }
        }
    }

    // Visualizes the attack range circle in the Unity Scene View
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}