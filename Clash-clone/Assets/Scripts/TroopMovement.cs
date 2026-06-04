using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(TroopAttack))]
public class TroopMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private TroopAttack attack;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        attack = GetComponent<TroopAttack>();
    }

    void Update()
    {
        if (attack.currentTarget == null)
        {
            if (agent.hasPath) agent.ResetPath();
            return;
        }

        float dist = Vector3.Distance(transform.position, attack.currentTarget.position);

        if (dist <= attack.attackRange)
        {
            if (!agent.isStopped) agent.isStopped = true;
        }
        else
        {
            if (agent.isStopped) agent.isStopped = false;
            agent.SetDestination(attack.currentTarget.position);
        }
    }
}