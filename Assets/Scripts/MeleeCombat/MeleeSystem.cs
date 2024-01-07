using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeleeSystem : MonoBehaviour
{
    [SerializeField] private BaseWeapon myWeapon;
    [SerializeField] private SerializableDictionary<CardinalDirections, Vector2> directionVectors;
    [SerializeField] private SerializableDictionary<CardinalDirections, Vector2> blockingAngles;
    [SerializeField] private LayerMask detectionLayerMask;
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private bool testAlwaysBlock;

    private Vector2 stanceDirection;
    private Vector2 mouseDirection;
    [SerializeField] private CardinalDirections currentDirection = CardinalDirections.Right;
    private Collider[] detectedAgents;
    private Dictionary<Collider, Agent> cachedAgents = new();

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
        // if (myWeapon.IsBlocking)
        //     DetectAgents();

        if (testAlwaysBlock)
        {
            SnapDirection(directionVectors[currentDirection]);
            myWeapon.SetBlockDirection(blockingAngles[currentDirection]);
            myWeapon.Block();
            myWeapon.CancelAttack();

            return;
        }

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

        if (CheckBlockingAngle(hitEvent.opponent))
        {
            myWeapon.TriggerHitEvent(HitEventType.Blocked, hitEvent.opponent, hitEvent.hitPoint);
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

    private void DetectAgents()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius,  detectionLayerMask);
        detectedAgents = colliders;
            
        foreach (Collider detectedAgent in detectedAgents)
        {
            if (detectedAgent == null)
                continue;

            if (detectedAgent.gameObject == gameObject)
                continue;
            
            if(detectedAgent.TryGetCachedComponent(ref cachedAgents, out Agent agent))
            {
                if (agent == OwningAgent)
                    continue;

                CheckBlockingAngle(agent);
            }
        }
    }

    private bool CheckBlockingAngle(Agent otherAgent)
    {
        print($"{OwningAgent.name} is checking if {otherAgent.name} is blocking their attack");
        if (otherAgent == OwningAgent)
            return false;

        Vector3 directionToOtherAgent = otherAgent.transform.position - transform.position;
        float angleToOtherAgent = Vector3.Angle(transform.forward, directionToOtherAgent);

        // make angle negative if other agent is to the left of the player
        if (Vector3.Cross(transform.forward, directionToOtherAgent).y < 0)
            angleToOtherAgent *= -1;

        if (angleToOtherAgent > blockingAngles[currentDirection].x && angleToOtherAgent < blockingAngles[currentDirection].y)
        {
            // print($"{OwningAgent.name}'s is blocking {otherAgent.name}'s attack from {angleToOtherAgent} degrees ({blockingAngles[currentDirection].x} to {blockingAngles[currentDirection].y}))");
            Debug.DrawLine(transform.position, otherAgent.transform.position, Color.red);
            return true;
        }


        return false;
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, stanceDirection);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, mouseDirection);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        foreach (Vector2 blockingAngle in blockingAngles.Values)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, Quaternion.Euler(0, blockingAngle.x, 0) * transform.forward * detectionRadius);
            Gizmos.DrawRay(transform.position, Quaternion.Euler(0, blockingAngle.y, 0) * transform.forward * detectionRadius);
        }
    }
}
