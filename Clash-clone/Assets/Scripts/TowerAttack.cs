using UnityEngine;

public class TowerAttack : MonoBehaviour
{
    public float range = 6f;
    public float attackCooldown = 1f;
    public GameObject projectilePrefab;
    public Transform shootPoint;

    private float timer = 0f;

    void Update()
    {
        timer -= Time.deltaTime;

        // Find all troops (modern Unity API)
        TroopHealth[] troops = Object.FindObjectsByType<TroopHealth>(FindObjectsInactive.Exclude);

        TroopHealth closest = null;
        float closestDist = Mathf.Infinity;

        foreach (var troop in troops)
        {
            float dist = Vector3.Distance(transform.position, troop.transform.position);

            if (dist < closestDist && dist <= range)
            {
                closestDist = dist;
                closest = troop;
            }
        }

        if (closest != null && timer <= 0f)
        {
            ShootProjectile(closest);
            timer = attackCooldown;
        }
    }

    void ShootProjectile(TroopHealth target)
    {
        GameObject proj = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);

        Projectile p = proj.GetComponent<Projectile>();
        p.SetTarget(target.transform.root);

    }
}



