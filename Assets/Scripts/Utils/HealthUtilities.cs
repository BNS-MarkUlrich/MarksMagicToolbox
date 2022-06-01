using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// WIP, kan nog niet op elk Gameobject worden aangeropen in Unity Events
/// </summary>
public static class HealthUtilities
{
    private static bool HasHealthComponent(this GameObject gameObject)
    {
        var hasHealthComponent = gameObject.GetComponent<Health>() != null;
        return hasHealthComponent;
    }
    
    public static void AddHealth(this GameObject gameObject, float health)
    {
        if (!HasHealthComponent(gameObject)) return;

        var healthComponent = gameObject.GetComponent<Health>();
        healthComponent.Heal(health);
    }
    
    public static void TakeDamage(this GameObject gameObject, float health)
    {
        if (!HasHealthComponent(gameObject)) return;

        var healthComponent = gameObject.GetComponent<Health>();
        healthComponent.TakeDamage(health);
    }

    public static bool IsDead(this GameObject gameObject)
    {
        if (!HasHealthComponent(gameObject)) return false;
        
        var isDead = gameObject.GetComponent<Health>().GetHealth <= 0;
        return isDead;
    }
}