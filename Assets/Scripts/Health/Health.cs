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
    [SerializeField] private UnityEvent onRessurected = new UnityEvent();
    
    public float GetHealth => health;
    public float GetMaxHealth => maxHealth;

    [Header("Options")] 
    [SerializeField] private bool canResurrect;
    [SerializeField] private float damage;
    [SerializeField] private float healAmount;
    [HideInInspector] public bool isDead;

    private void Start()
    {
        InitHealth();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(damage);
        }
        
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            AddHealth(healAmount);
        }
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
        isDead = true;
        onDie?.Invoke();
    }
}