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
        if (lifeTimer >= destroyAfter) Destroy(gameObject);
        if (target == null) return;
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // prefer troop first
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










