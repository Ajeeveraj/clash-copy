using UnityEngine;

public class TowerHealth : MonoBehaviour
{
    public int maxHealth = 1000;
    public int currentHealth;

    public bool isDestroyed = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (isDestroyed) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            DestroyTower();
        }
    }

    void DestroyTower()
    {
        isDestroyed = true;
        Debug.Log(gameObject.name + " has been destroyed!");

        // Disable the tower visually
        gameObject.SetActive(false);
    }
}

