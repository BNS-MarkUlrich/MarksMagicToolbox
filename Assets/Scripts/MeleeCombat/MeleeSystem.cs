using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSystem : MonoBehaviour
{
    [SerializeField] private BaseWeapon myWeapon;
    [SerializeField] private float blockingAngle = 45f;

    private Vector2 attackDirection;
    private Vector2 mouseDirection;

    public Agent OwningAgent { get; set; }
    public Vector2 AttackDirection => attackDirection;

    private void Start() 
    {
        myWeapon.OwningAgent = OwningAgent; // Change to pickup event later
        myWeapon.OnHit += OnHit;
    }

    private void Update() 
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (!Input.GetKey(KeyCode.Mouse1))
                ChooseDirection();
            
            myWeapon.Block();

            if (Input.GetKeyUp(KeyCode.Mouse0))
                myWeapon.Attack();
        }
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

        SetSwordRotation();
    }

    public void SetAttackDirection(Vector2 direction)
    {
        attackDirection = direction;
    }

    // set sword rotation to attack direction, using the player position as pivot
    // use a lerp to smooth the rotatio
    private void SetSwordRotation()
    {
        float angle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;
        myWeapon.transform.rotation = Quaternion.Lerp(myWeapon.transform.rotation, Quaternion.Euler(0f, 0f, angle), 0.5f);
    }

    private void OnHit(HitEvent hitEvent)
    {
        if (hitEvent.type == HitEventTypes.Blocked)
        {
            print($"{hitEvent.aggressor.name}'s attack was blocked by {hitEvent.opponent.name}");
            return;
        }

        hitEvent.opponent.HealthData.TakeDamage(hitEvent.weaponUsed.Damage);
        hitEvent.opponent.MyRigidbody.AddForce
        (
            hitEvent.attackDirection * hitEvent.weaponUsed.WeaponSpeed, 
            ForceMode.Impulse
        );

        print($"{hitEvent.opponent.name} took {hitEvent.weaponUsed.Damage} damage from {hitEvent.aggressor.name}'s {hitEvent.weaponUsed.name}");
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, attackDirection * 2f);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, mouseDirection * 2f);
        
        Vector2 attackAngleVector = Quaternion.Euler(0f, 0f, blockingAngle) * attackDirection;
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position, attackAngleVector * 2f);
        attackAngleVector = Quaternion.Euler(0f, 0f, -blockingAngle) * attackDirection;
        Gizmos.DrawRay(transform.position, attackAngleVector * 2f);
    }
}
