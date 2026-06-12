using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    public float speed = 15f;
    public float damageRadius = 2.5f;
    public int damageAmount = 50;

    [Header("Settings")]
    public bool isEnemySpell; // Just check this in the inspector for the enemy fireball prefab

    private Vector3 targetPosition;
    private bool isInitialized = false;

    public void InitializeTarget(Vector3 groundTarget)
    {
        targetPosition = groundTarget;
        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized) return;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPosition) <= 0.1f) Explode();
    }

    void Explode()
    {
        // Minimal logic: Enemy hits "Troop" (Player's) and "PlayerTower"
        // Player hits "EnemyTroop" and "EnemyTower"
        string troopTag = isEnemySpell ? "Troop" : "EnemyTroop";
        string towerTag = isEnemySpell ? "PlayerTower" : "EnemyTower";

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag(troopTag))
                hit.GetComponent<TroopHealth>()?.TakeDamage(damageAmount);
            
            if (hit.CompareTag(towerTag))
                hit.GetComponent<TowerHealth>()?.TakeDamage(damageAmount);
        }
        Destroy(gameObject);
    }
}