using UnityEngine;
using UnityEngine.UI;

public class TroopHealth : MonoBehaviour
{
    public int maxHealth = 200;
    public int currentHealth;

    public GameObject healthBarPrefab;   // Drag your prefab here
    private Image healthFill;            // The green bar
    private Transform barTransform;      // The whole bar

    void Start()
{
    currentHealth = maxHealth;

    // Instantiate as a child of the troop
    GameObject bar = Instantiate(healthBarPrefab, transform);
    bar.transform.localPosition = new Vector3(0, 2, 0); // above the head
    bar.transform.localRotation = Quaternion.identity;

    healthFill = bar.transform.Find("Background/Fill").GetComponent<Image>();
}


    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Update the green bar instantly
        float percent = (float)currentHealth / maxHealth;
        healthFill.fillAmount = percent;

        // If dead → destroy troop
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}




