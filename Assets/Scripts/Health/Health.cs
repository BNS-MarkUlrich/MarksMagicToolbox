using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float health = 100f;
    [SerializeField] private UnityEvent onHealthAdded = new UnityEvent();
    private float maxHealth;
    
    [Header("Damage")]
    [SerializeField] private UnityEvent onDamageTaken = new UnityEvent();
    [SerializeField] private UnityEvent onDie = new UnityEvent();
    
    public float GetHealth => health;
    public float GetMaxHealth => maxHealth;

    [HideInInspector] public bool isDead;

    private void Start()
    {
        InitHealth();
    }

    private void InitHealth()
    {
        maxHealth = health;
    }

    public void Heal(float healAmount)
    {
        var isMaxHealth = health >= maxHealth;
        if (isMaxHealth) return;
        
        health += healAmount;
        onHealthAdded?.Invoke();
        
        var resurrected = health > 0;
        if (resurrected) isDead = false;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        
        health -= damage;
        onDamageTaken?.Invoke();
        
        var hasDied = health <= 0;
        if (hasDied) Die();
    }

    private void Die()
    {
        isDead = true;
        onDie?.Invoke();
    }
}