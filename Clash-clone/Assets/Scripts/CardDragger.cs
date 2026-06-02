using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem; 

public class CardDragger : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [Header("Card Settings")]
    public float elixirCost = 3f;
    public GameObject troopPrefab; 

    [Header("Placement Rules")]
    // Adjust this number to match where your river or centerline sits!
    public float maxDeploymentZ = 0f; 

    private Vector3 originalPosition;
    private CanvasGroup canvasGroup;
    private CardManager cardManager;

    void Start()
    {
        originalPosition = transform.position;
        cardManager = FindFirstObjectByType<CardManager>();
        
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        canvasGroup.blocksRaycasts = false; 
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true; 

        Vector2 mousePos = Pointer.current != null ? Pointer.current.position.ReadValue() : eventData.position;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // NEW RULE: Check if the drop location's Z coordinate is on your half!
            if (hit.point.z <= maxDeploymentZ)
            {
                // Try to spend elixir and spawn
                if (cardManager != null && cardManager.SpendElixir(elixirCost))
                {
                    Instantiate(troopPrefab, hit.point, Quaternion.identity);
                }
            }
            else
            {
                Debug.Log("Cannot deploy on the enemy's side of the field!");
            }
        }

        transform.position = originalPosition;
    }
}