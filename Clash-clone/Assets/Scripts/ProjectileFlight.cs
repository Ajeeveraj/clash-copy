using UnityEngine;

public class ProjectileFlight : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 15f; 
    public float lifetime = 2f;

    [HideInInspector] public float damage; 
    [HideInInspector] public Transform target; 

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // If target dies mid-air, keep flying straight
        if (target == null)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            return;
        }

        // Homing logic: Track the target frame-by-frame
        Vector3 direction = (target.position - transform.position).normalized;
        transform.forward = direction;
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // If we don't have a target, don't register hits
        if (target == null) return;

        // Simplify: Just check if the object we hit is our locked target
        // (Checking both the collider's object and its root/parent for safety)
        if (other.transform == target || other.transform.root == target)
        {
            TroopHealth targetHealth = other.GetComponentInParent<TroopHealth>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage((int)damage);
            }
            
            Destroy(gameObject); // Boom!
        }
    }
}