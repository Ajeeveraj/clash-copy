using UnityEngine;

public class BillboardUpright : MonoBehaviour
{
    private Camera cam;
    private bool isFlyingUnit = false;
    private float flyingHeightOffset = 2.5f; // Adjust this number to change height above minion

    void Start()
    {
        cam = Camera.main;

        // Check if this specific health bar belongs to a flying unit
        TroopHealth parentHealth = GetComponentInParent<TroopHealth>();
        if (parentHealth != null && parentHealth.isFlying)
        {
            isFlyingUnit = true;
        }
    }

    void LateUpdate()
    {
        if (cam == null) return;

        // 1. Handle regular billboard rotation facing the camera
        Vector3 dir = cam.transform.position - transform.position;
        dir.y = 0f; // lock vertical tilt
        transform.rotation = Quaternion.LookRotation(-dir);

        // 2. MINION ONLY POSITION FIX: Force the bar to hover in the air 
        // even if the parent object's pivot point is dragging on the ground
        if (isFlyingUnit)
        {
            transform.localPosition = new Vector3(0f, flyingHeightOffset, 0f);
        }
    }
}