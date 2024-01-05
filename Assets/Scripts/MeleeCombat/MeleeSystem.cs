using System;
using UnityEngine;

public class MeleeSystem : MonoBehaviour
{
    [SerializeField] private BaseWeapon myWeapon;
    [SerializeField] private SerializableDictionary<CardinalDirections, Vector2> directionVectors;

    private Vector2 attackDirection;
    private Vector2 mouseDirection;
    private CardinalDirections currentDirection = CardinalDirections.Up;


    public Agent OwningAgent { get; set; }
    public Vector2 AttackDirection => attackDirection;
    public CardinalDirections CurrentDirection => currentDirection;

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
            myWeapon.CancelAttack();
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
        SnapDirection(mouseDirection);

        myWeapon.SetAttackDirection(attackDirection);        
        myWeapon.SetBlockDirection(attackDirection);
    }

    private void SnapDirection(Vector2 direction)
    {
        if (direction.x > 0.5f)
            currentDirection = CardinalDirections.Right;
        else if (direction.x < -0.5f)
            currentDirection = CardinalDirections.Left;
        else if (direction.y > 0.5f)
            currentDirection = CardinalDirections.Up;
        else if (direction.y < -0.5f)
            currentDirection = CardinalDirections.Down;
        
        directionVectors.TryGetValue(currentDirection, out attackDirection);
        attackDirection.Normalize();
    }

    public void SetAttackDirection(Vector2 direction)
    {
        attackDirection = direction;
    }

    private void OnHit(HitEvent hitEvent)
    {
        if (hitEvent.type == HitEventType.Missed)
        {
            print($"{hitEvent.aggressor.name}'s {currentDirection} attack with their {hitEvent.weaponUsed.name} missed"); // at {hitEvent.hitPoint}");
            return;
        }

        if (hitEvent.type == HitEventType.Bumped)
        {
            print($"{hitEvent.aggressor.name}'s {currentDirection} attack with their {hitEvent.weaponUsed.name} bumped off {hitEvent.opponent.name}");
            return;
        }

        if (hitEvent.type == HitEventType.Blocked)
        {
            print($"{hitEvent.aggressor.name}'s {currentDirection} attack was blocked by {hitEvent.opponent.name} at {hitEvent.hitPoint} with their {hitEvent.weaponUsed.name}");
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
