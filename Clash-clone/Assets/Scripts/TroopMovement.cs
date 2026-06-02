using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
public class TroopMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public string leftTowerTag = "EnemyTowerLeft";
    public string rightTowerTag = "EnemyTowerRight";
    public float mapCenterX = 8f;

    // Changed to public so TowerAttack can read what this troop is targeting
    public Transform targetTower; 
    
    private NavMeshAgent agent;
    
    // FIX: Changed from TowerAttack to TowerHealth so we look at the right script!
    private TowerHealth targetTowerHealth; 

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null) { Debug.LogError("No NavMeshAgent!"); return; }
        agent.speed = moveSpeed;
        
        // Default stopping distance for normal princess towers
        agent.stoppingDistance = 1.2f;

        SetInitialTarget();
    }

    void Update()
    {
        // 1. If we lose our target or it gets destroyed, look for the next closest one (the King Tower)
        if (targetTower == null || targetTowerHealth == null || targetTowerHealth.isDestroyed)
        {
            FindNextTarget();
        }

        if (agent == null || targetTower == null) return;

        // 2. Adjust stopping distance dynamically based on tower type
        // FIX: Now we securely read isKingTower from the TowerHealth component
        if (targetTowerHealth != null && targetTowerHealth.isKingTower)
        {
            // Give the massive King Tower a much larger stopping cushion
            agent.stoppingDistance = 3.5f; 
        }
        else
        {
            // Reset to default for standard princess towers
            agent.stoppingDistance = 1.2f;
        }

        // 3. Keep updating the path destination
        agent.SetDestination(targetTower.position);

        // 4. Handle smooth rotation
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Vector3 dir = agent.velocity;
            dir.y = 0;
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    void SetInitialTarget()
    {
        string tag = transform.position.x >= mapCenterX ? rightTowerTag : leftTowerTag;
        GameObject towerObj = GameObject.FindWithTag(tag);
        if (towerObj != null)
        {
            targetTower = towerObj.transform;
            // FIX: Grab the TowerHealth component instead of TowerAttack
            targetTowerHealth = towerObj.GetComponent<TowerHealth>();
        }
    }

    void FindNextTarget()
    {
        // Search for any remaining active enemy towers (like the King Tower)
        GameObject[] towers = GameObject.FindGameObjectsWithTag("EnemyTower"); 
        
        float closestDist = Mathf.Infinity;
        GameObject closestTower = null;

        foreach (GameObject t in towers)
        {
            TowerHealth th = t.GetComponent<TowerHealth>();
            if (th != null && !th.isDestroyed)
            {
                float dist = Vector3.Distance(transform.position, t.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestTower = t; 
                }
            }
        }

        if (closestTower != null)
        {
            targetTower = closestTower.transform;
            // FIX: Track the new target's health script component cleanly
            targetTowerHealth = closestTower.GetComponent<TowerHealth>();
        }
    }
}