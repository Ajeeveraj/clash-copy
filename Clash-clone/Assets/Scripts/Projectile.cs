using UnityEngine;

[DisallowMultipleComponent]
public class Projectile : MonoBehaviour
{
    public float speed = 12f;
    public int damage = 50;
    public float destroyAfter = 5f;

    private Transform target;
    private float lifeTimer;

    public void SetTarget(Transform t) { target = t; }

    void Update()
    {
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= destroyAfter)
        {
            Destroy(gameObject);
            return;
        }

        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // If already overlapping target, apply damage directly
        float dist = Vector3.Distance(transform.position, target.position);
        if (dist < 0.2f)
        {
            HitTarget();
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position, target.position, speed * Time.deltaTime);
    }

    private void HitTarget()
    {
        var troop = target.GetComponentInParent<TroopHealth>();
        if (troop != null)
        {
            troop.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        var tower = target.GetComponentInParent<TowerHealth>();
        if (tower != null)
        {
            tower.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        var troop = other.GetComponentInParent<TroopHealth>();
        if (troop != null)
        {
            troop.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        var tower = other.GetComponentInParent<TowerHealth>();
        if (tower != null)
        {
            tower.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
    }
}










