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
            // If attackRange is 3.0, the Giant will stop at 2.5 units away
            // This guarantees it is well within range to trigger the attack
            agent.stoppingDistance = attackRange - 0.5f; 
        }
    }
    void Update()
    {
        FindNearestTower();

        if (currentTarget != null)
        {
            // Vector3.Distance factors in height (Y). Let's calculate flat distance instead:
            Vector3 flatGiantPos = new Vector3(transform.position.x, 0f, transform.position.z);
            Vector3 flatTargetPos = new Vector3(currentTarget.position.x, 0f, currentTarget.position.z);
            
            float distance = Vector3.Distance(flatGiantPos, flatTargetPos);

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
        // Try to find the TowerHealth script on the target
        TowerHealth towerHealth = currentTarget.GetComponent<TowerHealth>();
        
        if (towerHealth != null) 
        { 
            // This actually calls the function in your new script!
            towerHealth.TakeDamage(damage); 
            Debug.Log($"Giant smashed the {currentTarget.name} for {damage} damage!");
        }
        else
        {
            Debug.LogWarning("Giant is in range, but the target doesn't have a TowerHealth script attached!");
        }
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