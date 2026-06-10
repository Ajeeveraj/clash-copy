using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Clash/Card Data")]
public class CardData : ScriptableObject
{
    public string cardName;
    public int elixirCost;
    public GameObject prefab;
    
    // Use this temporary color for testing instead of a sprite!
    public Color cardColor = Color.white; 
}