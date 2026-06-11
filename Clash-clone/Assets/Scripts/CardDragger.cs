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

        // 1. FIX THE SLOT FREEZE: Re-enable mouse clicks and snap the UI back to center
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
        }
        transform.localPosition = Vector3.zero;

        // 2. Safe floor coordinate math to prevent floating/NavMesh errors
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 spawnPos = ray.GetPoint(rayDistance);

            // 3. Game logic checks
            if (spawnPos.z <= 2f)
            {
                if (cardManager != null && cardManager.SpendElixir(elixirCost))
                {
                    // 4. SQUAD SPAWNING WITH OFFSETS RESTORED
                    if (currentCardData.cardName.Contains("Archer"))
                    {
                        Instantiate(troopPrefab, spawnPos + new Vector3(-0.75f, 0f, 0f), Quaternion.identity);
                        Instantiate(troopPrefab, spawnPos + new Vector3(0.75f, 0f, 0f), Quaternion.identity);
                    }
                    else if (currentCardData.cardName.Contains("Minion"))
                    {
                        Instantiate(troopPrefab, spawnPos + new Vector3(-1f, 0f, 0f), Quaternion.identity);
                        Instantiate(troopPrefab, spawnPos, Quaternion.identity);
                        Instantiate(troopPrefab, spawnPos + new Vector3(1f, 0f, 0f), Quaternion.identity);
                    }
                    else
                    {
                        Instantiate(troopPrefab, spawnPos, Quaternion.identity);
                    }

                    // 5. Cycle the card out of the hand
                    PlayerDeckManager deckManager = Object.FindAnyObjectByType<PlayerDeckManager>();
                    if (deckManager != null)
                    {
                        deckManager.CardPlayed(this, currentCardData);
                    }
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