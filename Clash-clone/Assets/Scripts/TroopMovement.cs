using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(TroopAttack))]
public class TroopMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private TroopAttack attack;

    private float pathUpdateCooldown = 0.2f;
    private float pathUpdateTimer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        attack = GetComponent<TroopAttack>();
    }

    void Start()
    {
        agent.updateRotation = false;
    }

    void Update()
    {
        // If the agent is off the NavMesh, don't try to move
        if (!agent.enabled || !agent.isOnNavMesh) return;

        // IF WE HAVE NO TARGET, STOP
        if (attack.currentTarget == null)
        {
            StopMovingFully();
            return;
        }

        // TARGETING LOGIC
        float distanceToTarget = Vector3.Distance(transform.position, attack.currentTarget.position);
        float combatRange = attack.GetActualAttackRange(attack.currentTarget);

        if (distanceToTarget <= combatRange)
        {
            StopMovingFully();
            // Handle rotation only
            Vector3 lookDir = attack.currentTarget.position - transform.position;
            lookDir.y = 0;
            if (lookDir.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir.normalized), Time.deltaTime * 12f);
            return;
        }

        // CHASE LOGIC
        ChaseTarget(attack.currentTarget.position);
    }

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