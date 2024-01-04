using System;
using System.Collections;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    [SerializeField] protected float damage = 10f;
    [SerializeField] protected float weaponLength = 1f;
    [SerializeField] protected float weaponSpeed = 1f;
    [SerializeField] protected float attackCooldown = 0.5f;
    
    protected bool isAttacking;
    protected Collider attackCollider;
    protected Collider blockingCollider;

    public float Damage => damage;
    public float WeaponLength => weaponLength;
    public float WeaponSpeed => weaponSpeed;

    public Agent OwningAgent { get; set; }

    public Action<HitEvent> OnHit;

    protected virtual void OnEnable()
    {
        attackCollider.enabled = false;
        blockingCollider.enabled = false;
    }

    protected virtual void Awake()
    {
        attackCollider = GetComponent<Collider>();
        blockingCollider = GetComponentInChildren<Collider>();
    }

    protected void OnTriggerEnter(Collider other)
    {
        // Code for hitting opponents, trigger HitEvent class (data structure)
        // which encapsulates all the data needed for the event
        // Such as: who hit who, what weapon was used, how much damage was dealt, etc.
        // Also figure out way to check if the hit was blocked or not and how the code should respond to that

        // Side note: use sprint joints on opponents to make them react to hits

        if(other.TryGetComponent(out Agent opponent))
        {
            if (opponent.AgentId == OwningAgent.AgentId) return;

            TriggerHitEvent(
                HitEventTypes.Hit,
                opponent, 
                other.ClosestPoint(transform.position)
            );
        }

        TriggerHitEvent(
            HitEventTypes.Blocked, 
            null, 
            other.ClosestPoint(transform.position)
        );
    }

    protected void TriggerHitEvent(HitEventTypes type, Agent opponent, Vector3 hitPoint)
    {
        HitEvent hitEvent = new()
        {
            type = type,
            aggressor = OwningAgent,
            opponent = opponent,
            weaponUsed = this,
            hitPoint = hitPoint,
            attackDirection = OwningAgent.MeleeSystem.AttackDirection
        };

        OnHit?.Invoke(hitEvent);
    }

    public void Attack()
    {
        if (isAttacking) 
            return;
        
        attackCollider.enabled = true;
        blockingCollider.enabled = false;
        
        StartCoroutine(AttackCooldown());
    }

    public void Block()
    {
        if (isAttacking) 
            return;
        
        attackCollider.enabled = false;
        blockingCollider.enabled = true;
    }

    protected IEnumerator AttackCooldown()
    {
        isAttacking = true;
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }
}
