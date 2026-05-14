// TowerDamageTester.cs
using UnityEngine;

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

[DisallowMultipleComponent]
public class TowerDamageTester : MonoBehaviour
{
    public int damagePerHit = 100;
    private TowerHealth th;

    void Awake()
    {
        th = GetComponent<TowerHealth>();
        if (th == null) Debug.LogError("TowerDamageTester: TowerHealth not found on this GameObject.");
    }

    void Update()
    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        // New Input System active (old InputManager disabled)
        if (Keyboard.current != null && Keyboard.current.dKey.wasPressedThisFrame)
        {
            ApplyDamage();
        }
#else
        // Old Input Manager or Both
        if (Input.GetKeyDown(KeyCode.D))
        {
            ApplyDamage();
        }
#endif
    }

    private void ApplyDamage()
    {
        if (th != null)
        {
            th.TakeDamage(damagePerHit);
            Debug.Log($"TowerDamageTester: applied {damagePerHit} damage to {gameObject.name}");
        }
    }
}


