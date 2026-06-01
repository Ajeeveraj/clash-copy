using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardManager : MonoBehaviour
{
    [Header("UI Reference")]
    public Slider elixirSlider;
    public TextMeshProUGUI elixirText; 

    [Header("Settings")]
    public float maxElixir = 10f;
    public float startingElixir = 5f;
    public float elixirRegenSpeed = 1f; // 1 elixir per second

    private float currentElixir;

    void Start()
    {
        currentElixir = startingElixir;

        if (elixirSlider != null)
        {
            elixirSlider.maxValue = maxElixir;
            elixirSlider.value = currentElixir;
        }
    }

    void Update()
    {
        if (currentElixir < maxElixir)
        {
            // Regenerate elixir over time
            currentElixir += Time.deltaTime * elixirRegenSpeed;
            currentElixir = Mathf.Min(currentElixir, maxElixir);
            
            // Update the Slider
            if (elixirSlider != null)
            {
                elixirSlider.value = currentElixir;
            }

            // Update the Text Number
            if (elixirText != null)
            {
                elixirText.text = Mathf.FloorToInt(currentElixir).ToString();
            }
        }
    }

    // Call this when you deploy a card!
    public bool SpendElixir(float amount)
    {
        if (currentElixir >= amount)
        {
            currentElixir -= amount;
            if (elixirSlider != null)
            {
                elixirSlider.value = currentElixir;
            }
            return true;
        }
        
        Debug.Log("Not enough elixir!");
        return false;
    }
}