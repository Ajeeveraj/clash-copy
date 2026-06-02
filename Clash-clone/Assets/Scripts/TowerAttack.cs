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
        // Find a target if we don't have one, or if it walks out of range
        if (currentTarget == null || Vector3.Distance(transform.position, currentTarget.position) > attackRange)
        {
            FindNearestValidTarget();
        }

        // Aim and fire at the target
        if (currentTarget != null)
        {
            Vector3 targetDir = currentTarget.position - transform.position;
            targetDir.y = 0; 
            
            if (targetDir != Vector3.zero)
            {
                // Forces the front of the tower asset to face the incoming troop
                Quaternion targetRotation = Quaternion.LookRotation(-targetDir); 
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }

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
            // Player towers ONLY target enemy units
            targetTag = ENEMY_TROOP_TAG;
        }
        else if (isEnemyTower)
        {
            // Enemy towers ONLY target player units
            targetTag = PLAYER_TROOP_TAG;
        }
        else
        {
            return;
        }

        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
        float closestDistance = Mathf.Infinity;
        GameObject closestTroop = null;

        foreach (GameObject troop in targets)
        {
            if (troop != null)
            {
                TroopHealth th = troop.GetComponentInParent<TroopHealth>();
                if (th != null && th.isDead) continue;

                float distance = Vector3.Distance(transform.position, troop.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTroop = troop;
                }
            }
        }

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