using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class RTSCameraMovement : ZeroGMovement
{
    [SerializeField] private float minZoom = 5;
    [SerializeField] private float maxZoom = 50;
    [SerializeField] private float currentZoom;
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float refocusSpeedModifier = 3f;
    [SerializeField] private Transform refocusParent;
    [SerializeField] private Transform refocusTarget;
    [SerializeField] private Transform rtsCamera;

    [SerializeField] private UnityEvent onReachedTarget = new UnityEvent();

    protected override void Awake()
    {
        base.Awake();
        InitRefocusTarget();
        transform.SetParent(transform.root.parent);
    }

    private void InitRefocusTarget()
    {
        if (refocusParent == null)
        {
            refocusParent = transform.parent;
        }
        refocusTarget.parent = refocusParent;
        refocusTarget.localPosition = Vector3.zero;
        
        Target = refocusTarget;
    }

    public void MoveRTSCamera(Vector3 input)
    {
        var velocity = input * maxSpeed;
        MyRigidBody.velocity = rtsCamera.transform.TransformDirection(velocity);
    }

    public void RotateRTSCamera(Vector2 rotationDelta)
    {
        var rotationVelocity = rotationDelta.x;
        rtsCamera.transform.RotateAround(transform.position, Vector3.up, rotationVelocity);
    }

    public void ZoomRTSCamera(Vector3 scrollDelta)
    {
        scrollDelta.z = scrollDelta.y;
        scrollDelta.y = 0;
        var velocity = scrollDelta; 
        
        var desiredPosition = rtsCamera.transform.position + rtsCamera.transform.TransformVector(velocity) * zoomSpeed;
        desiredPosition = Vector3.Lerp(rtsCamera.transform.position, desiredPosition, zoomSpeed * Time.deltaTime);
        
        currentZoom = Vector3.Distance(rtsCamera.transform.position, transform.position);
        var desiredZoom = Vector3.Distance(desiredPosition, transform.position);

        if (desiredZoom < minZoom || desiredZoom > maxZoom)
        {
            return;
        }
        
        rtsCamera.transform.position = desiredPosition;
    }

    public void FocusOnTarget()
    {
        if (HasReachedTarget())
        {
            onReachedTarget?.Invoke();
            return;
        }
        
        MoveToTargetIgnoreAngle(refocusParent, maxSpeed * refocusSpeedModifier);
    }

    public void LockToTarget()
    {
        transform.position = refocusTarget.position;
    }
}
