using UnityEngine;

public class PlayerSpellCaster : MonoBehaviour
{
    public static PlayerSpellCaster Instance; // Allows drag-and-drop scripts to access this easily

    [Header("Placement Layer")]
    [SerializeField] private LayerMask groundLayer; // Set this to your Arena floor layer

    [Header("Player Elixir")]
    public float currentElixir = 5f;
    public float maxElixir = 10f;
    public float elixirRegenSpeed = 0.35f;

    void Awake()
    {
        // Set up the static instance
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        // Simple elixir regeneration logic matching your game style
        if (currentElixir < maxElixir)
        {
            currentElixir += elixirRegenSpeed * Time.deltaTime;
        }
    }

    /// <summary>
    /// Call this from your Drag-and-Drop script when the spell card is released!
    /// </summary>
    /// <param name="spellPrefab">The prefab of the fireball/arrows to spawn</param>
    /// <param name="elixirCost">How much elixir the spell costs</param>
    /// <param name="dropScreenPosition">Pass Input.mousePosition or the touch position here</param>
    /// <returns>Returns true if the cast was successful, false if it failed</returns>
    public bool TryCastSpell(GameObject spellPrefab, int elixirCost, Vector3 dropScreenPosition)
    {
        // 1. Check Elixir economy
        if (currentElixir < elixirCost)
        {
            Debug.Log("Not enough Elixir to cast this spell!");
            return false; 
        }

        // 2. Shoot a ray from the camera through the exact drop location
        Ray ray = Camera.main.ScreenPointToRay(dropScreenPosition);
        RaycastHit hit;

        // 3. Convert screen drop point to the actual ground coordinates
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            Vector3 targetFloorPosition = hit.point;

            // 4. Deduct the cost
            currentElixir -= elixirCost;

            // 5. Fire! Spawn it high in the sky over the target floor spot
            SpawnSpellProjectile(spellPrefab, targetFloorPosition);
            return true;
        }

        return false;
    }

    private void SpawnSpellProjectile(GameObject prefab, Vector3 targetPos)
    {
        if (prefab == null) return;

        // Spawns 12 units up and slightly back (-4 on Z) for that classic diagonal incoming trajectory
        Vector3 spawnSkyPosition = targetPos + new Vector3(0f, 12f, -4f);

        GameObject projectileInstance = Instantiate(prefab, spawnSkyPosition, Quaternion.LookRotation(Vector3.down));

        // Pass the target coordinates down to the projectile's logic
        FireballProjectile fireballScript = projectileInstance.GetComponent<FireballProjectile>();
        if (fireballScript != null)
        {
            fireballScript.InitializeTarget(targetPos);
        }
    }
}