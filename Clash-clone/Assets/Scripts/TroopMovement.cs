using UnityEngine;
using UnityEngine.AI;
using System.Collections; // FIXED: Cleaned up the typo here!

[DisallowMultipleComponent]
public class TroopMovement : MonoBehaviour
{
    [Header("Team Settings")]
    public bool isEnemy = false; 

    [Header("Movement Settings")]
    public float moveSpeed = 2f;

    [Header("Targeting")]
    public Transform targetTower; 
    
    private NavMeshAgent agent;
    private TowerHealth targetTowerHealth; 
    private string masterTowerTag;
    private bool isResettingPath = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null) { Debug.LogError("No NavMeshAgent!"); return; }
        agent.speed = moveSpeed;
        agent.stoppingDistance = 1.2f;

        masterTowerTag = isEnemy ? "PlayerTower" : "EnemyTower";
        FindNextTarget();
    }

    void Update()
    {
        // Check if our current target is dead or missing
        if (targetTower == null || targetTowerHealth == null || targetTowerHealth.isDestroyed)
        {
            if (!isResettingPath)
            {
                StartCoroutine(ForceHardPathReset());
            }
            return;
        }

        if (agent == null || !agent.enabled || !agent.isOnNavMesh || targetTower == null) return;

        if (targetTowerHealth != null && targetTowerHealth.isKingTower)
        {
            agent.stoppingDistance = 3.5f; 
        }
        else
        {
            agent.stoppingDistance = 1.2f;
        }

        agent.SetDestination(targetTower.position);

        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Vector3 dir = agent.velocity;
            dir.y = 0;
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    // Coroutine to break the spawnpoint tracking loop completely
    IEnumerator ForceHardPathReset()
    {
        isResettingPath = true;

        if (agent != null)
        {
            agent.ResetPath();
            agent.velocity = Vector3.zero;
            agent.enabled = false; // Turn off to sever the link to the original spawn point
        }

        FindNextTarget();

        // Wait exactly one physics frame
        yield return new WaitForEndOfFrame();

        if (agent != null)
        {
            agent.enabled = true; // Turn back on at current location
            
            if (targetTower != null && agent.isOnNavMesh)
            {
                agent.SetDestination(targetTower.position);
            }
        }

        isResettingPath = false;
    }

    void FindNextTarget()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag(masterTowerTag); 
        
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
            targetTowerHealth = closestTower.GetComponent<TowerHealth>();
        }
    }
}