using UnityEngine;

public class ArrowFlight : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f; 

    void Start()
    {
        // Destroy the arrow after 2 seconds to keep the game clean
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // This detects if the arrow hits something with the tag "Troop"
        // You can add more tags here if needed (e.g., "EnemyTower")
        if (other.CompareTag("Troop") || other.CompareTag("EnemyTower"))
        {
            // 1. (Optional) Call a "TakeDamage" function here later
            
            // 2. Destroy the arrow immediately on impact
            Destroy(gameObject);
        }
    }
}