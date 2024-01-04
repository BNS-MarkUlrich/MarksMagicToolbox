using System;
using System.Collections;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    [SerializeField] protected float damage = 10f;
    [SerializeField] protected float weaponLength = 1f;
    [SerializeField] protected float weaponSpeed = 150f;
    [SerializeField] protected float finishAttackAngle = 120f;
    [SerializeField] protected float attackCooldown = 0.1f;
    [SerializeField] protected Collider blockingCollider;
    
    protected Collider attackCollider;
    protected bool isAttacking;
    protected bool isBlocked;
    protected bool isFinishedSwinging = true;
    protected float currentAttackAngle;
    protected Quaternion originalRotation;
    protected Rigidbody myRigidbody;

    public float Damage => damage;
    public float WeaponLength => weaponLength;
    public float WeaponSpeed => weaponSpeed;
    public bool IsAttacking => isAttacking;
    public bool IsBlocked => isBlocked;
    public Rigidbody MyRigidbody => myRigidbody;

    public Agent OwningAgent { get; set; }

    public Action<HitEvent> OnHit;
    public Action OnAttack;

    protected virtual void OnEnable()
    {
        myRigidbody = GetComponent<Rigidbody>();
        attackCollider = GetComponent<Collider>();
        attackCollider.enabled = false;
        blockingCollider.enabled = false;

        OnAttack += Attack;
        OnHit += StopSwing;
    }

    private void FixedUpdate() 
    {
        if (isAttacking)
            Swing();
        
        if (isBlocked)
            ResetSwing();
    }

    protected void OnTriggerEnter(Collider other)
    {
        // Code for hitting opponents, trigger HitEvent class (data structure)
        // which encapsulates all the data needed for the event
        // Such as: who hit who, what weapon was used, how much damage was dealt, etc.
        // Also figure out way to check if the hit was blocked or not and how the code should respond to that

        // Side note: use sprint joints on opponents to make them react to hits

        if (!isAttacking)
            return;

        if (other == blockingCollider)
        {
            TriggerHitEvent(
                HitEventTypes.Blocked,
                null,
                attackCollider.ClosestPoint(other.transform.position)
            );

            return;
        }

        if(other.TryGetComponent(out Agent opponent))
        {
            if (opponent.AgentId == OwningAgent.AgentId) return;

            TriggerHitEvent(
                HitEventTypes.Hit,
                opponent, 
                attackCollider.ClosestPoint(other.transform.position)
            );

            return;
        }

        TriggerHitEvent(
            HitEventTypes.Missed,
            null,
            attackCollider.ClosestPoint(other.transform.position)
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

        isAttacking = true;
        attackCollider.enabled = true;
        blockingCollider.enabled = false;

        // TODO: Add animation here
        originalRotation = transform.rotation;
    }
    
    public void Block()
    {
        if (isAttacking) 
            return;
        
        attackCollider.enabled = false;

        // Logic here needs to be changed to check for angle of incoming attack, to prevent people moving their weapon through the collider
        blockingCollider.enabled = true;
    }

    public void SetAttackDirection(Vector2 direction)
    {
        if (isAttacking || !isFinishedSwinging)
            return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, angle), (WeaponSpeed / MyRigidbody.mass / 10f) * Time.deltaTime);
        //myWeapon.transform.rotation = Quaternion.Euler(0f, 0f, MathF.Round(angle / 45f) * 45f);
    }

    public void SetBlockDirection(Vector2 direction)
    {
        blockingCollider.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }

    protected void Swing()
    {
        isFinishedSwinging = false;
        float angleStep = weaponSpeed * Time.deltaTime;

        if (currentAttackAngle < finishAttackAngle)
        {
            currentAttackAngle += angleStep;
            transform.Rotate(-Vector3.up * angleStep);
            return;
        }

        attackCollider.enabled = false;
        blockingCollider.enabled = false;
        currentAttackAngle = 0f;
        isBlocked = true;

        isAttacking = false;
    }

    protected void ResetSwing()
    {
        float angleStep = weaponSpeed * 2 * Time.deltaTime;

        if (transform.rotation != originalRotation)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, originalRotation, angleStep);
            return;
        }

        isBlocked = false;
        isFinishedSwinging = true;
    }

    protected void StopSwing(HitEvent hitEvent)
    {
        if (hitEvent.type == HitEventTypes.Blocked || hitEvent.type == HitEventTypes.Missed)
        {
            attackCollider.enabled = false;
            blockingCollider.enabled = false;
            currentAttackAngle = 0f;

            isAttacking = false;

            isBlocked = true;
        }
    }
}
