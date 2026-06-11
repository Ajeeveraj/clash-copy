using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Clash/Card Data")]
public class CardData : ScriptableObject
{
    public string cardName;
    public int elixirCost;
    public GameObject prefab;
    public Color cardColor = Color.white; 
    
    // Add this line! Default it to 1 for normal cards.
    public int spawnCount = 1; 
}