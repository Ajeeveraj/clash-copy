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
    private TowerAttack targetTowerAttack; // To check if the target is a King Tower

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
        if (targetTower == null || targetTower.GetComponent<TowerHealth>() == null || targetTower.GetComponent<TowerHealth>().isDestroyed)
        {
            FindNextTarget();
        }

        if (agent == null || targetTower == null) return;

        // 2. Adjust stopping distance dynamically based on tower type
        if (targetTowerAttack != null && targetTowerAttack.isKingTower)
        {
            // Give the massive King Tower a much larger stopping cushion
            agent.stoppingDistance = 3.5f; 
        }
        else
        {
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
            targetTowerAttack = towerObj.GetComponent<TowerAttack>();
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
                    closestTower = t; // <-- Fixed typo here!
                }
            }
        }

        if (closestTower != null)
        {
            targetTower = closestTower.transform;
            targetTowerAttack = closestTower.GetComponent<TowerAttack>();
        }
    }
}

