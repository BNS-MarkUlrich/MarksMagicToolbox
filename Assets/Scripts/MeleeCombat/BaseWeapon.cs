using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseWeapon : MonoBehaviour
{
    [SerializeField] protected WeaponAttributes weaponAttributes;

    [SerializeField] private float blockingAngle = 45f;
    [SerializeField] protected float finishAttackAngle = 120f;
    [SerializeField] protected float attackCooldown = 0.1f;

    protected bool isAttacking;
    protected bool isBlocking;
    protected bool canCancelAttack;
    protected bool hasHit;
    protected bool isBlocked;
    protected bool isFinishedSwinging = true;
    protected float currentAttackAngle;
    protected Quaternion originalRotation;
    protected Rigidbody myRigidbody;
    protected Dictionary<Collider, Agent> hitAgents = new();
    protected HashSet<Agent> agentsHit = new();

    public WeaponAttributes WeaponAttributes => weaponAttributes;
    public string TypeOfDamage => DamageType.Cut.ToString(); // TODO: Add damage type selection
    public float Damage => WeaponAttributes.DamageTypes[DamageType.Cut]; // TODO: Add damage type selection
    public float WeaponLength => WeaponAttributes.WeaponLength;
    public Vector3 TopPoint => BottomPoint + transform.up * WeaponLength; // TODO: Add to Array in WeaponAttributes

    // TODO: Add CenterPoint and add to Array in WeaponAttributes

    public Vector3 HiltPoint => transform.position + transform.up * HiltRange; // TODO: Add to Array in WeaponAttributes
    public Vector3 BottomPoint => transform.position + transform.up - transform.up; // TODO: Add to Array in WeaponAttributes
    public float StrikeRange => WeaponLength - HiltRange;
    public float HiltRange => WeaponLength / (weaponAttributes.HiltPoint + 2);
    public float SwingSpeed => WeaponAttributes.WeaponSpeed / WeaponAttributes.WeaponLength / WeaponAttributes.WeaponWeight;
    public bool IsAttacking => isAttacking;
    public bool IsBlocking => isBlocking;
    public bool IsBlocked => isBlocked;
    public float BlockingAngle => blockingAngle;
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

    protected virtual void OnDisable()
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

    public void TriggerHitEvent(HitEventType type, Agent opponent, Vector3 hitPoint)
    {
        HitEvent hitEvent = new()
        {
            type = type,
            aggressor = OwningAgent,
            opponent = opponent,
            weaponUsed = this,
            hitPoint = hitPoint,
            stanceDirection = OwningAgent.MeleeSystem.CurrentDirection,
            attackDirection = OwningAgent.MeleeSystem.AttackDirection
        }; 

        OnHit?.Invoke(hitEvent);
    }

    public void Attack()
    {
        if (isAttacking) 
            return;

        isAttacking = true;
        isBlocking = false;
        agentsHit.Clear();

        // TODO: Add animation here
        originalRotation = transform.rotation;
    }
    
    public void Block()
    {
        if (isAttacking) 
            return;

        isBlocking = true;
    }

    public void SetAttackDirection(Vector2 direction)
    {
        if (isAttacking || !isFinishedSwinging)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);

        if (Quaternion.Angle(transform.rotation, targetRotation) < 15f)
        {
            transform.rotation = targetRotation;
            return;
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * SwingSpeed);
    }

    public void SetBlockDirection(Vector2 direction)
    {

    }

    protected void Swing()
    {
        isFinishedSwinging = false;
        float angleStep = SwingSpeed * 20 * Time.deltaTime;

        if (currentAttackAngle < finishAttackAngle)
        {
            currentAttackAngle += angleStep;
            canCancelAttack = currentAttackAngle < finishAttackAngle / 2;

            transform.Rotate(-Vector3.left * angleStep);
            
            CheckSwingCollisions();
            return;
        }

        currentAttackAngle = 0f;
        isAttacking = false;
        isBlocked = true;
    }

    protected void CheckSwingCollisions()
    {
        RaycastHit hit;
        if (Physics.Raycast(BottomPoint, HiltPoint - BottomPoint, out hit, HiltRange))
        {
            if (hit.collider.TryGetCachedComponent(ref hitAgents, out Agent agent))
            {
                if (!agentsHit.Contains(agent))
                {
                    agentsHit.Add(agent);
                    hasHit = false;
                }
            }

            if (!hasHit)
                TriggerHitEvent(HitEventType.Bumped, agent, hit.point);
            
            return;
        }

        if (Physics.Raycast(HiltPoint, TopPoint - HiltPoint, out hit, StrikeRange))
        {
            if (hit.collider.gameObject != OwningAgent.gameObject)
            {
                if (hit.collider.TryGetCachedComponent(ref hitAgents, out Agent agent))
                {
                    if (agentsHit.Contains(agent))
                        return;
                    
                    TriggerHitEvent(HitEventType.Hit, agent, hit.point);
                    agentsHit.Add(agent);
                    hasHit = true;
                }
                else
                {
                    TriggerHitEvent(HitEventType.Missed, null, hit.point);
                }
                return;
            }
        }
    }

    protected void ResetSwing()
    {
        float angleStep = SwingSpeed * 10 * Time.deltaTime;

        if (transform.rotation != originalRotation)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, originalRotation, angleStep);
            return;
        }

        isBlocked = false;
        isFinishedSwinging = true;
        hasHit = false;
    }

    protected void StopSwing(HitEvent hitEvent)
    {
        canCancelAttack = true;
        if (hitEvent.type is HitEventType.Blocked or HitEventType.Missed or HitEventType.Bumped)
            CancelAttack();
    }

    public void CancelAttack()
    {
        if (!canCancelAttack)
            return;
        
        currentAttackAngle = 0f;
        isAttacking = false;
        isBlocked = true;
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(TopPoint, 0.1f);
        Gizmos.DrawRay(TopPoint, -transform.up * StrikeRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(HiltPoint, 0.1f);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(BottomPoint, 0.1f);
        Gizmos.DrawRay(BottomPoint, transform.up * HiltRange);
    }
}
