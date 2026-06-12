using UnityEngine;

public class EnemySpellAI : MonoBehaviour
{
    [Header("Spell Settings")]
    public GameObject enemyFireballPrefab;
    public GameObject enemyArrowPrefab; // <- Added this so it shows up in your Inspector
    public float fireballBlastRadius = 2.5f; 
    public int minimumTroopsToTarget = 3;

    [Header("AI Rules")]
    public float checkInterval = 1.5f; 
    public float spellCooldown = 15f; 
    
    private float checkTimer = 0f;
    private float cooldownTimer = 0f;

    void Update()
    {
        if (cooldownTimer > 0) 
        {
            cooldownTimer -= Time.deltaTime;
        }

        checkTimer -= Time.deltaTime;
        if (checkTimer <= 0f)
        {
            checkTimer = checkInterval;
            
            if (cooldownTimer <= 0f)
            {
                AnalyzeBoardForTargets();
            }
        }
    }

    void AnalyzeBoardForTargets()
    {
        GameObject[] playerTroops = GameObject.FindGameObjectsWithTag("Troop");

        if (playerTroops.Length < minimumTroopsToTarget) return;

        int bestClusterSize = 0;
        Vector3 bestTargetLocation = Vector3.zero;

        foreach (GameObject troop in playerTroops)
        {
            int clusterCount = 0;
            Vector3 clusterCenter = Vector3.zero;

            foreach (GameObject neighbor in playerTroops)
            {
                if (Vector3.Distance(troop.transform.position, neighbor.transform.position) <= fireballBlastRadius)
                {
                    clusterCount++;
                    clusterCenter += neighbor.transform.position; 
                }
            }

            if (clusterCount >= minimumTroopsToTarget && clusterCount > bestClusterSize)
            {
                bestClusterSize = clusterCount;
                bestTargetLocation = clusterCenter / clusterCount; 
            }
        }

        if (bestClusterSize >= minimumTroopsToTarget)
        {
            CastFireball(bestTargetLocation, bestClusterSize);
        }
    }

    void CastFireball(Vector3 targetPos, int clusterSize)
    {
        targetPos.y = 0f;
        Vector3 skyPos = new Vector3(targetPos.x, 12f, 20f);
        
        GameObject fireball = Instantiate(enemyFireballPrefab, skyPos, Quaternion.LookRotation(Vector3.down));

        if (fireball.TryGetComponent<FireballProjectile>(out var fbScript))
        {
            fbScript.isEnemySpell = true; 
            fbScript.InitializeTarget(targetPos);
        }

        cooldownTimer = spellCooldown;
    }
}