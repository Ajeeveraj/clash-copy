using UnityEngine;

public class TowerHealth : MonoBehaviour
{
    public int maxHealth = 1000;
    public int currentHealth;
    public bool isDestroyed = false;

    private MeshRenderer[] renderers;

    void Start()
    {
        currentHealth = maxHealth;
        renderers = GetComponentsInChildren<MeshRenderer>();
    }

    public void TakeDamage(int amount)
    {
        if (isDestroyed) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDestroyed = true;

            // Disable tower visuals
            foreach (MeshRenderer r in renderers)
                r.enabled = false;

            Debug.Log(gameObject.name + " has been destroyed!");
        }
    }
}



