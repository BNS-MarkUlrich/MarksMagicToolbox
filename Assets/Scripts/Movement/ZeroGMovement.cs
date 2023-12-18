using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroGMovement : Movement
{
    protected Transform Target;
    private float _distanceToTarget;

    protected bool HasReachedTarget(float distanceThreshold = 1f)
    {
        if (Target == null) return true;

        _distanceToTarget = Vector3.Distance(transform.position, Target.transform.position);

        return _distanceToTarget < distanceThreshold;
    }
    
    protected void MoveToTarget(Transform target, float speed = 0f)
    {
        if (speed == 0f) speed = maxSpeed;

        var velocityDirection = target.position - transform.position;

        var angle = Vector3.Angle(velocityDirection, transform.forward) / 10;
        
        var currentSpeed = speed / angle;
        
        if (currentSpeed >= speed) currentSpeed = speed;

        MyRigidBody.velocity = transform.forward.normalized * (currentSpeed) / Mass;
    }

    protected void MoveToTargetIgnoreAngle(Transform target, float speed = 0f)
    {
        if (speed == 0f) speed = maxSpeed;

        var velocityDirection = target.position - transform.position;

        MyRigidBody.velocity = velocityDirection.normalized * (speed) / Mass;
    }
    
    protected void RotateToTarget(Transform rotationTarget, float rotatespeed = 0f)
    {
        if (rotatespeed == 0f) rotatespeed = rotationSpeed;
        
        var targetDirection = rotationTarget.position - transform.position;
        var angle = Vector3.Angle(targetDirection, transform.forward) / 10;
        var turnDirection = Vector3.Dot(targetDirection, transform.right);
        var pitch = (angle / 100) / Mass;
        if (turnDirection > 0f) 
        {
            pitch = -pitch;
        } 
        transform.Rotate(transform.InverseTransformDirection(transform.forward), pitch);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDirection), Time.deltaTime * (rotatespeed / angle / Mass));
    }
}
