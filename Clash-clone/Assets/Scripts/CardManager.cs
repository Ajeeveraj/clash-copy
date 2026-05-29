using UnityEngine;
using UnityEngine.UI; // Required for using the Image component

public class CardManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Image elixirFillImage; // Drag your 'Fill' object here in the Inspector

    [Header("Elixir Settings")]
    public float currentElixir = 0f;
    public float maxElixir = 10f;
    public float elixirRegenSpeed = 1f; // How fast elixir builds up per second

    void Update()
    {
        RegenerateElixir();
        UpdateElixirUI();
    }

    void RegenerateElixir()
    {
        if (currentElixir < maxElixir)
        {
            currentElixir += elixirRegenSpeed * Time.deltaTime;
            
            // Clamp the value so it never goes past the maximum limit
            currentElixir = Mathf.Clamp(currentElixir, 0f, maxElixir);
        }
    }

    void UpdateElixirUI()
    {
        if (elixirFillImage != null)
        {
            // Converts elixir to a percentage between 0.0 and 1.0
            elixirFillImage.fillAmount = currentElixir / maxElixir;
        }
    }
}