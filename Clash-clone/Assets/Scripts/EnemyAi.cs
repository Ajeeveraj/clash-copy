using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    [Header("Resource Settings")]
    public float currentElixir = 0f;
    public float maxElixir = 10f;
    public float elixirRegenRate = 1f; // 1 elixir per second (adjust to match player)

    [Header("Spawn Points")]
    public Transform leftLaneSpawn;
    public Transform rightLaneSpawn;

    [Header("AI Deck (Troop Prefabs)")]
    public List<GameObject> troopPrefabs; // Drop your 4 enemy troop prefabs here
    public List<int> troopCosts;          // Match the costs to the prefabs array order

    [Header("AI Logic Timer")]
    private float decisionTimer = 3f;
    private float currentTimer = 0f;

    void Update()
    {
        // 1. Regenerate Elixir
        if (currentElixir < maxElixir)
        {
            currentElixir += elixirRegenRate * Time.deltaTime;
        }

        // 2. Cooldown timer for making decisions (stops AI from spamming instantly)
        currentTimer += Time.deltaTime;
        if (currentTimer >= decisionTimer)
        {
            MakeAIDecision();
            currentTimer = 0f; // Reset decision window
        }
    }

    void MakeAIDecision()
    {
        // Don't do anything if we are completely broke
        if (currentElixir < 2) return; 

        // Pick a random troop from the deck
        int randomTroopIndex = Random.Range(0, troopPrefabs.Count);
        int cost = troopCosts[randomTroopIndex];

        // Check if the AI can afford it
        if (currentElixir >= cost)
        {
            Transform chosenSpawnPoint = DetermineBestLane();
            
            // Spawn the troop!
            Instantiate(troopPrefabs[randomTroopIndex], chosenSpawnPoint.position, Quaternion.identity);
            currentElixir -= cost;
        }
    }

    Transform DetermineBestLane()
    {
        // Find all player troops currently on the battlefield
        // (Make sure your player troop prefabs are tagged as "PlayerTroop"!)
        GameObject[] playerTroops = GameObject.FindGameObjectsWithTag("Troop");

        if (playerTroops.Length > 0)
        {
            // React to the first player troop found
            GameObject targetTroop = playerTroops[0];

            // If its X position is less than 0, it's on the left side
            if (targetTroop.transform.position.x < 0)
            {
                Debug.Log("AI detecting attack on LEFT lane.");
                return leftLaneSpawn;
            }
            else
            {
                Debug.Log("AI detecting attack on RIGHT lane.");
                return rightLaneSpawn;
            }
        }

        // If no player troops exist, pick a completely random lane
        int randomLane = Random.Range(0, 2);
        return randomLane == 0 ? leftLaneSpawn : rightLaneSpawn;
    }
}