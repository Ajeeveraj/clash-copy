using UnityEngine;

public class TroopAttack : MonoBehaviour
{
    public int damage = 40;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;

    private float cooldownTimer = 0f;
    private Transform targetTower;
    private TowerHealth towerHealth;

    void Start()
    {
        // CHANGE THIS depending on troop side:
        GameObject towerObj = GameObject.FindWithTag("EnemyPrincessTower");

        if (towerObj != null)
        {
            targetTower = towerObj.transform;
            towerHealth = targetTower.GetComponent<TowerHealth>();
        }
        else
        {
            Debug.LogError("No EnemyPrincessTower found in scene!");
        }
    }

    void Update()
    {
        if (targetTower == null)
            return;

        cooldownTimer -= Time.deltaTime;

        float distance = Vector3.Distance(transform.position, targetTower.position);

        if (distance <= attackRange)
        {
            if (cooldownTimer <= 0f)
            {
                cooldownTimer = attackCooldown;
                towerHealth.TakeDamage(damage);
                Debug.Log("Troop attacked tower for " + damage);
            }
        }
    }
}



