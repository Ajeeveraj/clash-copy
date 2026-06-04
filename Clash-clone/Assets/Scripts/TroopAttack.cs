using UnityEngine;
using System.Collections.Generic;

public class TroopAttack : MonoBehaviour
{
    [Header("Team Settings")]
    public bool isEnemy = false; 

    [Header("Targeting Settings")]
    public float attackRange = 2.5f; 
    private GameObject currentTarget;

    [Header("Combat Settings")]
    public int damage = 10; 
    public float attackSpeed = 1.5f; 
    private float attackCooldown;

    void Update()
    {
        // FIX: If the target is destroyed, dump the reference instantly!
        if (currentTarget != null)
        {
            TowerHealth towerHealth = currentTarget.GetComponent<TowerHealth>();
            if (towerHealth != null && towerHealth.isDestroyed)
            {
                currentTarget = null; // Drop dead target
            }
        }

        // If we have no target, find a new valid one immediately
        if (currentTarget == null)
        {
            FindTarget();
        }
        else
        {
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
            
            if (distanceToTarget <= attackRange)
            {
                attackCooldown -= Time.deltaTime;
                if (attackCooldown <= 0f)
                {
                    AttackTarget();
                    attackCooldown = attackSpeed; 
                }
            }
        }
    }

    void FindTarget()
    {
        List<string> targetTags = new List<string>();

        if (isEnemy)
        {
            targetTags.Add("Troop");
            targetTags.Add("PlayerTower"); 
        }
        else
        {
            targetTags.Add("EnemyTroop");
            targetTags.Add("EnemyTower"); 
        }

        List<GameObject> potentialTargets = new List<GameObject>();
        foreach (string tag in targetTags)
        {
            GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);
            potentialTargets.AddRange(objectsWithTag);
        }

        GameObject closestTarget = null;
        float shortestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject potentialTarget in potentialTargets)
        {
            if (potentialTarget != null)
            {
                TowerHealth towerHealth = potentialTarget.GetComponent<TowerHealth>();
                if (towerHealth != null && towerHealth.isDestroyed)
                {
                    continue; 
                }

                float distanceToEnemy = Vector3.Distance(currentPosition, potentialTarget.transform.position);
                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    closestTarget = potentialTarget;
                }
            }
        }

        currentTarget = closestTarget;
    }

    void AttackTarget()
    {
        if (currentTarget == null) return;

        // FIX: Look on the object, its parents, AND its children for TowerHealth
        TowerHealth tower = currentTarget.GetComponentInParent<TowerHealth>();
        if (tower == null)
        {
            tower = currentTarget.GetComponentInChildren<TowerHealth>();
        }

        if (tower != null)
        {
            tower.TakeDamage(damage); 
            // This log matches what you see for Princess towers
            Debug.Log($"{gameObject.name} attacked Tower for {damage} damage!"); 
        }
        else
        {
            // If it STILL fails, it will tell you exactly what object it's hitting
            Debug.LogWarning($"CRITICAL: {gameObject.name} is hitting {currentTarget.name}, but absolutely no TowerHealth script exists anywhere on its hierarchy!");
        }
    }
}


