using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MeleeSystem : MonoBehaviour
{
    [SerializeField] private BaseWeapon myWeapon;
    [SerializeField] private SerializableDictionary<CardinalDirections, Vector2> directionVectors;
    [SerializeField] private SerializableDictionary<CardinalDirections, Vector2> blockingAngles;
    [SerializeField] private LayerMask blockingLayers;
    [SerializeField] private float blockingRadius = 1f;
    [SerializeField] private float blockingAngleLeft = -45f;
    [SerializeField] private float blockingAngleRight = 45f;
    [SerializeField] private bool testAlwaysBlock;

    private Vector2 stanceDirection;
    private Vector2 mouseDirection;
    private CardinalDirections currentDirection = CardinalDirections.Right;


    public Agent OwningAgent { get; set; }
    public Vector2 AttackDirection => stanceDirection;
    public CardinalDirections CurrentDirection => currentDirection;

    private void Start() 
    {
        myWeapon = GetComponentInChildren<BaseWeapon>();
        myWeapon.OwningAgent = OwningAgent; // Change to pickup event later
        myWeapon.OnHit += OnHit;
    }

    private void Update() 
    {
        if (testAlwaysBlock)
        {
            currentDirection = CardinalDirections.Right;
            SnapDirection(directionVectors[currentDirection]);
            myWeapon.SetBlockDirection(blockingAngles[currentDirection]);
            myWeapon.Block();
            myWeapon.CancelAttack();
            DetectObjectsBetweenRays();
            return;
        }

        if (Input.GetKey(KeyCode.Mouse0) && !myWeapon.IsAttacking)
        {
            if (!Input.GetKey(KeyCode.Mouse1))
                ChooseDirection();
            
            myWeapon.Block();
            myWeapon.CancelAttack();
            DetectObjectsBetweenRays();
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

        myWeapon.SetAttackDirection(stanceDirection);        
        myWeapon.SetBlockDirection(blockingAngles[currentDirection]);
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
        
        directionVectors.TryGetValue(currentDirection, out stanceDirection);
        stanceDirection.Normalize();
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

    private void DetectObjectsBetweenRays()
    {
        Vector3 forwardDirection = transform.forward;

        Vector3 leftDirection = Quaternion.Euler(0, blockingAngleLeft / 2f, 0) * forwardDirection;
        Vector3 rightDirection = Quaternion.Euler(0, blockingAngleRight / 2f, 0) * forwardDirection;

        // Calculate the endpoints of the left and right rays
        Vector3 leftEndpoint = transform.position + leftDirection * blockingRadius;
        Vector3 rightEndpoint = transform.position + rightDirection * blockingRadius;

        // Calculate the direction vector from leftEndpoint to rightEndpoint
        Vector3 thirdDirection = (rightEndpoint - leftEndpoint).normalized;

        RaycastHit hit;

        // Perform raycasts to detect objects
        if (Physics.Raycast(transform.position, leftDirection, out hit, blockingRadius, blockingLayers) ||
            Physics.Raycast(transform.position, rightDirection, out hit, blockingRadius, blockingLayers) ||
            Physics.Raycast(leftEndpoint, thirdDirection, out hit, Vector3.Distance(leftEndpoint, rightEndpoint), blockingLayers))
        {
            print($"Hit {hit.collider.name}");
        }
        else
        {
            // No objects detected between the rays
            Debug.DrawLine(transform.position, transform.position + leftDirection * blockingRadius, Color.green);
            Debug.DrawLine(transform.position, transform.position + rightDirection * blockingRadius, Color.green);
            Debug.DrawLine(transform.position, transform.position + thirdDirection, Color.green);
        }
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, stanceDirection);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, mouseDirection);

        // Gizmos.color = Color.green;
        // Gizmos.DrawWireSphere(transform.position, blockingRadius);

        Vector3 forwardDirection = transform.forward;

            Vector3 leftDirection =  Quaternion.Euler(0, blockingAngleLeft / 2f, 0) * forwardDirection;
            Vector3 rightDirection = Quaternion.Euler(0, blockingAngleRight / 2f, 0) * forwardDirection;

            // Calculate the endpoints of the left and right rays
            Vector3 leftEndpoint = transform.position + leftDirection * blockingRadius;
            Vector3 rightEndpoint = transform.position + rightDirection * blockingRadius;

            // Calculate the direction vector from leftEndpoint to rightEndpoint
            Vector3 thirdDirection = (rightEndpoint - leftEndpoint).normalized;

            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, leftDirection * blockingRadius);
            Gizmos.DrawRay(transform.position, rightDirection * blockingRadius);
            Gizmos.DrawRay(leftEndpoint, thirdDirection * Vector3.Distance(leftEndpoint, rightEndpoint));
    }
}
