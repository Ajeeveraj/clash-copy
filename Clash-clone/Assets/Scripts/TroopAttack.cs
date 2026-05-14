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
        // find closest enemy tower (works with multiple towers)
        GameObject[] towers = GameObject.FindGameObjectsWithTag(enemyTowerTag);
        if (towers.Length == 0)
        {
            Debug.LogError("No towers with tag " + enemyTowerTag + " found!");
            return;
        }

        float closestDist = Mathf.Infinity;
        foreach (GameObject t in towers)
        {
            float dist = Vector3.Distance(transform.position, t.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                targetTower = t.transform;
                targetHealth = t.GetComponentInParent<TowerHealth>();
            }
        }

        if (targetHealth == null) Debug.LogError("TowerHealth NOT found on parent tower!");
    }

    void Update()
    {
        if (targetTower == null || targetHealth == null) return;
        if (targetHealth.isDestroyed) return;

        cooldownTimer -= Time.deltaTime;
        float distance = Vector3.Distance(transform.position, targetTower.position);

        if (distance <= attackRange && cooldownTimer <= 0f)
        {
            cooldownTimer = attackCooldown;
            targetHealth.TakeDamage(damage);
            Debug.Log($"{gameObject.name} attacked {targetTower.name} for {damage}");
        }
    }
}






