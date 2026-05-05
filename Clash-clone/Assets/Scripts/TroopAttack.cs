using UnityEngine;

public class TroopAttack : MonoBehaviour
{
    public int damage = 50;
    public float attackRange = 2f;
    public float attackCooldown = 1f;

    private float attackTimer = 0f;

    private TroopMovement movement;
    private Transform targetTower;
    private TowerHealth towerHealth;

    void Start()
    {
        movement = GetComponent<TroopMovement>();
        targetTower = movement.GetTargetTower();
        towerHealth = targetTower.GetComponent<TowerHealth>();
    }

    void Update()
    {
        if (towerHealth == null || towerHealth.isDestroyed) return;

        float distance = Vector3.Distance(transform.position, targetTower.position);

        if (distance <= attackRange)
        {
            movement.StopMoving(); // stop walking
            AttackTower();
        }
    }

    void AttackTower()
    {
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            towerHealth.TakeDamage(damage);
            attackTimer = attackCooldown;
        }
    }
}

