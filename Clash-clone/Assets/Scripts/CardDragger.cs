using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class CardDragger : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [Header("Card Settings")]
    public float elixirCost = 3f;
    public GameObject troopPrefab; 

    [Header("Placement Rules")]
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

        // 1. Raycast hits EVERYTHING (no layer mask)
        if (Physics.Raycast(ray, out hit, 100f))
        {
            int groundLayerIndex = LayerMask.NameToLayer("ground");

            // 2. We check if the FIRST thing we hit was the ground
            if (hit.collider.gameObject.layer == groundLayerIndex)
            {
                if (hit.point.z <= maxDeploymentZ)
                {
                    if (cardManager != null && cardManager.SpendElixir(elixirCost))
                    {
                        Vector3 spawnPos = hit.point;
                        NavMeshHit navHit;
                        
                        // Small search radius so they don't teleport across the map
                        if (NavMesh.SamplePosition(hit.point, out navHit, 2.0f, NavMesh.AllAreas))
                        {
                            spawnPos = navHit.position;
                        }

                        Instantiate(troopPrefab, spawnPos, Quaternion.identity);
                    }
                }
                else
                {
                    Debug.Log("Placement rejected: Enemy side!");
                }
            }
            else
            {
                // 3. If you hit the new River BoxCollider, it triggers this and stops!
                Debug.Log("Placement rejected: You hit " + hit.collider.gameObject.name + " which is not ground.");
            }
        }
        else
        {
            Debug.Log("Placement rejected: Hit nothing.");
        }

        transform.position = originalPosition;
    }
}