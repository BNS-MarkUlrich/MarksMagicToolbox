using System.Collections.Generic;
using UnityEngine;

public class MeleeSystem : MonoBehaviour
{
    [SerializeField] private BaseWeapon myWeapon;
    [SerializeField] private SerializableDictionary<CardinalDirections, Vector2> directionVectors;
    [SerializeField] private SerializableDictionary<CardinalDirections, Vector2> blockingAngles; // TODO: Change it so X is a direction and Y is the range of min/max angle
    [SerializeField] private LayerMask detectionLayerMask;
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private bool testAlwaysBlock;

    private Vector2 attackdirection;
    private Vector2 mouseDirection;
    [SerializeField] private CardinalDirections currentDirection = CardinalDirections.Right;
    private Collider[] detectedAgents;
    private Dictionary<Collider, Agent> cachedAgents = new();

    public Agent OwningAgent { get; set; }
    public Vector2 AttackDirection => attackdirection;
    public CardinalDirections CurrentDirection => currentDirection;
    public bool StanceIsLeftOrRight => currentDirection == CardinalDirections.Left || currentDirection == CardinalDirections.Right;
    public bool StanceIsUpOrDown => currentDirection == CardinalDirections.Up || currentDirection == CardinalDirections.Down;

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
            SnapDirection(directionVectors[currentDirection]);
            myWeapon.SetBlockDirection(blockingAngles[currentDirection]);
            myWeapon.Block();
            myWeapon.CancelAttack();
            DetectAgents();

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

        myWeapon.SetAttackDirection(attackdirection);        
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
        
        directionVectors.TryGetValue(currentDirection, out attackdirection);
        attackdirection = transform.InverseTransformDirection(attackdirection);
        attackdirection.Normalize();
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
            print($"{hitEvent.aggressor.name}'s {hitEvent.stanceDirection} attack was blocked by {hitEvent.opponent.name}'s {hitEvent.opponent.MeleeSystem.CurrentDirection} block");
            return;
        }

        if (hitEvent.opponent.MeleeSystem.HasBlockedOpponent(OwningAgent))
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

                IsAgentInBlockingAngle(agent);
            }
        }
    }

    private bool HasBlockedOpponent(Agent opponent)
    {
        if (opponent == null || opponent == OwningAgent || !IsAgentInBlockingAngle(opponent))
            return false;

        bool areMatchingHorizontalStances = StanceIsLeftOrRight && opponent.MeleeSystem.StanceIsLeftOrRight;
        bool leftRightBlock = areMatchingHorizontalStances && opponent.MeleeSystem.CurrentDirection != CurrentDirection;
        bool upDownBlock = StanceIsUpOrDown && opponent.MeleeSystem.CurrentDirection == CurrentDirection;

        return leftRightBlock || upDownBlock;
    }

    private bool IsAgentInBlockingAngle(Agent otherAgent)
    {
        if (otherAgent == OwningAgent)
            return false;

        float angleToOtherAgent = GetAngleToTarget(otherAgent);
        
        if (angleToOtherAgent > blockingAngles[currentDirection].x && angleToOtherAgent < blockingAngles[currentDirection].y)
        {
            Debug.DrawLine(transform.position, otherAgent.transform.position, Color.red);
            return true;
        }

        return false;
    }

    private float GetAngleToTarget(Agent target)
    {
        Vector3 directionToOtherAgent = target.transform.position - transform.position;
        float angleToOtherAgent = Vector3.Angle(transform.forward, directionToOtherAgent);

        // make angle negative if other agent is to the left of the player
        if (Vector3.Cross(transform.forward, directionToOtherAgent).y < 0)
            angleToOtherAgent *= -1;

        return angleToOtherAgent;
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, attackdirection);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, mouseDirection);

        // Gizmos.color = Color.green;
        // Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, blockingAngles[currentDirection].x, 0) * transform.forward * detectionRadius);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, blockingAngles[currentDirection].y, 0) * transform.forward * detectionRadius);  
    }
}
