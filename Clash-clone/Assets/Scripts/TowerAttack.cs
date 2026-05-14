using UnityEngine;

[DisallowMultipleComponent]
public class TowerAttack : MonoBehaviour
{
    public float attackRange = 7f;
    public float attackCooldown = 1f;
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public string targetTag = "Troop";

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

        cooldownTimer -= Time.deltaTime;

        GameObject[] troops = GameObject.FindGameObjectsWithTag(targetTag);
        if (troops == null || troops.Length == 0) return;

        Transform closest = null;
        float closestDist = Mathf.Infinity;
        Vector3 towerPos = transform.position;

        foreach (GameObject t in troops)
        {
            TroopHealth th = t.GetComponentInParent<TroopHealth>();
            if (th != null && th.isDead) continue;

            Collider col = t.GetComponentInChildren<Collider>();
            float dist = (col != null) ? Vector3.Distance(towerPos, col.ClosestPoint(towerPos))
                                       : Vector3.Distance(towerPos, t.transform.position);

            if (dist < closestDist)
            {
                closestDist = dist;
                closest = t.transform;
            }
        }

        if (closest == null) return;
        if (closestDist > attackRange) return;

        if (cooldownTimer <= 0f)
        {
            cooldownTimer = attackCooldown;
            GameObject p = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
            Projectile proj = p.GetComponent<Projectile>();
            if (proj != null) proj.SetTarget(closest);
            Debug.Log($"{name} fired at {closest.name} (dist {closestDist:F1}).");
        }
    }
}






