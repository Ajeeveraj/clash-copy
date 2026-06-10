using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Lane Spawn Points")]
    public Transform[] spawnPoints; 

    [Header("Unit Prefabs")]
    public GameObject giantPrefab;
    public GameObject troopPrefab;
    public GameObject archerPrefab;
    public GameObject minipekkaPrefab; 

    [Header("Elixir Settings")]
    public float currentElixir = 5f;
    public float maxElixir = 10f;
    public float elixirRegenSpeed = 0.35f; 

    private int giantCost = 5;
    private int troopCost = 3;
    private int archerCost = 3;
    private int minipekkaCost = 4; 

    [Header("Timer Settings")]
    private float decisionTimer = 0f;
    public float decisionInterval = 2.5f; 

    void Update()
    {
        // 1. Regenerate Elixir automatically over time
        if (currentElixir < maxElixir)
        {
            currentElixir += elixirRegenSpeed * Time.deltaTime;
        }

        // 2. The Spawning Timer
        decisionTimer += Time.deltaTime;
        if (decisionTimer >= decisionInterval)
        {
            EnemyDecisionLogic();
            decisionTimer = 0f; 
        }
    }

    void EnemyDecisionLogic()
    {
        if (spawnPoints.Length == 0) return;

        // Pick a random unit archetype: 0 = Giant, 1 = Troop, 2 = Archer, 3 = Mini P.E.K.K.A
        int randomUnit = Random.Range(0, 4); 

        // Check if we have enough Elixir to afford the chosen card
        if (randomUnit == 0 && currentElixir >= giantCost)
        {
            SpawnEnemyUnit(giantPrefab, giantCost);
        }
        // This is your Knight card
        else if (randomUnit == 1 && currentElixir >= troopCost)
        {
            SpawnEnemyUnit(troopPrefab, troopCost);
        }
        else if (randomUnit == 2 && currentElixir >= archerCost)
        {
            SpawnEnemyUnit(archerPrefab, archerCost);
        }
        else if (randomUnit == 3 && currentElixir >= minipekkaCost)
        {
            SpawnEnemyUnit(minipekkaPrefab, minipekkaCost);
        }
    }

    void SpawnEnemyUnit(GameObject prefab, int cost)
    {
        if (prefab == null) return;

        // Choose a random invisible lane point
        Transform randomLane = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Spawn the unit
        Instantiate(prefab, randomLane.position, randomLane.rotation);

        // Deduct the elixir cost cleanly
        currentElixir -= cost;
    }
}