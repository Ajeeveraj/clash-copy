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
    public float elixirRegenSpeed = 1f;

    public float currentElixir;

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
            currentElixir += Time.deltaTime * elixirRegenSpeed;
            currentElixir = Mathf.Min(currentElixir, maxElixir);
            
            if (elixirSlider != null) elixirSlider.value = currentElixir;
            if (elixirText != null) elixirText.text = Mathf.FloorToInt(currentElixir).ToString();
        }
    }

    // The CardDragger script calls this function to check and spend elixir
    public bool SpendElixir(float amount)
    {
        if (currentElixir >= amount)
        {
            currentElixir -= amount;
            if (elixirSlider != null) elixirSlider.value = currentElixir;
            return true; 
        }
        
        Debug.Log("Not enough elixir!");
        return false; 
    }
}