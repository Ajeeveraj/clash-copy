using UnityEngine;

public class TroopHealth : MonoBehaviour
{
    public int maxHealth = 200;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Troop took damage, new HP = " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Troop died!");
        Destroy(gameObject);
    }
}


