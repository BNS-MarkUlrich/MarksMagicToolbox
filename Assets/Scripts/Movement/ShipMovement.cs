using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipMovement : ZeroGMovement
{
    [SerializeField] private Transform target;
    
    private bool isFollowingTarget;

    private void Start()
    {
        InitTarget();
    }

    private void InitTarget()
    {
        Target = target;
    }

    public void EnableTarget()
    {
        isFollowingTarget = true;
        target.transform.parent = transform.root.parent;
        target.gameObject.SetActive(true);
        target.transform.position = transform.position;
    }

    public void DisableTarget()
    {
        isFollowingTarget = false;
        target.transform.parent = transform;
        target.gameObject.SetActive(false);
        target.transform.position = transform.position;
    }

    public void SetTargetDestination(Vector3 destination)
    {
        target.transform.position = destination;
    }

    private void Update()
    {
        if (!isFollowingTarget) return;

        if (HasReachedTarget(2f))
        {
            MyRigidBody.velocity *= 0.5f * Time.deltaTime;
            return;
        }
        
        MoveToTarget(target);
        RotateToTarget(target);
    }
}
