using UnityEngine;

[DisallowMultipleComponent]
public class TowerAttack : MonoBehaviour
{
    public float attackRange = 10f;
    public float attackCooldown = 1f;
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public string targetTag = "Troop";
    public bool isKingTower = false;

    [Header("King Tower Only")]
    public TowerHealth princessTower1;
    public TowerHealth princessTower2;

    private float cooldownTimer = 0f;
    private TowerHealth towerHealth;

    void Awake()
    {
        towerHealth = GetComponent<TowerHealth>();
        if (shootPoint == null) shootPoint = transform;
    }

    void Update()
    {
        if (towerHealth != null && towerHealth.isDestroyed) return;
        if (projectilePrefab == null) return;

        if (isKingTower)
        {
            bool p1Alive = princessTower1 != null && !princessTower1.isDestroyed;
            bool p2Alive = princessTower2 != null && !princessTower2.isDestroyed;
            if (p1Alive || p2Alive) return;
        }

        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer > 0f) return;

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

        // Lane check - only fire at troops on my side of the map
        if (shootPoint.position.x > 0 && closest.position.x < 0) return;
        if (shootPoint.position.x < 0 && closest.position.x > 0) return;

        cooldownTimer = attackCooldown;
        GameObject p = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        Projectile proj = p.GetComponent<Projectile>();
        if (proj != null) proj.SetTarget(closest);
        Debug.Log($"{name} fired at {closest.name} (dist {closestDist:F1}).");
    }
}




