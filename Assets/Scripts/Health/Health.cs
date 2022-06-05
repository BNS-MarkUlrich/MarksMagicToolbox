using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    //[Header("Health")]
    [SerializeField] private float health = 100f;
    [SerializeField] private UnityEvent onHealthAdded = new UnityEvent();
    private float maxHealth;
    
    //[Header("Damage")]
    [SerializeField] private UnityEvent onDamageTaken = new UnityEvent();
    [SerializeField] private UnityEvent onDie = new UnityEvent();
    [SerializeField] private UnityEvent onRessurected = new UnityEvent();
    
    public float GetHealth => health;
    public float GetMaxHealth => maxHealth;
    
    [HideInInspector] public bool isDead;

    [Header("Options")] 
    [SerializeField] private bool canResurrect;
    
    private void Start()
    {
        InitHealth();
    }

    private void InitHealth()
    {
        maxHealth = health;
    }

    public void AddHealth(float healthAmount)
    {
        if (isDead && !canResurrect) return;

        var isMaxHealth = health >= maxHealth;
        if (isMaxHealth) return;
        
        health += healthAmount;
        onHealthAdded?.Invoke();

        if (canResurrect) Resurrect();
    }

    private bool CanResurrect => health > 0;

    private void Resurrect()
    {
        if (!CanResurrect) return;
        
        isDead = false;
        onRessurected?.Invoke();
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
        health = 0;
        isDead = true;
        onDie?.Invoke();
    }
}