using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class TroopMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    
    // We create slots for both scripts. Only one will actually be filled!
    private TroopAttack groundAttack;
    private MinionAttack flyingAttack;

    private float pathUpdateCooldown = 0.2f;
    private float pathUpdateTimer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        
        // The script checks the prefab. It fills whichever one exists and leaves the other null.
        groundAttack = GetComponent<TroopAttack>();
        flyingAttack = GetComponent<MinionAttack>();
    }

    void Start()
    {
        agent.updateRotation = false;
    }

    void Update()
    {
        // If the agent is off the NavMesh, don't try to move
        if (!agent.enabled || !agent.isOnNavMesh) return;

        // 1. Ask our helper method for the current target
        Transform currentTarget = GetTarget();

        // IF WE HAVE NO TARGET, STOP
        if (currentTarget == null)
        {
            StopMovingFully();
            return;
        }

        // TARGETING LOGIC
        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
        
        // 2. Ask our helper method for the correct combat range
        float combatRange = GetCombatRange(currentTarget);

        if (distanceToTarget <= combatRange)
        {
            StopMovingFully();
            // Handle rotation only
            Vector3 lookDir = currentTarget.position - transform.position;
            lookDir.y = 0;
            if (lookDir.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir.normalized), Time.deltaTime * 12f);
            return;
        }

        // CHASE LOGIC
        ChaseTarget(currentTarget.position);
    }

    // ==========================================
    // HELPER METHODS (The Fix!)
    // ==========================================
    
    private Transform GetTarget()
    {
        // If we have the ground script, use its target
        if (groundAttack != null) return groundAttack.currentTarget;
        
        // If we have the flying script, use its target
        if (flyingAttack != null) return flyingAttack.currentTarget;
        
        return null;
    }

    private float GetCombatRange(Transform target)
    {
        // If we have the ground script, use its range
        if (groundAttack != null) return groundAttack.GetActualAttackRange(target);
        
        // If we have the flying script, use its range
        if (flyingAttack != null) return flyingAttack.GetActualAttackRange(target);
        
        return 0f;
    }

    // ==========================================

    private void ChaseTarget(Vector3 targetPosition)
    {
        agent.isStopped = false;
        pathUpdateTimer -= Time.deltaTime;

        if (pathUpdateTimer <= 0f)
        {
            pathUpdateTimer = pathUpdateCooldown;
            agent.SetDestination(targetPosition);
        }

        HandleRotation();
    }

    private void HandleRotation()
    {
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Vector3 moveDir = agent.velocity.normalized;
            moveDir.y = 0;

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(moveDir),
                Time.deltaTime * 10f
            );
        }
    }

    private void StopMovingFully()
    {
        pathUpdateTimer = 0f;

        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }
    }
}