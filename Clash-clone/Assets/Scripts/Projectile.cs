using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 80f;
    public float speed = 20f;
    public float lifeTime = 5f;

    private Transform target;
    private Vector3 targetPosition;
    private bool usePosition = false;

    void Start()
    {
        Debug.Log("[Projectile] Started at " + transform.position);
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (target != null)
            MoveTowards(target.position);
        else if (usePosition)
            MoveTowards(targetPosition);
    }

    void MoveTowards(Vector3 dest)
    {
        Vector3 dir = dest - transform.position;
        float dist = dir.magnitude;
        if (dist < 0.05f) return;
        dir.Normalize();
        transform.position += dir * speed * Time.deltaTime;
        if (Time.frameCount % 10 == 0)
            Debug.Log("[Projectile] moving. pos=" + transform.position + " targetDist=" + dist);
    }

    public void SetTarget(Transform t)
    {
        target = t;
        usePosition = false;
        Debug.Log("[Projectile] SetTarget called. target=" + (t ? t.name : "null"));
    }

    public void SetTarget(Vector3 worldPos)
    {
        target = null;
        targetPosition = worldPos;
        usePosition = true;
        Debug.Log("[Projectile] SetTarget called. position=" + worldPos);
    }

    public GameObject shooter; // set this when instantiating

    void OnTriggerEnter(Collider other)
{
    if (other.gameObject == shooter) return;

    if (!other.CompareTag("Troop"))
    {
        Debug.Log("[Projectile] Ignored (not Troop): " + other.name + " layer=" + LayerMask.LayerToName(other.gameObject.layer));
        return;
    }

    TroopHealth th = other.GetComponent<TroopHealth>() ?? other.GetComponentInParent<TroopHealth>();
    if (th != null)
    {
        Debug.Log("[Projectile] Applying damage " + damage + " to " + th.gameObject.name);
        th.TakeDamage(damage);
        Destroy(gameObject);
        return;
    }

    Debug.Log("[Projectile] Tagged Troop but no TroopHealth found on " + other.name);
}



}







