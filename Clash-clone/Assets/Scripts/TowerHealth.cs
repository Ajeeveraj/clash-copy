using UnityEngine;

public class TowerHealth : MonoBehaviour
{
    [Header("Tower Settings")]
    public float maxHealth = 2500f;
    public float currentHealth;
    
    [Tooltip("Check this box in the Inspector ONLY for the King Tower")]
    public bool isKingTower = false;

    // Added this variable to fix the TroopAttack error!
    public bool isDestroyed = false;

    void Start()
    {
        // Initialize health when the game starts
        currentHealth = maxHealth;
        isDestroyed = false;
    }

    public void TakeDamage(float damageAmount)
    {
        // If it's already dead, don't take more damage
        if (isDestroyed) return;

        currentHealth -= damageAmount;
        
        Debug.Log($"{gameObject.name} took {damageAmount} damage! Remaining Health: {currentHealth}");

        // Check if the tower should be destroyed
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Tell the TroopAttack script that this tower is officially dead!
        isDestroyed = true;

        if (isKingTower)
        {
            Debug.Log("King Tower down! Game Over!");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Princess Tower down!");
            Destroy(gameObject);
        }
    }
}