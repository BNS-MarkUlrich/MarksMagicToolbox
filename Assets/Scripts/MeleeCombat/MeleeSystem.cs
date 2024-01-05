using System;
using UnityEngine;

public class MeleeSystem : MonoBehaviour
{
    [SerializeField] private BaseWeapon myWeapon;

    private Vector2 attackDirection;
    private Vector2 mouseDirection;

    public Agent OwningAgent { get; set; }
    public Vector2 AttackDirection => attackDirection;

    private void Start() 
    {
        myWeapon = GetComponentInChildren<BaseWeapon>();
        myWeapon.OwningAgent = OwningAgent; // Change to pickup event later
        myWeapon.OnHit += OnHit;
    }

    private void Update() 
    {
        if (Input.GetKey(KeyCode.Mouse0) && !myWeapon.IsAttacking)
        {
            if (!Input.GetKey(KeyCode.Mouse1))
                ChooseDirection();
            
            myWeapon.Block();
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
            myWeapon.Attack();
    }

    private void ChooseDirection()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        mouseDirection.Set(mouseX, mouseY);
        mouseDirection.Normalize();

        // snap attack directon to 4 cardinal directions
        if (Mathf.Abs(mouseDirection.x) > Mathf.Abs(mouseDirection.y))
            attackDirection.Set(mouseDirection.x, 0f);
        else if (Mathf.Abs(mouseDirection.y) > Mathf.Abs(mouseDirection.x))
            attackDirection.Set(0f, mouseDirection.y);

        attackDirection.Normalize();

        myWeapon.SetAttackDirection(attackDirection);        
        myWeapon.SetBlockDirection(attackDirection);
    }

    public void SetAttackDirection(Vector2 direction)
    {
        attackDirection = direction;
    }

    private void OnHit(HitEvent hitEvent)
    {
        if (hitEvent.type == HitEventType.Missed)
        {
            print($"{hitEvent.aggressor.name}'s attack missed");
            return;
        }

        if (hitEvent.type == HitEventType.Blocked)
        {
            print($"{hitEvent.aggressor.name}'s attack was blocked");
            return;
        }

        hitEvent.opponent.HealthData.TakeDamage(hitEvent.weaponUsed.Damage);
        hitEvent.opponent.MyRigidbody.AddForce
        (
            -hitEvent.attackDirection, 
            ForceMode.Impulse
        );

        print($"{hitEvent.opponent.name} took {hitEvent.weaponUsed.Damage} {hitEvent.weaponUsed.TypeOfDamage} damage from {hitEvent.aggressor.name}'s {hitEvent.weaponUsed.name}");
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, attackDirection);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, mouseDirection);
    }
}
