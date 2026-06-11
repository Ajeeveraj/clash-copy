using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class CardDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Card Settings")]
    public float elixirCost = 3f;
    public GameObject troopPrefab;

    [Header("Deployment Zone")]
    public float minDeploymentZ = -20f;
    public float maxDeploymentZ = 2f;

    private Vector3 originalPosition;
    private CanvasGroup canvasGroup;
    private CardManager cardManager;
    private CardData currentCardData;
    
    [Header("UI References")]
    public Image cardImageComponent; 
    public Text costText; 

    void Awake()
    {
        maxDeploymentZ = 2f;
    }

    void Start()
    {
        originalPosition = transform.position;
        cardManager = Object.FindFirstObjectByType<CardManager>();
        
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) 
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (currentCardData == null) return;

        // FIX: Using eventData to find the world position directly
        // (We use 10f for Z because that is usually the distance from the camera)
        Vector3 spawnPos = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, 10f));

        // 1. Check if the player is dropping the card on their side of the arena
        if (spawnPos.z <= 2f)
        {
            // 2. Check if the player has enough Elixir and deduct it
            if (cardManager != null && cardManager.SpendElixir(elixirCost))
            {
                // SQUAD CHECKLIST
                if (currentCardData.cardName.Contains("Archer"))
                {
                    Instantiate(troopPrefab, spawnPos + new Vector3(-0.75f, 0f, 0f), Quaternion.identity);
                    Instantiate(troopPrefab, spawnPos + new Vector3(0.75f, 0f, 0f), Quaternion.identity);
                }
                else if (currentCardData.cardName.Contains("Minions"))
                {
                    Instantiate(troopPrefab, spawnPos + new Vector3(-1.0f, 0f, 0f), Quaternion.identity);
                    Instantiate(troopPrefab, spawnPos, Quaternion.identity);
                    Instantiate(troopPrefab, spawnPos + new Vector3(1.0f, 0f, 0f), Quaternion.identity);
                }
                else
                {
                    Instantiate(troopPrefab, spawnPos, Quaternion.identity);
                }

                // 3. Cycle the card
                PlayerDeckManager deckManager = Object.FindFirstObjectByType<PlayerDeckManager>();
                if (deckManager != null)
                {
                    deckManager.CardPlayed(this, currentCardData);
                }
            }
        }
    }

    public void LoadNewCard(CardData newCard)
    {
        this.currentCardData = newCard;
        this.elixirCost = newCard.elixirCost;
        this.troopPrefab = newCard.prefab;

        if (cardImageComponent != null)
        {
            cardImageComponent.color = newCard.cardColor;
        }

        if (costText != null)
        {
            costText.text = newCard.elixirCost.ToString();
        }
    }
}