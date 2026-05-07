using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 40;

    private Transform target;

    public void SetTarget(Transform t)
    {
        target = t;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Move toward target
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // Check if close enough to hit
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            // Always get TroopHealth from the root object
            TroopHealth th = target.transform.root.GetComponent<TroopHealth>();
            if (th != null)
            {
                Debug.Log("Projectile hit troop for " + damage);
                th.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}

