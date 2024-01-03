using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSystem : MonoBehaviour
{
    [SerializeField] private Sword sword;
    [SerializeField] private float blockingAngle = 45f;

    private Vector2 attackDirection;
    private Vector2 mouseDirection;
    private HashSet<HealthData> enemiesHit = new();

    private void Update() 
    {
        if (Input.GetKey(KeyCode.Mouse0))
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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out HealthData healthData) && !enemiesHit.Contains(healthData))
            enemiesHit.Add(healthData);
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
        sword.transform.rotation = Quaternion.Lerp(sword.transform.rotation, Quaternion.Euler(0f, 0f, angle), 0.5f);
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
