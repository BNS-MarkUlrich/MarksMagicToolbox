using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class TPMovement : Movement
{
    [Header("Components")]
    [SerializeField] protected Camera myCamera;

    [Header("Speed")]
    [SerializeField] protected float currentSpeed;
    [SerializeField] protected float maxReverseSpeed;
    
    [Header("Rotation")]
    [SerializeField] protected Vector2 rotationVelocity;
    [SerializeField] protected float maxRotationVelocity;

    [Header("Forward Thrust")]
    [SerializeField] protected float forwardThrust;
    [SerializeField] protected float maxForwardThrust;
    
    [Header("Backward Thrust")]
    [SerializeField] protected float reverseThrust;
    [SerializeField] protected float maxReverseThrust;
    
    [Header("Turning Thrust")]
    [SerializeField] protected Vector2 turningThrust;
    [SerializeField] protected float maxTurningThrust;
    
    [Header("Pilot Options")]
    [SerializeField] protected bool isBrakingAutomatically;

    protected bool IsGrounded => Physics.SphereCast(transform.position, transform.localScale.x/2, Vector3.down, out _, 0.6f);

    protected override void Awake()
    {
        base.Awake();
        maxRotationVelocity = maxSpeed / Mass;
    }

    public void ApplyForwardThrust(float thrustMultiplier)
    {
        if (!IsGrounded)
        {
            return;
        }

        if (thrustMultiplier == 0)
        {
            forwardThrust = 0;
            reverseThrust = 0;
            
            if (isBrakingAutomatically)
            {
                currentSpeed = Mathf.Lerp(currentSpeed, 0,  Time.deltaTime * 2);
                currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
            }
            
            return;
        }
        else if (thrustMultiplier > 0)
        {
            forwardThrust += thrustMultiplier * maxForwardThrust * Time.deltaTime;
            forwardThrust = Mathf.Clamp(forwardThrust, 0, maxForwardThrust);
            currentSpeed += forwardThrust / Mass;
        }
        else if (thrustMultiplier < 0)
        {
            reverseThrust -= thrustMultiplier * maxReverseThrust * Time.deltaTime;
            reverseThrust = Mathf.Clamp(reverseThrust, maxReverseThrust, 0);
            currentSpeed += reverseThrust / Mass;
        }

        currentSpeed = Mathf.Clamp(currentSpeed, maxReverseSpeed, maxSpeed);

        var relativeDirection = myCamera.transform.forward.normalized;
        relativeDirection.y = 0;
        MyRigidBody.velocity = relativeDirection * (currentSpeed * Time.deltaTime);
        //MyRigidBody.velocity = Vector3.ClampMagnitude(MyRigidBody.velocity, maxSpeed);
        MyRigidBody.MoveRotation(Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(MyRigidBody.velocity), Time.deltaTime * rotationSpeed));
    }

    public void ApplyLateralThrust(Vector2 thrustVelocity, bool ignorePitch)
    {
        thrustVelocity.y = 0;
        ApplyTurningThrust(thrustVelocity, ignorePitch);
    }
    
    public void ApplyTurningThrust(Vector2 thrustVelocity, bool ignorePitch)
    {
        if (thrustVelocity.magnitude <= 0.05f)
        {
            thrustVelocity = Vector2.zero;
            turningThrust = thrustVelocity;
            rotationVelocity = EaseOutVelocity(rotationVelocity, maxTurningThrust);
        }
        else
        {
            turningThrust = GetThrust(thrustVelocity, maxTurningThrust);
            rotationVelocity = GetAngularVelocity(turningThrust, maxTurningThrust, maxRotationVelocity);
        }

        // var relativeDirection = myCamera.transform.forward.normalized;
        // relativeDirection.y = 0;
        // MyRigidBody.velocity = relativeDirection * (currentSpeed * Time.deltaTime);
        // MyRigidBody.MoveRotation(Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(MyRigidBody.velocity), Time.deltaTime * rotationSpeed));

        transform.Rotate(Vector3.up * rotationVelocity.x / Mass);
        transform.Rotate(Vector3.right * -rotationVelocity.y / Mass);
    }

    private static float GetThrust(float thrustMultiplier, float maxThrust)
    {
        var thrust = thrustMultiplier * maxThrust;
        return Mathf.Clamp(thrust, -maxThrust, maxThrust);
    }

    private static Vector2 GetThrust(Vector2 thrustVelocity, float maxThrust)
    {
        var thrust = thrustVelocity * maxThrust;
        
        thrust.x = Mathf.Clamp(thrust.x, -maxThrust, maxThrust);
        thrust.y = Mathf.Clamp(thrust.y, -maxThrust, maxThrust);

        return thrust;
    }
    
    private static float GetThrustAdditive(float thrust, float thrustMultiplier, float maxThrust)
    {
        thrust += thrustMultiplier * maxThrust;
        return Mathf.Clamp(thrust, -maxThrust, maxThrust);
    }
    
    private static float GetThrustFromAngle(float fromAngle, float toAngle, float maxThrust)
    {
        var angle = Mathf.DeltaAngle(fromAngle, toAngle);

        if (angle is < 0.5f and > 0 or < 0 and > -0.5f)
        {
            return 0;
        }

        return fromAngle switch
        {
            > 180 => -GetThrust(angle / 10, maxThrust),
            <= 180 => -GetThrust(angle / 10, maxThrust),
            _ => 0
        };
    }

    private static float GetAngularVelocity(float thrust, float maxThrust, float maxVelocity)
    {
        float velocity = (thrust / maxThrust) * maxVelocity;
        velocity = Mathf.Clamp(velocity, -maxVelocity, maxVelocity);

        return velocity;
    }
    
    private static Vector2 GetAngularVelocity(Vector2 thrust, float maxThrust, float maxVelocity)
    {
        Vector2 velocity = (thrust / maxThrust) * maxVelocity;

        velocity.x = Mathf.Clamp(velocity.x, -maxVelocity, maxVelocity);
        velocity.y = Mathf.Clamp(velocity.y, -maxVelocity, maxVelocity);

        return velocity;
    }
    
    private static Vector2 GetClampedAngularVelocity(Vector2 velocity, float maxVelocity)
    {
        velocity.x = Mathf.Clamp(velocity.x, -maxVelocity, maxVelocity);
        velocity.y = Mathf.Clamp(velocity.y, -maxVelocity, maxVelocity);

        return velocity;
    }
    
    public void MoveTowards(Transform target)
    {
        var position = transform.position;
        var distance = Vector3.Distance(target.position, position);
        
        RotateTowards(target);
        ApplyForwardThrust(distance);
    }

    public void RotateTowards(Transform rotationTarget)
    {
        transform.LookAt(rotationTarget);
    }

    public void AlignRotation(Quaternion targetRotation)
    {
        var rotation = transform.rotation;
        var angle = Quaternion.Angle(targetRotation, rotation);

        turningThrust = GetThrust(Vector2.one * angle, maxTurningThrust);
        rotationVelocity = GetAngularVelocity(turningThrust, maxTurningThrust, maxRotationVelocity);

        transform.rotation = Quaternion.Slerp(rotation, targetRotation, rotationVelocity.magnitude * Time.deltaTime / Mass);
    }

    private static Vector2 EaseOutVelocity(Vector2 velocity, float maxThrust)
    {
        if (velocity.magnitude <= 0.25f)
        {
            return Vector2.zero;
        }
        
        return Vector2.Lerp(velocity, Vector2.zero, maxThrust * (velocity.magnitude * 2) * Time.deltaTime);
    }
    
    public void AlignRotation(Vector3 targetPosition)
    {
        var position = transform.position;
        var targetRotation = Quaternion.LookRotation(targetPosition - position);
        AlignRotation(targetRotation);
    }

    private void FixedUpdate() 
    {
        // Apply gravity
        MyRigidBody.velocity += Vector3.down * (MyRigidBody.mass / 10) * (9.81f / 2) * Time.deltaTime;
    }

    // Jump method
    public void Jump(float jumpHeight)
    {
        if (!IsGrounded) return;

        MyRigidBody.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
        // MyRigidBody.AddForce(transform.forward * Mathf.Sqrt(jumpHeight/2 * -2f * Physics.gravity.y), ForceMode.Acceleration);
    }
}
