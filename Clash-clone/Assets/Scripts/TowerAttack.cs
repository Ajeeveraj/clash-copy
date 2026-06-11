using UnityEngine;

public class TowerAttack : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject projectilePrefab; 
    public Transform firePoint;         
    public float attackRange = 7f;
    public float fireRate = 1f;
    private float nextFireTime = 0f;

    [Header("Team Setup")]
    [Tooltip("Check this box if this tower belongs to the PLAYER.")]
    public bool isPlayerTower = false;

    [Tooltip("Check this box if this tower belongs to the ENEMY AI.")]
    public bool isEnemyTower = false;
    
    private Transform currentTarget;

    // Hardcoded team tags to guarantee no inspector mistakes
    private const string PLAYER_TROOP_TAG = "Troop";
    private const string ENEMY_TROOP_TAG = "EnemyTroop";

    void Update()
    {
        // FIX: Calculate target distance on a flat horizontal plane
        float distanceToTarget = Mathf.Infinity;
        if (currentTarget != null)
        {
            Vector3 myFlatPos = new Vector3(transform.position.x, 0f, transform.position.z);
            Vector3 targetFlatPos = new Vector3(currentTarget.position.x, 0f, currentTarget.position.z);
            distanceToTarget = Vector3.Distance(myFlatPos, targetFlatPos);
        }

        // Find a target if we don't have one, or if it walks out of flat range
        if (currentTarget == null || distanceToTarget > attackRange)
        {
            FindNearestValidTarget();
        }

        // Aim and fire at the target
        if (currentTarget != null)
        {
            Vector3 targetDir = currentTarget.position - transform.position;
            targetDir.y = 0; 
            
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + 1f / fireRate;
            }
        }
    }

    void FindNearestValidTarget()
    {
        string targetTag = "";

        if (isPlayerTower)
        {
            targetTag = ENEMY_TROOP_TAG;
        }
        else if (isEnemyTower)
        {
            targetTag = PLAYER_TROOP_TAG;
        }
        else
        {
            return;
        }

        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
        float closestDistance = Mathf.Infinity;
        GameObject closestTroop = null;

        Vector3 myFlatPos = new Vector3(transform.position.x, 0f, transform.position.z);

        foreach (GameObject troop in targets)
        {
            if (troop != null)
            {
                TroopHealth th = troop.GetComponentInParent<TroopHealth>();
                if (th != null && th.isDead) continue;

                // FIX: Flatten position calculation to prevent flying units from bypassing the range check
                Vector3 troopFlatPos = new Vector3(troop.transform.position.x, 0f, troop.transform.position.z);
                float distance = Vector3.Distance(myFlatPos, troopFlatPos);
                
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTroop = troop;
                }
            }
        }

        // closestDistance is already flat here, so it accurately reflects ground positioning
        if (closestTroop != null && closestDistance <= attackRange)
        {
            currentTarget = closestTroop.transform;
        }
        else
        {
            currentTarget = null;
        }
    }

    void Shoot()
    {
        if (projectilePrefab != null && firePoint != null && currentTarget != null)
        {
            GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            
            Projectile projectileScript = proj.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.SetTarget(currentTarget);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (isPlayerTower) Gizmos.color = Color.blue;
        else if (isEnemyTower) Gizmos.color = Color.red;
        else Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}