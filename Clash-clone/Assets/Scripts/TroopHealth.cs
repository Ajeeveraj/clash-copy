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
        Debug.Log($"{name} took {amount} damage. HP: {currentHealth}/{maxHealth}");

        if (healthBar != null) healthBar.Set(currentHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
            Die();
        }
    }

    private void Die()
    {
        foreach (var r in renderers) if (r != null) r.enabled = false;
        foreach (var c in colliders) if (c != null) c.enabled = false;
        Debug.Log($"{name} died.");
    }

    void OnValidate()
    {
        if (maxHealth < 1) maxHealth = 1;
        if (!Application.isPlaying) currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }
}












