using UnityEngine;
using UnityEngine.AI; // MUST HAVE THIS FOR NAVMESH UTILITIES
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    [Header("Resource Settings")]
    public float currentElixir = 0f;
    public float maxElixir = 10f;
    public float elixirRegenRate = 1f; 

    [Header("Spawn Points")]
    public Transform leftLaneSpawn;
    public Transform rightLaneSpawn;

    [Header("AI Deck (Troop Prefabs)")]
    public List<GameObject> troopPrefabs; 
    public List<int> troopCosts;          

    [Header("AI Logic Timer")]
    private float decisionTimer = 3f;
    private float currentTimer = 0f;

    void Update()
    {
        if (currentElixir < maxElixir)
        {
            currentElixir += elixirRegenRate * Time.deltaTime;
        }

        currentTimer += Time.deltaTime;
        if (currentTimer >= decisionTimer)
        {
            MakeAIDecision();
            currentTimer = 0f; 
        }
    }

    void MakeAIDecision()
    {
        if (currentElixir < 2) return; 

        int randomTroopIndex = Random.Range(0, troopPrefabs.Count);
        int cost = troopCosts[randomTroopIndex];

        if (currentElixir >= cost)
        {
            Transform chosenSpawnPoint = DetermineBestLane();
            
            // --- THE DEFINITIVE FIX ---
            // Find the absolute closest valid point on the baked NavMesh within 3 units of your spawn point object
            NavMeshHit hit;
            Vector3 finalSpawnPosition = chosenSpawnPoint.position;

            if (NavMesh.SamplePosition(chosenSpawnPoint.position, out hit, 3.0f, NavMesh.AllAreas))
            {
                // Snap the position coordinates exactly onto the blue mesh surface
                finalSpawnPosition = hit.position;
            }
            else
            {
                Debug.LogWarning($"⚠️ Spawn Point '{chosenSpawnPoint.name}' is too far away from the baked NavMesh floor!");
            }

            // Spawn the troop exactly on the clean grid line so its anchor point never bugs out
            GameObject spawnedTroop = Instantiate(troopPrefabs[randomTroopIndex], finalSpawnPosition, Quaternion.identity);
            
            // Force the agent to realize it is perfectly on the mesh right out of the gate
            NavMeshAgent agent = spawnedTroop.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.enabled = false;
                spawnedTroop.transform.position = finalSpawnPosition;
                agent.enabled = true;
            }

            currentElixir -= cost;
        }
    }

    Transform DetermineBestLane()
    {
        GameObject[] playerTroops = GameObject.FindGameObjectsWithTag("Troop");

        if (playerTroops.Length > 0)
        {
            GameObject targetTroop = playerTroops[0];

            if (targetTroop.transform.position.x < 0)
            {
                return leftLaneSpawn;
            }
            else
            {
                return rightLaneSpawn;
            }
        }

        int randomLane = Random.Range(0, 2);
        return randomLane == 0 ? leftLaneSpawn : rightLaneSpawn;
    }
}