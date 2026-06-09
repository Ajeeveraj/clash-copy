using UnityEngine;

[DisallowMultipleComponent]
public class TroopHealth : MonoBehaviour
{
    public int maxHealth = 200;
    [HideInInspector] public int currentHealth;
    [HideInInspector] public bool isDead = false;

    public HealthBar healthBar;

    private Renderer[] renderers;
    private Collider[] colliders;

    void Awake()
    {
        currentHealth = Mathf.Max(1, maxHealth);
        renderers = GetComponentsInChildren<Renderer>(true);
        colliders = GetComponentsInChildren<Collider>(true);

        if (healthBar != null) healthBar.SetMax(currentHealth);
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;
        if (amount <= 0) return;

        currentHealth -= amount;
        Debug.Log($"[TroopHealth] {name} HP: {currentHealth}/{maxHealth} isDead={isDead}");

        if (healthBar != null) healthBar.Set(currentHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
            Debug.Log($"[TroopHealth] {name} CALLING DIE NOW");
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        // Turn off navigation immediately so it stops blocking the link
        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null) agent.enabled = false;

        // Turn off the collider so new troops can walk right through the space
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // Trigger your death animation here if you have one
        
        // Completely remove the object
        Destroy(gameObject); 
    }

    void OnValidate()
    {
        if (maxHealth < 1) maxHealth = 1;
        if (!Application.isPlaying) currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }
}












