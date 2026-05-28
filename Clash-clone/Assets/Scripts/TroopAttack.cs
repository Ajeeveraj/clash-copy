using UnityEngine;

public class TroopAttack : MonoBehaviour
{
    public int damage = 40;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public string enemyTowerTag = "EnemyTower";

    private float cooldownTimer = 0f;
    private Transform targetTower;
    private TowerHealth targetHealth;

    void Start()
    {
        // Find the initial target when spawning
        FindClosestTower();
    }

    void Update()
    {
        // If we don't have a target, or our current target is dead, look for a new one!
        if (targetTower == null || targetHealth == null || targetHealth.isDestroyed)
        {
            FindClosestTower();
            
            // If there are literally no towers left on the map, stop acting
            if (targetTower == null || targetHealth == null) return;
        }

        cooldownTimer -= Time.deltaTime;
        float distance = Vector3.Distance(transform.position, targetTower.position);

        if (distance <= attackRange && cooldownTimer <= 0f)
        {
            cooldownTimer = attackCooldown;
            targetHealth.TakeDamage(damage);
            Debug.Log($"{gameObject.name} attacked {targetTower.name} for {damage}");
        }
    }

    // Moved the target finding logic into its own reusable method
    void FindClosestTower()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag(enemyTowerTag);
        if (towers.Length == 0)
        {
            targetTower = null;
            targetHealth = null;
            return;
        }

        float closestDist = Mathf.Infinity;
        Transform bestTower = null;
        TowerHealth bestHealth = null;

        foreach (GameObject t in towers)
        {
            TowerHealth th = t.GetComponent<TowerHealth>();
            
            // Skip towers that are already dead!
            if (th == null || th.isDestroyed) continue;

            float dist = Vector3.Distance(transform.position, t.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                bestTower = t.transform;
                bestHealth = th;
            }
        }

        // Assign the new active target
        targetTower = bestTower;
        targetHealth = bestHealth;

        if (targetHealth == null && towers.Length > 0) 
        {
            Debug.LogError("TowerHealth NOT found on nearby towers!");
        }
    }
}





