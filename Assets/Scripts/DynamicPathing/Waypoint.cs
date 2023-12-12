using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class Waypoint : MonoBehaviour
{
    [SerializeField] 
    protected GameObject p_target;
    
    [Header("Path Creation")]
    [SerializeField] 
    protected bool p_createConnectedWaypoint;
    
    [Range(0, 5)]
    [SerializeField]
    protected float p_waypointRadius = 1f;
    
    [Header("Waypoint Settings")]
    [SerializeField] 
    protected bool p_connectSelection;
    
    [SerializeField] 
    protected bool p_cutSelection;
    
    [SerializeField] 
    protected bool p_blockConnection;
    
    [SerializeField]
    protected List<Waypoint> p_directConnections = new();

    [Range(0, 1)] 
    [SerializeField]
    protected float p_directConnectionsChance = 1f;

    [SerializeField]
    protected List<Waypoint> p_indirectConnections = new();

    [Range(0, 1)] 
    [SerializeField]
    protected float p_indirectConnectionChance = 0.1f;
    
    [SerializeField]
    protected List<Waypoint> p_blockedConnections = new();

    [Range(0, 1)] 
    [SerializeField]
    protected float p_blockedConnectionsChance = 0.05f;

    [SerializeField]
    protected bool p_isDeadEnd;

    [Header("Debugging")]
    [SerializeField] 
    protected bool p_drawGizmos;

    [SerializeField]
    protected Color p_connectionColor = Color.red;

    protected List<DynamicPathFollower> p_dynamicFollowers = new();

    public GameObject Target => p_target;
    public bool IsDeadEnd => p_isDeadEnd;
    public List<Waypoint> DirectConnections => p_directConnections;
    public float DirectConnectionsChance => p_directConnectionsChance;
    public List<Waypoint> IndirectConnections => p_indirectConnections;
    public float IndirectConnectionChance => p_indirectConnectionChance;
    public List<Waypoint> BlockedConnections => p_blockedConnections;
    public float BlockedConnectionsChance => p_blockedConnectionsChance;
    public List<Waypoint> ConnectedWaypoints => p_directConnections.Concat(p_indirectConnections).ToList();

    public Action<DynamicPathFollower, Waypoint> OnReachedWaypoint;

    /// <summary>
    /// Subscribes a DynamicPathFollower to this Waypoint.
    /// </summary>
    /// <param name="newDynamicFollower">The DynamicPathFollower to subscribe.</param>
    public void Subscribe(DynamicPathFollower newDynamicFollower)
    {
        if (!p_dynamicFollowers.Contains(newDynamicFollower))
            p_dynamicFollowers.Add(newDynamicFollower);
    }

    /// <summary>
    /// Unsubscribes a DynamicPathFollower from this Waypoint.
    /// </summary>
    /// <param name="newDynamicFollower">The DynamicPathFollower to unsubscribe.</param>
    public void UnSubscribe(DynamicPathFollower newDynamicFollower)
    {
        if (p_dynamicFollowers.Contains(newDynamicFollower))
            p_dynamicFollowers.Remove(newDynamicFollower);
    }

    protected void AddConnection(Waypoint newConnection)
    {
        bool isAlreadyConnected = p_directConnections.Contains(newConnection) || p_indirectConnections.Contains(newConnection);

        if (newConnection == this || isAlreadyConnected)
            return;

        if (IsTargetWaypointCloser(newConnection))
            p_directConnections.Add(newConnection);
        else
            p_indirectConnections.Add(newConnection);
    }

    protected void AddConnections(List<Waypoint> newConnections) => newConnections.ForEach(connection => AddConnection(connection));

    protected void RemoveConnection(Waypoint targetConnection)
    {
        if (p_blockedConnections.Contains(targetConnection))
            p_blockedConnections.Remove(targetConnection);

        if (p_indirectConnections.Contains(targetConnection))
        {
            p_indirectConnections.Remove(targetConnection);
            return;
        }
                
        if (p_directConnections.Contains(targetConnection))
            p_directConnections.Remove(targetConnection);
    }
    
    protected void RemoveConnections(List<Waypoint> targetWaypoints)
    {
        for (int i = targetWaypoints.Count - 1; i >= 0; i--)
            RemoveConnection(targetWaypoints[i]);
    }

    protected float GetDistanceToTarget(Waypoint waypoint) 
        => Vector3.Distance(waypoint.transform.position, p_target.transform.position);

    /// <summary>
    /// Determines whether the target waypoint is closer to the target than the current waypoint.
    /// </summary>
    /// <param name="targetWaypoint">The target waypoint to compare.</param>
    /// <returns>True if the target waypoint is closer; otherwise, false.</returns>
    public bool IsTargetWaypointCloser(Waypoint targetWaypoint) => GetDistanceToTarget(this) > GetDistanceToTarget(targetWaypoint);

    protected void ClearConnections()
    {
        p_directConnections.Clear();
        p_indirectConnections.Clear();
        p_blockedConnections.Clear();
    }

    /// <summary>
    /// Returns a random position within the waypoint.
    /// </summary>
    /// <returns>A random position within the waypoint.</returns>
    public Vector3 GetRandomPositionInWaypoint()
    {
        Vector3 randomPos = transform.position + Random.insideUnitSphere * p_waypointRadius;
        randomPos.y = transform.position.y;

        return randomPos;
    }

#if UNITY_EDITOR

    protected void CreateConnectedWaypoint()
    {
        p_createConnectedWaypoint = false;
        Waypoint newWaypoint = Instantiate(this, transform.parent);
        AddConnection(newWaypoint);
        newWaypoint.ClearConnections();
        newWaypoint.AddConnection(this);
        newWaypoint.name = name;
        Selection.activeObject = newWaypoint;
    }

    protected Waypoint[] GetSelection() => Selection.GetFiltered<Waypoint>(SelectionMode.Unfiltered);

    protected void ConnectSelection()
    {
        p_connectSelection = false;
        AddConnections(GetSelection().ToList());
    }

    protected void CutConnection()
    {
        p_cutSelection = false;
        RemoveConnections(GetSelection().ToList());
    }

#endif

    protected void ChainBlockConnections()
    {
        bool hasDirectConnectionsLeft = ListExtensions.GetListWithout(DirectConnections, BlockedConnections).Count > 0;
        if (hasDirectConnectionsLeft || IsDeadEnd)
            return;

        ConnectedWaypoints.ForEach(waypoint => waypoint.BlockConnection(this, true));
    }

    protected void ChainUnBlockConnections()
    {
        if (DirectConnections.Count > 1)
            return;

        ConnectedWaypoints.ForEach(waypoint => waypoint.BlockConnection(this, false));
    }
    
    /// <summary>
    /// Blocks or unblocks the connection to another waypoint.
    /// </summary>
    /// <param name="connection">The waypoint to block or unblock.</param>
    /// <param name="isBlocked">True to block the connection, false to unblock it.</param>
    public void BlockConnection(Waypoint connection, bool isBlocked)
    {
        if (isBlocked && !p_blockedConnections.Contains(connection))
        {
            p_blockedConnections.Add(connection);
            ChainBlockConnections();
        }
        else if (!isBlocked && p_blockedConnections.Contains(connection))
        {
            p_blockedConnections.Remove(connection);
            ChainUnBlockConnections();
        }
    }

#if UNITY_EDITOR

    protected void BlockConnection()
    {
        p_blockConnection = false;
        for (int i = 0; i < GetSelection().Length; i++)
        {
            if (GetSelection()[i] == this)
                continue;

            if (!p_blockedConnections.Contains(GetSelection()[i]))
            {
                BlockConnection(GetSelection()[i], true);
                continue;
            }

            BlockConnection(GetSelection()[i], false);
        }
    }

#endif

    protected void CutConnections()
    {
        for (int i = p_directConnections.Count - 1; i >= 0; i--)
            p_directConnections[i].ClearConnections();
        
        for (int i = p_indirectConnections.Count - 1; i >= 0; i--)
            p_indirectConnections[i].ClearConnections();
    }

    protected void UpdateConnections()
    {
        if (p_directConnections.Count <= 0 && p_indirectConnections.Count <= 0)
            return;

        if (p_directConnections.Count > 0)
            CheckConnections(p_directConnections, p_indirectConnections, true);

        if (p_indirectConnections.Count > 0)
            CheckConnections(p_indirectConnections, p_directConnections, false);
    }

    private void CheckConnections(List<Waypoint> connections, List<Waypoint> otherConnections, bool isDirectConnections)
    {
        for (int i = connections.Count - 1; i >= 0; i--)
        {
            if (connections[i] == null)
            {
                RemoveConnection(connections[i]);
                continue;
            }

            if (IsTargetWaypointCloser(connections[i]) != isDirectConnections)
            {
                if (!otherConnections.Contains(connections[i]))
                    otherConnections.Add(connections[i]);

                connections.Remove(connections[i]);
                continue;
            }

            if (!p_drawGizmos)
                continue;

            bool isBlocked = p_blockedConnections.Contains(connections[i]);
            Gizmos.color = isBlocked ? Color.black : p_connectionColor;
            Gizmos.DrawLine(transform.position, connections[i].transform.position);
        }
    }

#if UNITY_EDITOR

    protected virtual void OnDrawGizmos()
    {
        // Todo: optimise this (Maybe move to Editor script?)

        UpdateConnections();
        
        if (p_createConnectedWaypoint)
            CreateConnectedWaypoint();

        if (p_connectSelection)
            ConnectSelection();

        if (p_cutSelection)
            CutConnection();

        if (p_blockConnection)
            BlockConnection();

        if (!p_drawGizmos)
            return;
        
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, p_waypointRadius);
    }

#endif

    protected virtual void OnDestroy() => CutConnections();
}
