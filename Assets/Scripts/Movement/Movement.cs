using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public abstract class Movement : MonoBehaviour
{
    [SerializeField] protected float maxSpeed = 20f;
    [SerializeField] protected float rotationSpeed = 5f;
    
    protected Rigidbody MyRigidBody;
    protected Vector3 Velocity => MyRigidBody.velocity;
    protected float Mass => MyRigidBody.mass;

    public float MaxSpeed => maxSpeed;
    
    protected virtual void Awake()
    {
        MyRigidBody = GetComponent<Rigidbody>();
    }
}
