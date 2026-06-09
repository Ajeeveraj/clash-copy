using UnityEngine;
using UnityEngine.AI;

public class GiantAttack : MonoBehaviour
{
    [Header("Targeting")]
    public bool isEnemy = false;

    [Header("Combat Stats")]
    public float damage = 200f;
    public float attackRate = 1.5f;
    public float attackRange = 3.0f;

    private float nextAttackTime;
    private Transform currentTarget;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        if (agent != null)
        {
            agent.stoppingDistance = attackRange - 0.2f; 
        }
    }

    void Update()
    {
        FindNearestTower();

        if (currentTarget != null)
        {
            float distance = Vector3.Distance(transform.position, currentTarget.position);

            if (distance <= attackRange)
            {
                if (Time.time >= nextAttackTime)
                {
                    Attack();
                    nextAttackTime = Time.time + attackRate;
                }
            }
        }
    }

    void Attack()
    {
        Debug.Log($"Giant smashed the {currentTarget.name} for {damage} damage!");
        
        /* Uncomment this when your Tower health script is ready
        TowerHealth towerHealth = currentTarget.GetComponent<TowerHealth>();
        if (towerHealth != null) 
        { 
            towerHealth.TakeDamage(damage); 
        }
        */
    }

    void FindNearestTower()
    {
        // Automatically assigns the correct tag based on the checkbox
        string targetTag = isEnemy ? "PlayerTower" : "EnemyTower";

        GameObject[] towers = GameObject.FindGameObjectsWithTag(targetTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestTower = null;

        foreach (GameObject tower in towers)
        {
            float distanceToTower = Vector3.Distance(transform.position, tower.transform.position);
            if (distanceToTower < shortestDistance)
            {
                shortestDistance = distanceToTower;
                nearestTower = tower;
            }
        }

        if (nearestTower != null)
        {
            currentTarget = nearestTower.transform;
        }
        else
        {
            currentTarget = null;
        }
    }
}