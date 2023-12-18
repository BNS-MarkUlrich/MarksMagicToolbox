using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ManualFlightMovement : Movement
{
    [Header("Speed")]
    [SerializeField] protected float currentSpeed;
    [SerializeField] protected float maxReverseSpeed;
    
    [Header("Rotation")]
    [SerializeField] protected Vector2 rotationVelocity;
    [SerializeField] protected float maxRotationVelocity;
    
    [SerializeField] protected float pitchVelocity;
    [SerializeField] protected float maxPitchVelocity;

    [Header("Forward Thrust")]
    [SerializeField] protected float forwardThrust;
    [SerializeField] protected float maxForwardThrust;
    
    [Header("Backward Thrust")]
    [SerializeField] protected float reverseThrust;
    [SerializeField] protected float maxReverseThrust;
    
    [Header("Turning Thrust")]
    [SerializeField] protected Vector2 turningThrust;
    [SerializeField] protected float maxTurningThrust;
    
    [Header("Pitch Thrust")]
    [SerializeField] protected float pitchThrust;
    [SerializeField] protected float maxPitchThrust;

    [Header("Pilot Options")]
    [SerializeField] protected bool isBrakingAutomatically;
    [SerializeField] protected bool isPitchingAutomatically;

    protected override void Awake()
    {
        base.Awake();
        //maxReverseSpeed = -maxSpeed / Mass;
        maxRotationVelocity = maxSpeed / Mass;
        maxPitchVelocity = maxRotationVelocity / 4;
    }

    private void Update()
    {
        if (isPitchingAutomatically) ResetPitch();
    }

    public void ApplyForwardThrust(float thrustMultiplier)
    {
        if (thrustMultiplier == 0)
        {
            forwardThrust = 0;
            reverseThrust = 0;
            
            if (isBrakingAutomatically)
            {
                currentSpeed = Mathf.Lerp(currentSpeed, 0,  Time.deltaTime * 2);

                currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
            }
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

        MyRigidBody.velocity = transform.forward.normalized * (currentSpeed * Time.deltaTime);
    }

    public void ApplyLateralThrust(Vector2 thrustVelocity, bool ignorePitch)
    {
        thrustVelocity.y = 0;
        ApplyTurningThrust(thrustVelocity, ignorePitch);
    }
    
    public void ApplyTurningThrust(Vector2 thrustVelocity, bool ignorePitch)
    {
        // BEGIN Mark: Testing some stuff
        /*thrustVelocity.y = -thrustVelocity.x / 2;
        ApplyPitchThrust(thrustVelocity.x);*/
        // END Mark: Testing some stuff

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
            //rotationVelocity = GetClampedAngularVelocity(rotationVelocity, maxRotationVelocity);
        }

        if (!ignorePitch)
        {
            pitchThrust = GetThrustAdditive(pitchThrust,thrustVelocity.x, maxPitchThrust);
            pitchVelocity = GetAngularVelocity(pitchThrust, maxPitchThrust, maxPitchVelocity);
            transform.Rotate(Vector3.forward * -pitchVelocity / Mass);
        }
        else
        {
            ResetPitch();
        }
        
        //MyRigidBody.MoveRotation(Quaternion.Euler(Vector3.right * -rotationVelocity / Mass)); // Todo Mark: Try this out

        transform.Rotate(Vector3.up * rotationVelocity.x / Mass);
        transform.Rotate(Vector3.right * -rotationVelocity.y / Mass);
        
        //MyRigidBody.MoveRotation(transform.rotation);
    }
    
    public void ApplyPitchThrust(float thrustMultiplier)
    {
        if (thrustMultiplier == 0 && rotationVelocity.x == 0)
        {
            ResetPitch();
        }
        else
        {
            pitchThrust = GetThrust(thrustMultiplier, maxPitchThrust);
        }

        pitchVelocity = GetAngularVelocity(pitchThrust, maxPitchThrust, maxPitchVelocity);
        
        transform.Rotate(Vector3.forward * -pitchVelocity / Mass);
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
        var targetDirection = rotationTarget.position - transform.position;
        /*var angle = Vector3.Angle(targetDirection, transform.forward);
        var turnDirection = Vector3.Dot(targetDirection, transform.right);*/

        transform.LookAt(rotationTarget);
        
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDirection), rotationVelocity.magnitude * Time.deltaTime / Mass);
    }

    public void AlignRotation(Quaternion targetRotation)
    {
        var rotation = transform.rotation;
        var angle = Quaternion.Angle(targetRotation, rotation);

        turningThrust = GetThrust(Vector2.one * angle, maxTurningThrust);
        rotationVelocity = GetAngularVelocity(turningThrust, maxTurningThrust, maxRotationVelocity);

        transform.rotation = Quaternion.Slerp(rotation, targetRotation, rotationVelocity.magnitude * Time.deltaTime / Mass);
    }

    private void ResetPitch()
    {
        var eulerAngles = transform.eulerAngles;
        
        pitchThrust = GetThrustFromAngle(eulerAngles.z,0, maxPitchThrust);
        pitchVelocity = GetAngularVelocity(pitchThrust,maxPitchThrust, maxPitchVelocity);

        transform.Rotate(Vector3.forward * -pitchVelocity / Mass);
    }
    
    // Experimental
    
    private static Vector2 EaseOutVelocity(Vector2 velocity, float maxThrust)
    {
        if (velocity.magnitude <= 0.25f)
        {
            return Vector2.zero;
        }
        
        return Vector2.Lerp(velocity, Vector2.zero, maxThrust * (velocity.magnitude * 2) * Time.deltaTime);
    }
    
    private void RotateToAngle(Vector2 to) // So far hasn't worked, purely experimental
    {
        var eulerAngles = transform.eulerAngles;
        
        turningThrust.x = GetThrustFromAngle(eulerAngles.x, to.x, maxTurningThrust);
        turningThrust.y = GetThrustFromAngle(eulerAngles.y, to.y, maxTurningThrust);
        
        rotationVelocity.x = GetAngularVelocity(turningThrust.x,maxTurningThrust, maxRotationVelocity);
        rotationVelocity.y = GetAngularVelocity(turningThrust.y,maxTurningThrust, maxRotationVelocity);

        transform.Rotate(Vector3.up * rotationVelocity.x / Mass);
        transform.Rotate(Vector3.right * -rotationVelocity.y / Mass);
    }
    
    private float FixedEulerAngle(float angle)
    {
        if (angle < 180)
        {
            return angle;
        }
        
        var fixedAngle = 360 - angle;
        fixedAngle = -fixedAngle;

        return fixedAngle;
    }
}
