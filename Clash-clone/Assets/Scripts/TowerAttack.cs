using UnityEngine;

[DisallowMultipleComponent]
public class TowerAttack : MonoBehaviour
{
    public float attackRange = 12f;
    public float attackCooldown = 1f;
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public string targetTag = "Troop";
    public bool isKingTower = false;

    [Header("King Tower & Lane Setup")]
    public TowerHealth princessTower1;
    public TowerHealth princessTower2;

    private float cooldownTimer = 0f;
    private TowerHealth towerHealth;
    private float maxHealthCache;

    void Awake()
    {
        towerHealth = GetComponent<TowerHealth>();
        if (shootPoint == null) shootPoint = transform;
    }

    void Start()
    {
        if (towerHealth != null)
        {
            maxHealthCache = towerHealth.maxHealth;
        }
    }

    void Update()
    {
        if (towerHealth != null && towerHealth.isDestroyed) return;
        if (projectilePrefab == null) return;

        // --- KING TOWER WAKE UP LOGIC ---
        if (isKingTower && towerHealth != null)
        {
            bool p1Alive = princessTower1 != null && !princessTower1.isDestroyed;
            bool p2Alive = princessTower2 != null && !princessTower2.isDestroyed;
            
            bool hasBeenDamaged = towerHealth.currentHealth < maxHealthCache;

            if (hasBeenDamaged)
            {
                // King is awake! Let execution pass down to targeting logic
            }
            else if (p1Alive || p2Alive)
            {
                return; // Stay asleep if princess towers are alive and untargeted
            }
        }

        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer > 0f) return;

        // --- TARGETING LOGIC ---
        GameObject[] troops = GameObject.FindGameObjectsWithTag(targetTag);
        if (troops == null || troops.Length == 0) return;

        Transform closest = null;
        float closestDist = Mathf.Infinity;
        Vector3 towerPos = shootPoint.position;

        foreach (GameObject t in troops)
        {
            TroopHealth th = t.GetComponent<TroopHealth>();
            if (th != null && th.isDead) continue;

            float dist = Vector3.Distance(towerPos, t.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = t.transform;
            }
        }

        if (closest == null) return;
        if (closestDist > attackRange) return;

        // --- FIXED LANE CHECK ---
        if (!isKingTower)
        {
            // OVERRIDE RULE: If the troop is targeted on the King Tower, Princesses ignore it.
            TroopMovement movement = closest.GetComponent<TroopMovement>();
            if (movement != null && movement.targetTower != null)
            {
                TowerAttack targetTowerAttack = movement.targetTower.GetComponent<TowerAttack>();
                if (targetTowerAttack != null && targetTowerAttack.isKingTower)
                {
                    return; // The troop is tracking the King Tower! Princess stands down.
                }
            }

            // Standard distance-based lane check fallback
            TowerHealth oppositePrincess = (GetComponent<TowerHealth>() == princessTower1) ? princessTower2 : princessTower1;
            if (oppositePrincess != null)
            {
                float distToMeX = Mathf.Abs(closest.position.x - transform.position.x);
                float distToOppositeX = Mathf.Abs(closest.position.x - oppositePrincess.transform.position.x);

                if (distToOppositeX < distToMeX) 
                {
                    return; 
                }
            }
        }

        // --- FIRE PROJECTILE ---
        cooldownTimer = attackCooldown;
        GameObject p = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        Projectile proj = p.GetComponent<Projectile>();
        if (proj != null) proj.SetTarget(closest);

        Debug.Log($"{name} fired a defense shot at {closest.name}!");
    }
}