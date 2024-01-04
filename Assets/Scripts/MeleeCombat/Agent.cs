using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    private int agentId;
    private Rigidbody myRigidbody;
    private HealthData healthData;
    private MeleeSystem meleeSystem;

    public int AgentId => agentId;
    public Rigidbody MyRigidbody => myRigidbody;
    public HealthData HealthData => healthData;
    public MeleeSystem MeleeSystem => meleeSystem;

    private void OnEnable()
    {
        agentId = gameObject.GetInstanceID();
        myRigidbody = GetComponent<Rigidbody>();
        healthData = GetComponent<HealthData>();
        meleeSystem = GetComponent<MeleeSystem>();
        meleeSystem.OwningAgent = this;
    }
}
