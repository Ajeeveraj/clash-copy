using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
public class TroopMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Transform targetTower; // drag the correct tower in the Inspector

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null) { Debug.LogError("No NavMeshAgent!"); return; }

        agent.speed = moveSpeed;
        agent.stoppingDistance = 1.2f;

        if (targetTower != null)
            agent.SetDestination(targetTower.position);
        else
            Debug.LogError("No target tower assigned!");
    }

    void Update()
    {
        if (agent == null || targetTower == null) return;
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Vector3 dir = agent.velocity;
            dir.y = 0;
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }
}



