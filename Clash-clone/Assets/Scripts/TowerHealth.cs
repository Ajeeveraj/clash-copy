using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class TowerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 1000;
    [HideInInspector] public int currentHealth;
    [HideInInspector] public bool isDestroyed = false;

    [Header("Optional")]
    public GameObject deathVFX;
    public bool destroyOnDeath = false;
    public UnityEvent onDestroyed;

    // UI hook (optional): assign a HealthBar component on the same GameObject or child
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
        if (isDestroyed) return;
        if (amount <= 0) return;

        currentHealth -= amount;
        Debug.Log($"{name} took {amount} damage. HP: {currentHealth}/{maxHealth}");

        if (healthBar != null) healthBar.Set(currentHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDestroyed = true;
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (isDestroyed) return;
        if (amount <= 0) return;
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        if (healthBar != null) healthBar.Set(currentHealth);
    }

    private void Die()
    {
        foreach (var r in renderers) if (r != null) r.enabled = false;
        foreach (var c in colliders) if (c != null) c.enabled = false;

        if (deathVFX != null) Instantiate(deathVFX, transform.position, Quaternion.identity);
        Debug.Log($"{name} destroyed");
        onDestroyed?.Invoke();

        if (destroyOnDeath) Destroy(gameObject);
    }

    void OnValidate()
    {
        if (maxHealth < 1) maxHealth = 1;
        if (!Application.isPlaying) currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }
}






