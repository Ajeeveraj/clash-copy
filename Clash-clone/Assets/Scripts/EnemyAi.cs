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
    public GameObject musketeerPrefab; 
    public GameObject minionsPrefab; // Added Minion Prefab slot!

    [Header("Elixir Settings")]
    public float currentElixir = 5f;
    public float maxElixir = 10f;
    public float elixirRegenSpeed = 0.35f; 

    private int giantCost = 5;
    private int troopCost = 3;
    private int archerCost = 3;
    private int minipekkaCost = 4; 
    private int musketeerCost = 4; 
    private int minionsCost = 3; // Added Minion Cost!

    [Header("Timer Settings")]
    private float decisionTimer = 0f;
    public float decisionInterval = 2.5f; 

    void Update()
    {
        if (currentElixir < maxElixir)
        {
            currentElixir += elixirRegenSpeed * Time.deltaTime;
        }

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

        // Pick a random unit archetype: changed max to 6 now that Minions are here
        int randomUnit = Random.Range(0, 6); 

        if (randomUnit == 0 && currentElixir >= giantCost)
        {
            SpawnEnemyUnit(giantPrefab, giantCost);
        }
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
        else if (randomUnit == 4 && currentElixir >= musketeerCost)
        {
            SpawnEnemyUnit(musketeerPrefab, musketeerCost);
        }
        else if (randomUnit == 5 && currentElixir >= minionsCost)
        {
            SpawnEnemyUnit(minionsPrefab, minionsCost);
        }
    }

    void SpawnEnemyUnit(GameObject prefab, int cost)
    {
        if (prefab == null) return;

        Transform randomLane = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // 1. Determine group spawn size dynamically
        int spawnCount = 1;
        if (prefab == archerPrefab) spawnCount = 2;
        else if (prefab == minionsPrefab) spawnCount = 3;

        // 2. Loop to spawn the entire squad side-by-side
        for (int i = 0; i < spawnCount; i++)
        {
            // Centers the group and offsets each unit by 0.7 units to the left or right
            float horizontalOffset = (i - (spawnCount - 1) * 0.5f) * 0.7f;
            
            // Uses randomLane.right so the offset matches whichever way your lane point is rotated
            Vector3 finalPosition = randomLane.position + (randomLane.right * horizontalOffset);

            Instantiate(prefab, finalPosition, randomLane.rotation);
        }

        currentElixir -= cost;
    }
}