using UnityEngine;

[DisallowMultipleComponent]
public class Projectile : MonoBehaviour
{
    public float speed = 12f;
    public int damage = 50;
    public float destroyAfter = 5f;

    private Transform target;
    private float lifeTimer;
    private bool hasHit = false;

    private TroopHealth troopTarget;
    private TowerHealth towerTarget;

    public void SetTarget(Transform t)
    {
        target = t;
        troopTarget = t.GetComponentInParent<TroopHealth>();
        if (troopTarget == null)
            towerTarget = t.GetComponentInParent<TowerHealth>();

        float initialDist = Vector3.Distance(transform.position, t.position);
        Debug.Log($"Projectile spawned, initial distance to target: {initialDist}");
    }

    void Update()
    {
        if (hasHit) return;

        lifeTimer += Time.deltaTime;
        if (lifeTimer >= destroyAfter) { Destroy(gameObject); return; }

        if (target == null) { Destroy(gameObject); return; }

        if (troopTarget != null && troopTarget.isDead) { Destroy(gameObject); return; }
        if (towerTarget != null && towerTarget.isDestroyed) { Destroy(gameObject); return; }

        float dist = Vector3.Distance(transform.position, target.position);
        if (dist < 0.3f)
        {
            HitTarget();
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position, target.position, speed * Time.deltaTime);
    }

    private void HitTarget()
    {
        if (hasHit) return;
        hasHit = true;

        if (troopTarget != null && !troopTarget.isDead)
            troopTarget.TakeDamage(damage);
        else if (towerTarget != null && !towerTarget.isDestroyed)
            towerTarget.TakeDamage(damage);

        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        if (troopTarget != null)
        {
            TroopHealth th = other.GetComponentInParent<TroopHealth>();
            if (th != troopTarget) return;
        }

        if (towerTarget != null)
        {
            TowerHealth tw = other.GetComponentInParent<TowerHealth>();
            if (tw != towerTarget) return;
        }

        HitTarget();
    }
}









