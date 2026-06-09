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

    void Awake()
    {
        // This forces the limit to 2 immediately when the game starts,
        // ignoring any old "4" saved in the Inspector.
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
        canvasGroup.blocksRaycasts = true;
        transform.position = originalPosition;

        if (Pointer.current == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Pointer.current.position.ReadValue());
        int groundLayer = LayerMask.GetMask("ground"); 

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        {
            // Check for River OR PlayerTower OR EnemyTower
            if (hit.collider.CompareTag("River") || 
                hit.collider.CompareTag("PlayerTower") || 
                hit.collider.CompareTag("EnemyTower"))
            {
                Debug.Log($"Rejected: You hit a {hit.collider.tag}. Cannot place here.");
                return;
            }

            Vector3 spawnPos = hit.point;
            
            // Snap to NavMesh
            if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 0.5f, NavMesh.AllAreas))
            {
                spawnPos = navHit.position;
            }

            if (spawnPos.z <= 2f)
            {
                if (cardManager != null && cardManager.SpendElixir(elixirCost))
                {
                    Instantiate(troopPrefab, spawnPos, Quaternion.identity);
                }
            }
        }
    }
}