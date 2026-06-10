using UnityEngine;

public class ArrowFlight : MonoBehaviour

{
    [Header("Movement")]
    public float speed = 15f;
    public float lifetime = 2f;

    [HideInInspector] public bool isEnemyArrow;

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

        // Homing logic: Calculate direction and look at target frame-by-frame
        Vector3 direction = (target.position - transform.position).normalized;
        transform.forward = direction;

        // Move forward along the track
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("The Arrow just hit an object named: " + other.gameObject.name);
        bool hitValidTarget = false;

        // If an ENEMY fired this, only hit PLAYER units
        if (isEnemyArrow)
        {
            if (other.CompareTag("Troop") || other.CompareTag("PlayerTower"))
            {
                hitValidTarget = true;
            }
        }
        // If a PLAYER fired this, only hit ENEMY units
        else 
        {
            if (other.CompareTag("EnemyTroop") || other.CompareTag("EnemyTower"))
            {
                hitValidTarget = true;
            }
        }

        // If it was a valid enemy target, deal the damage
        if (hitValidTarget)
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