using UnityEngine;
using TMPro; 

public class CardDisplay : MonoBehaviour
{
    [Header("UI Text References")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;

    // This is the function your Deck/Hand Manager will call when dealing a card!
    public void SetupCard(string newTroopName, int newElixirCost)
    {
        if (nameText != null)
        {
            nameText.text = newTroopName;
        }

        if (costText != null)
        {
            costText.text = newElixirCost.ToString();
        }
    }
}