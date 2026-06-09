using UnityEngine;
using UnityEngine.AI;
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

    [Header("AI Deck (0 = Troop, 1 = Giant)")]
    public List<GameObject> troopPrefabs; 
    public List<int> troopCosts;          

    [Header("Randomized Timers")]
    public float minTroopTime = 4f;
    public float maxTroopTime = 7f;
    private float troopTimer;

    public float minGiantTime = 15f;
    public float maxGiantTime = 25f;
    private float giantTimer;

    void Start()
    {
        // Start the game by picking a random time for both units
        troopTimer = Random.Range(minTroopTime, maxTroopTime);
        giantTimer = Random.Range(minGiantTime, maxGiantTime);
    }

    void Update()
    {
        // Elixir Regen
        if (currentElixir < maxElixir)
        {
            currentElixir += elixirRegenRate * Time.deltaTime;
        }

        // Count down both timers
        troopTimer -= Time.deltaTime;
        giantTimer -= Time.deltaTime;

        // Check the Giant timer first (Assuming Giant is Index 1 in your lists)
        if (giantTimer <= 0f)
        {
            // If it successfully spawns (has enough elixir), reset the timer with a new random number
            if (AttemptSpawn(1)) 
            {
                giantTimer = Random.Range(minGiantTime, maxGiantTime);
            }
        }

        // Check the Troop timer (Assuming normal Troop is Index 0 in your lists)
        if (troopTimer <= 0f)
        {
            if (AttemptSpawn(0)) 
            {
                troopTimer = Random.Range(minTroopTime, maxTroopTime);
            }
        }
    }

    // Changed MakeAIDecision to AttemptSpawn so we can tell it exactly WHICH troop to spawn
    bool AttemptSpawn(int listIndex)
    {
        // Failsafe: Make sure you actually added the troops to the list in the Inspector!
        if (listIndex >= troopPrefabs.Count) return false;

        int cost = troopCosts[listIndex];

        // Only spawn if the AI has saved up enough Elixir
        if (currentElixir >= cost)
        {
            Transform chosenSpawnPoint = DetermineBestLane();
            
            // --- THE DEFINITIVE FIX ---
            NavMeshHit hit;
            Vector3 finalSpawnPosition = chosenSpawnPoint.position;

            if (NavMesh.SamplePosition(chosenSpawnPoint.position, out hit, 3.0f, NavMesh.AllAreas))
            {
                finalSpawnPosition = hit.position;
            }
            else
            {
                Debug.LogWarning($"⚠️ Spawn Point '{chosenSpawnPoint.name}' is too far away from the baked NavMesh floor!");
            }

            GameObject spawnedTroop = Instantiate(troopPrefabs[listIndex], finalSpawnPosition, Quaternion.identity);
            
            NavMeshAgent agent = spawnedTroop.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.enabled = false;
                spawnedTroop.transform.position = finalSpawnPosition;
                agent.enabled = true;
            }

            // Deduct the elixir cost and tell Update() the spawn was a success
            currentElixir -= cost;
            return true; 
        }

        // Not enough elixir, return false so the timer stays at 0 and tries again next frame
        return false; 
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