using System.Collections.Generic;
using UnityEngine;

public class PlayerDeckManager : MonoBehaviour
{
    [Header("Deck Setup")]
    public List<CardData> startingDeck = new List<CardData>(); // Put your 5 cards here
    
    [Header("Your 4 UI Slots")]
    public CardDragger[] cardSlots = new CardDragger[4]; 
    
    private List<CardData> drawPile = new List<CardData>();
    private List<CardData> discardPile = new List<CardData>();

    void Start()
    {
        // 1. Load the deck
        drawPile.AddRange(startingDeck);
        ShuffleDrawPile();
        
        // 2. Fill the 4 CardSlots on your screen
        for (int i = 0; i < 4; i++)
        {
            cardSlots[i].LoadNewCard(drawPile[0]);
            drawPile.RemoveAt(0); // Remove from pile once drawn
        }
    }

    void ShuffleDrawPile()
    {
        for (int i = 0; i < drawPile.Count; i++)
        {
            CardData temp = drawPile[i];
            int randomIndex = Random.Range(i, drawPile.Count);
            drawPile[i] = drawPile[randomIndex];
            drawPile[randomIndex] = temp;
        }
    }

    // Called automatically by your CardDragger when a unit is spawned
    public void CardPlayed(CardDragger usedSlot, CardData playedCard)
    {
        // 1. Move the played card to the discard pile
        discardPile.Add(playedCard);

        // 2. If the draw pile is empty, recycle the discard pile
        if (drawPile.Count == 0)
        {
            drawPile.AddRange(discardPile);
            discardPile.Clear();
        }

        // 3. Push the next card from the draw pile directly into the empty UI slot
        usedSlot.LoadNewCard(drawPile[0]);
        drawPile.RemoveAt(0);
    }
}