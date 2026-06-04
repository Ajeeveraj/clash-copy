using UnityEngine;

public class TroopAttack : MonoBehaviour
{
    public bool isEnemy;
    public Transform currentTarget;
    public float attackRange = 1.5f;

    [Header("Settings")]
    [SerializeField] private float attackCooldown = 1.0f;
    [SerializeField] private float detectionRadius = 5.0f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private int damage = 10;

    private float nextAttackTime;
    private string enemyTroopTag;
    private string enemyTowerTag;

    void Start()
    {
        enemyTroopTag = isEnemy ? "Troop" : "EnemyTroop";
        enemyTowerTag = isEnemy ? "PlayerTower" : "EnemyTower";
    }

    void Update()
    {
        // 1. If target exists, check if it's dead
        if (currentTarget != null)
        {
            if (IsTargetDead(currentTarget)) currentTarget = null;
        }

        // 2. If no target, search
        if (currentTarget == null)
        {
            LookForNearbyEnemies();
            if (currentTarget == null) FindClosestTower();
        }

        // 3. Attack if in range
        if (currentTarget != null && Vector3.Distance(transform.position, currentTarget.position) <= attackRange)
        {
            if (Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    private bool IsTargetDead(Transform t)
    {
        if (t.TryGetComponent<TroopHealth>(out var th)) return th.isDead;
        if (t.TryGetComponent<TowerHealth>(out var toh)) return toh.isDestroyed;
        return false;
    }

    private void LookForNearbyEnemies()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, targetLayer);
        Transform closest = null;
        float dist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            if (hit.gameObject.layer == gameObject.layer) continue;
            if (hit.CompareTag(enemyTroopTag))
            {
                float d = Vector3.Distance(transform.position, hit.transform.position);
                if (d < dist) { dist = d; closest = hit.transform; }
            }
        }
        currentTarget = closest;
    }

    private void FindClosestTower()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag(enemyTowerTag);
        Transform closest = null;
        float dist = Mathf.Infinity;

        foreach (var t in towers)
        {
            if (t.TryGetComponent<TowerHealth>(out var th) && th.isDestroyed) continue;
            float d = Vector3.Distance(transform.position, t.transform.position);
            if (d < dist) { dist = d; closest = t.transform; }
        }
        currentTarget = closest;
    }

    private void Attack()
    {
        if (currentTarget.TryGetComponent<TroopHealth>(out var th)) th.TakeDamage(damage);
        else if (currentTarget.TryGetComponent<TowerHealth>(out var toh)) toh.TakeDamage(damage);
    }
}