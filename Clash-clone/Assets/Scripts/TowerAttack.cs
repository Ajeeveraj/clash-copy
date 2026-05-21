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

        TowerAttack[] all = GetComponents<TowerAttack>();
        if (all.Length > 1)
            Debug.LogError($"{name} has {all.Length} TowerAttack components! Remove duplicates.");
    }

    void Update()
    {
        if (towerHealth != null && towerHealth.isDestroyed) return;
        if (projectilePrefab == null) return;

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
            Debug.Log($"Troop {t.name} is {dist} units away");

            if (dist < closestDist)
            {
                closestDist = dist;
                closest = t.transform;
            }
        } // foreach ends here

       if (closest == null) return;

        Debug.Log($"{gameObject.name} Distance to troop: {closestDist}, attackRange: {attackRange}"); // REPLACE THIS LINE

        if (closestDist > attackRange)
        {
        Debug.Log("OUT OF RANGE - should not fire");
        return;
        }

Debug.Log("IN RANGE - firing");
cooldownTimer = attackCooldown;
        GameObject p = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        Projectile proj = p.GetComponent<Projectile>();
        if (proj != null) proj.SetTarget(closest);
        Debug.Log($"{name} fired at {closest.name} (dist {closestDist:F1}).");
    }
}





