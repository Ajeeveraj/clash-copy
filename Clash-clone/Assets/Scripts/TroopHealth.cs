using UnityEngine;
using UnityEngine.UI;

public class TroopHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Optional UI")]
    public Image healthFillImage; // assign in inspector or auto-find

    [Header("Death")]
    public bool destroyOnDeath = true;
    public float destroyDelay = 0.5f;

    [Header("Pooling")]
    public bool usesPooling = false; // set true if you reuse troops via pooling

    void Awake()
    {
        currentHealth = maxHealth;
        AutoAssignHealthFill();
        UpdateHealthUI();
        Debug.Log($"[TroopHealth] Awake {gameObject.name} id={gameObject.GetInstanceID()} maxHealth={maxHealth}");
    }

    void OnEnable()
    {
        // If using pooling, ensure health resets when re-enabled
        if (usesPooling)
        {
            currentHealth = maxHealth;
            UpdateHealthUI();
            EnableRenderersAndColliders(true);
        }
    }

    private void AutoAssignHealthFill()
    {
        if (healthFillImage != null) return;

        // Common child path: HealthBar/Fill
        Transform fill = transform.Find("HealthBar/Fill");
        if (fill != null)
        {
            healthFillImage = fill.GetComponent<Image>();
            return;
        }

        // Fallback: first Image in children
        healthFillImage = GetComponentInChildren<Image>();
    }

    public void TakeDamage(float amount)
    {
        Debug.Log($"[TroopHealth] TakeDamage called on {gameObject.name} id={gameObject.GetInstanceID()} amount={amount} before={currentHealth}");

        if (amount <= 0f) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Debug.Log($"[TroopHealth] {gameObject.name} health now {currentHealth}/{maxHealth}");

        UpdateHealthUI();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthFillImage != null)
        {
            float fill = (maxHealth > 0f) ? (currentHealth / maxHealth) : 0f;
            healthFillImage.fillAmount = fill;
        }
    }

    private void Die()
    {
        Debug.Log($"[TroopHealth] Die() called on {gameObject.name} id={gameObject.GetInstanceID()}");

        // Disable physical interaction and visible 3D renderers but keep UI intact
        foreach (var c in GetComponentsInChildren<Collider>()) c.enabled = false;
        foreach (var mr in GetComponentsInChildren<MeshRenderer>()) mr.enabled = false;
        foreach (var sr in GetComponentsInChildren<SpriteRenderer>()) sr.enabled = false;

        // Optionally fade or hide world-space UI via CanvasGroup if present
        CanvasGroup cg = GetComponentInChildren<CanvasGroup>();
        if (cg != null) cg.alpha = 0f;

        if (usesPooling)
        {
            // For pooling, deactivate and let pool manager reuse it
            gameObject.SetActive(false);
            return;
        }

        if (destroyOnDeath)
        {
            Destroy(gameObject, destroyDelay);
        }
        else
        {
            // If not destroying, disable behaviours to stop logic
            var behaviours = GetComponents<MonoBehaviour>();
            foreach (var b in behaviours) b.enabled = false;
        }
    }

    // Public helper to heal or reset health (useful for pooling)
    public void HealToFull()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        EnableRenderersAndColliders(true);
    }

    private void EnableRenderersAndColliders(bool enabled)
    {
        foreach (var c in GetComponentsInChildren<Collider>()) c.enabled = enabled;
        foreach (var mr in GetComponentsInChildren<MeshRenderer>()) mr.enabled = enabled;
        foreach (var sr in GetComponentsInChildren<SpriteRenderer>()) sr.enabled = enabled;

        CanvasGroup cg = GetComponentInChildren<CanvasGroup>();
        if (cg != null) cg.alpha = enabled ? 1f : 0f;
    }

    // Debug helper: apply damage from inspector context menu
    [ContextMenu("Debug Apply 10 Damage")]
    private void DebugApply10()
    {
        TakeDamage(10f);
    }
}








