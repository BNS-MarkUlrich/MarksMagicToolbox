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
    public Vector3 TopPoint => BottomPoint + transform.up * WeaponLength;
    public Vector3 MiddlePoint => transform.position + transform.up * (WeaponLength / 5);
    public Vector3 BottomPoint => transform.position + transform.up - transform.up;
    public float SwingSpeed => WeaponAttributes.WeaponSpeed / WeaponAttributes.WeaponLength / WeaponAttributes.WeaponWeight;
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

    protected void TriggerHitEvent(HitEventType type, Agent opponent, Vector3 hitPoint)
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
        agentsHit.Clear();

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

        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);
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
            transform.Rotate(-Vector3.left * angleStep);
            
            // detect if there is a gameobject between the top, middle, and bottom points of the weapon
            RaycastHit hit;
            if (Physics.Raycast(BottomPoint, MiddlePoint - BottomPoint, out hit, WeaponLength / 5))
            {
                TriggerHitEvent(HitEventType.Blocked, null, hit.point);
                return;
            }

            if (Physics.Raycast(MiddlePoint, TopPoint - MiddlePoint, out hit, WeaponLength / (WeaponLength / 5)))
            {
                if (hit.collider.gameObject != OwningAgent.gameObject)
                {
                    if (hitAgents.ContainsKey(hit.collider) && !agentsHit.Contains(hitAgents[hit.collider]))
                    {
                        agentsHit.Add(hitAgents[hit.collider]);
                        TriggerHitEvent(HitEventType.Hit, hitAgents[hit.collider], hit.point);
                        return;
                    }

                    if (hit.collider.TryGetComponent(out Agent agent))
                    {
                        if (agentsHit.Contains(agent))
                            return;
                        
                        hitAgents.Add(hit.collider, agent);
                        agentsHit.Add(agent);
                        TriggerHitEvent(HitEventType.Hit, agent, hit.point);
                        return;
                    }
                }

                TriggerHitEvent(HitEventType.Missed, null, hit.point);
            }
            return;
        }

        currentAttackAngle = 0f;
        isAttacking = false;
        isBlocked = true;
    }

    protected void ResetSwing()
    {
        float angleStep = SwingSpeed * 30 * Time.deltaTime;

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
        if (hitEvent.type == HitEventType.Blocked || hitEvent.type == HitEventType.Missed)
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
        Gizmos.DrawRay(TopPoint, -transform.up * (WeaponLength / 2));

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(MiddlePoint, 0.1f);
        //Gizmos.DrawRay(MiddlePoint + transform.up * (weaponLength / 2), -transform.up * weaponLength);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(BottomPoint, 0.1f);
        Gizmos.DrawRay(BottomPoint, transform.up * (WeaponLength / 2));
        
        if (OwningAgent != null)
        {
            Vector2 attackAngleVector = Quaternion.Euler(0f, 0f, blockingAngle) * OwningAgent.MeleeSystem.AttackDirection;
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(OwningAgent.transform.position, attackAngleVector);
            attackAngleVector = Quaternion.Euler(0f, 0f, -blockingAngle) * OwningAgent.MeleeSystem.AttackDirection;
            Gizmos.DrawRay(OwningAgent.transform.position, attackAngleVector);
        }
    }
}
