using UnityEngine;

public class TowerAttack : MonoBehaviour
{
    public float attackRange = 7f;
    public float attackCooldown = 1f;
    public GameObject projectilePrefab;
    public Transform shootPoint;

    private float cooldownTimer = 0f;

    void Update()
    {
        cooldownTimer -= Time.deltaTime;

        // Find nearest troop
        GameObject[] troops = GameObject.FindGameObjectsWithTag("Troop");
        Transform closest = null;
        float closestDist = Mathf.Infinity;

        foreach (GameObject t in troops)
        {
            float dist = Vector3.Distance(transform.position, t.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = t.transform;
            }
        }

        if (closest == null)
            return;

        if (closestDist > attackRange)
            return;

        if (cooldownTimer <= 0f)
        {
            cooldownTimer = attackCooldown;
            GameObject p = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
            p.GetComponent<Projectile>().SetTarget(closest);


            Debug.Log("Tower shot troop");
        }
    }
}




