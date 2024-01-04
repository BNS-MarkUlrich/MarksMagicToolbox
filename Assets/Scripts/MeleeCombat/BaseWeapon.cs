using System;
using System.Collections;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    [SerializeField] protected float damage = 10f;
    [SerializeField] private float blockingAngle = 45f;
    [SerializeField] protected float weaponLength = 1f;
    [SerializeField] protected float weaponSpeed = 150f;
    [SerializeField] protected float finishAttackAngle = 120f;
    [SerializeField] protected float attackCooldown = 0.1f;
    
    protected bool isAttacking;
    protected bool isBlocked;
    protected bool isFinishedSwinging = true;
    protected float currentAttackAngle;
    protected Quaternion originalRotation;
    protected Rigidbody myRigidbody;

    public float Damage => damage;
    public float WeaponLength => weaponLength;
    public Vector3 TopPoint => BottomPoint + transform.up * (weaponLength);
    public Vector3 BottomPoint => transform.position + transform.up - transform.up;
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

        OnAttack += Attack;
        OnHit += StopSwing;
    }

    private void OnDisable() 
    {
        OnAttack -= Attack;
        OnHit -= StopSwing;
    }

    private void FixedUpdate() 
    {
        if (isAttacking)
            Swing();
        
        if (isBlocked)
            ResetSwing();
    }

    protected void TriggerHitEvent(HitEventTypes type, Agent opponent, Vector3 hitPoint)
    {
        HitEvent hitEvent = new()
        {
            type = type,
            aggressor = OwningAgent,
            opponent = opponent,
            weaponUsed = this,
            attackDirection = OwningAgent.MeleeSystem.AttackDirection
        };

        OnHit?.Invoke(hitEvent);
    }

    public void Attack()
    {
        if (isAttacking) 
            return;

        isAttacking = true;

        // TODO: Add animation here
        originalRotation = transform.rotation;
    }
    
    public void Block()
    {
        if (isAttacking) 
            return;
    }

    public void SetAttackDirection(Vector2 direction)
    {
        if (isAttacking || !isFinishedSwinging)
            return;

        float angle = Mathf.Atan2(-direction.x, direction.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, angle), (WeaponSpeed / 10f) * Time.deltaTime);
        //myWeapon.transform.rotation = Quaternion.Euler(0f, 0f, MathF.Round(angle / 45f) * 45f);
    }

    public void SetBlockDirection(Vector2 direction)
    {
        
    }

    protected void Swing()
    {
        isFinishedSwinging = false;
        float angleStep = weaponSpeed * Time.deltaTime;

        if (currentAttackAngle < finishAttackAngle)
        {
            currentAttackAngle += angleStep;
            transform.Rotate(-Vector3.left * angleStep);
            return;
        }

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
            currentAttackAngle = 0f;
            isAttacking = false;
            isBlocked = true;
        }
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(TopPoint, 0.1f);
        Gizmos.DrawRay(TopPoint, -transform.up * (weaponLength / 2));
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(BottomPoint, 0.1f);
        Gizmos.DrawRay(BottomPoint, transform.up * (weaponLength / 2));
        
        if (OwningAgent != null)
        {
            Vector2 attackAngleVector = Quaternion.Euler(0f, 0f, blockingAngle) * OwningAgent.MeleeSystem.AttackDirection;
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(OwningAgent.transform.position, attackAngleVector * 2f);
            attackAngleVector = Quaternion.Euler(0f, 0f, -blockingAngle) * OwningAgent.MeleeSystem.AttackDirection;
            Gizmos.DrawRay(OwningAgent.transform.position, attackAngleVector * 2f);
        }
    }
}
