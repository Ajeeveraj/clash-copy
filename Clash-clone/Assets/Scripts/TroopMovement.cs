using UnityEngine;
using UnityEngine.AI;
using System.Collections;

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
    private bool updatingTarget = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null) { Debug.LogError("No NavMeshAgent!"); return; }
        
        agent.speed = moveSpeed;
        agent.stoppingDistance = 1.2f;
        agent.acceleration = 30f; 

        masterTowerTag = isEnemy ? "PlayerTower" : "EnemyTower";
        FindNextTarget();
    }

    void Update()
    {
        // If we are currently executing our forced override lock, stop all normal movement code
        if (updatingTarget)
        {
            StopAndZeroAgent();
            return;
        }

        // Detect if our target tower has been destroyed
        if (targetTower == null || targetTowerHealth == null || targetTowerHealth.isDestroyed)
        {
            StartCoroutine(ForceLockKingTowerPath());
            return;
        }

        ExecuteMovement();
    }

    void ExecuteMovement()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh || targetTower == null) return;

        if (targetTowerHealth != null && targetTowerHealth.isKingTower)
        {
            agent.stoppingDistance = 1.5f; 
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

    void StopAndZeroAgent()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.ResetPath();
            agent.velocity = Vector3.zero;
        }
    }

    // This coroutine physically locks down the unit to prevent any other script from sending them backward
    IEnumerator ForceLockKingTowerPath()
    {
        updatingTarget = true;

        // 1. Instantly freeze them in place right where the princess tower died
        StopAndZeroAgent();

        // 2. Find the King Tower target asset
        FindNextTarget();

        // 3. Keep forcing them to stand still for 0.2 seconds
        // This acts as a protective shield against other scripts trying to command them back to spawn!
        float timer = 0.2f;
        while (timer > 0f)
        {
            StopAndZeroAgent();
            timer -= Time.deltaTime;
            yield return null;
        }

        // 4. Now that other scripts have cleared their queues, force the fresh forward destination
        if (agent != null && agent.isOnNavMesh && targetTower != null)
        {
            agent.ResetPath();
            agent.SetDestination(targetTower.position);
        }

        updatingTarget = false;
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