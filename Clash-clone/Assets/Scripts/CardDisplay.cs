using UnityEngine;
using TMPro; // Uses TextMeshPro for clean, crisp UI text

public class CardDisplay : MonoBehaviour
{
    [Header("Card Data Data")]
    public string troopName;
    public int elixirCost;

    [Header("UI Text References")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;

    void Start()
    {
        UpdateCardUI();
    }

    // Call this to update the text elements on the screen
    public void UpdateCardUI()
    {
        if (nameText != null)
        {
            nameText.text = troopName;
        }

        if (costText != null)
        {
            costText.text = elixirCost.ToString();
        }
    }
}