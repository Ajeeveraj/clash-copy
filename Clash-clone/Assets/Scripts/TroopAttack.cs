using UnityEngine;
using UnityEngine.AI;

public class TroopAttack : MonoBehaviour
{
    public bool isEnemy;
    public Transform currentTarget;

    [Header("Combat Settings")]
    public float attackRange = 1.0f;
    [SerializeField] private float attackCooldown = 1.0f;
    [SerializeField] private int damage = 50;
    [SerializeField] public float detectionRadius = 8.0f; 

    private float nextAttackTime;
    private string enemyTroopTag;
    private string enemyTowerTag;
    private NavMeshAgent myAgent;

    void Start()
    {
        enemyTroopTag = isEnemy ? "Troop" : "EnemyTroop";
        enemyTowerTag = isEnemy ? "PlayerTower" : "EnemyTower";
        myAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // 1. Instantly clear dead targets
        if (currentTarget != null && IsTargetDead(currentTarget))
        {
            currentTarget = null;
        }

        // 2. Target Selection & Priority (THE REBELS VS BUILDINGS FIX)
        Transform nearbyTroop = LookForNearbyEnemies();

        if (nearbyTroop != null)
        {
            // Enemy troops inside our vision radius ALWAYS take priority over static structures
            currentTarget = nearbyTroop;
        }
        else if (currentTarget == null || currentTarget.CompareTag("PlayerTower") || currentTarget.CompareTag("EnemyTower"))
        {
            // If no enemy troops are around, lock back onto the closest tower
            currentTarget = FindClosestTower();
        }

        // 3. Combat Execution using EDGE-TO-EDGE distance
        if (currentTarget != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);

            if (distanceToTarget <= GetActualAttackRange(currentTarget))
            {
                if (Time.time >= nextAttackTime)
                {
                    Attack();
                    nextAttackTime = Time.time + attackCooldown;
                }
            }
        }
    }

    public float GetActualAttackRange(Transform target)
    {
        float actualRange = attackRange;
        if (myAgent != null) actualRange += myAgent.radius;

        if (target != null)
        {
            if (target.TryGetComponent<NavMeshAgent>(out var targetAgent)) actualRange += targetAgent.radius;
            else if (target.TryGetComponent<NavMeshObstacle>(out var obs)) actualRange += (obs.radius > 0 ? obs.radius : 1.5f);
            else actualRange += 1.0f;
        }
        return actualRange;
    }

    private bool IsTargetDead(Transform t)
    {
        if (t == null) return true;
        if (t.TryGetComponent<TroopHealth>(out var th)) return th.isDead;
        if (t.TryGetComponent<TowerHealth>(out var toh)) return toh.isDestroyed;
        return true;
    }

    private Transform LookForNearbyEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTroopTag);
        Transform closest = null;
        float dist = Mathf.Infinity;

        foreach (var e in enemies)
        {
            if (e == null) continue;
            if (e.TryGetComponent<TroopHealth>(out var th) && th.isDead) continue;
            if (th != null && th.isFlying) continue;

            float d = Vector3.Distance(transform.position, e.transform.position);
            if (d <= detectionRadius && d < dist) { dist = d; closest = e.transform; }
        }
        return closest;
    }

    private Transform FindClosestTower()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag(enemyTowerTag);
        Transform closest = null;
        float dist = Mathf.Infinity;

        foreach (var t in towers)
        {
            if (t == null) continue;
            if (t.TryGetComponent<TowerHealth>(out var th) && th.isDestroyed) continue;
            float d = Vector3.Distance(transform.position, t.transform.position);
            if (d < dist) { dist = d; closest = t.transform; }
        }
        return closest;
    }

    private void Attack()
    {
        if (currentTarget == null) return;
        if (currentTarget.TryGetComponent<TroopHealth>(out var th)) th.TakeDamage(damage);
        else if (currentTarget.TryGetComponent<TowerHealth>(out var toh)) toh.TakeDamage(damage);
    }
}