using UnityEngine;

public class TroopAttack : MonoBehaviour
{
    public int damage = 40;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;

    private float cooldownTimer = 0f;
    private TowerHealth targetHealth;
    private Transform targetTower;

    void Start()
    {
        // Find ALL enemy towers
        GameObject[] towers = GameObject.FindGameObjectsWithTag("EnemyTower");

        if (towers.Length == 0)
        {
            Debug.LogError("No EnemyTower objects found!");
            return;
        }

        // Pick the closest tower
        float closestDist = Mathf.Infinity;

        foreach (GameObject t in towers)
        {
            float dist = Vector3.Distance(transform.position, t.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                targetTower = t.transform;
                targetHealth = t.GetComponent<TowerHealth>();
            }
        }
    }

    void Update()
    {
        if (targetTower == null || targetHealth == null)
            return;

        // Stop attacking dead towers
        if (targetHealth.isDestroyed)
            return;

        cooldownTimer -= Time.deltaTime;

        float distance = Vector3.Distance(transform.position, targetTower.position);

        if (distance <= attackRange)
        {
            if (cooldownTimer <= 0f)
            {
                cooldownTimer = attackCooldown;
                targetHealth.TakeDamage(damage);
                Debug.Log("Troop attacked tower for " + damage);
            }
        }
    }
}




