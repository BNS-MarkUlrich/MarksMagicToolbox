using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DynamicPathFollower : MonoBehaviour
{
    [SerializeField]
    private Waypoint _currentWaypoint;
    
    [SerializeField]
    private Waypoint _lastWaypoint;

    [SerializeField]
    private LayerMask _obstacleLayerMask;

    [SerializeField]
    private float _detectionRadius = 0.5f;

    [SerializeField]
    private LayerMask _followerLayerMask;

    private Vector3 _targetPosition;
    private bool _lastWaypointIsBlocked;
    private bool _isFindingNewPath;
    private bool _isCheckingBlockedWaypoints;
    private float _baseMovementSpeed;

    public float Speed { get; set; }
    public float RotationSpeed { get; set; }
    public bool CanMove { get; set; }
    public bool IsWaiting { get; set; }

    private bool HasDirectConnections => ListExtensions.GetListWithout(_currentWaypoint.DirectConnections, _lastWaypoint).Count > 0;
    private bool HasIndirectConnections => ListExtensions.GetListWithout(_currentWaypoint.IndirectConnections, _lastWaypoint).Count > 0;
    private bool HasBlockedConnections => ListExtensions.GetListWithout(_currentWaypoint.BlockedConnections, _lastWaypoint).Count > 0;
    private bool HasDirectConnectionsLeft => ListExtensions.GetListWithout(_currentWaypoint.DirectConnections,_currentWaypoint.BlockedConnections, _lastWaypoint).Count > 0;
    private bool HasInDirectConnectionsLeft => ListExtensions.GetListWithout(_currentWaypoint.IndirectConnections,_currentWaypoint.BlockedConnections, _lastWaypoint).Count > 0;

    private void Update()
    {
        if (CanMove && !IsWaiting)
            MoveToWaypoint();

        ObstacleDetection();

        if (_currentWaypoint.IsDeadEnd)
            CheckQueue();
    }

    /// <summary>
    /// Starts the walking process from the current position to the specified waypoint.
    /// </summary>
    /// <param name="waypoint">The waypoint to walk towards.</param>
    public void StartWalking(Waypoint waypoint)
    {
        if (CanMove)
            return;
        
        SetWaypoint(waypoint);
        _lastWaypoint = waypoint;

        CanMove = true;
    }

    /// <summary>
    /// Sets the waypoint for the dynamic path follower.
    /// </summary>
    /// <param name="waypoint">The waypoint to set.</param>
    public void SetWaypoint(Waypoint waypoint)
    {
        _currentWaypoint = waypoint;
        _targetPosition = _currentWaypoint.GetRandomPositionInWaypoint();
        transform.rotation = Quaternion.LookRotation(_targetPosition - transform.position);
    }

    private void ObstacleDetection()
    {
        Collider[] obstacleDetection = Physics.OverlapSphere(transform.position, _detectionRadius, _obstacleLayerMask);
        if (obstacleDetection.Length == 0)
            return;

        _lastWaypoint.BlockConnection(_currentWaypoint, true);
        _lastWaypointIsBlocked = true;
        _isFindingNewPath = true;

        transform.rotation = Quaternion.LookRotation(_lastWaypoint.transform.position - transform.position);

        SetNewWaypoint(_lastWaypoint);
    }

    private void CheckQueue()
    {
        bool isBlockedByOther = Physics.Linecast(transform.position, transform.position + (transform.forward / 4), _followerLayerMask);
        Debug.DrawLine(transform.position, transform.position + (transform.forward / 4), isBlockedByOther ? Color.red : Color.green);
        if (isBlockedByOther)
            IsWaiting = true;
        else
            IsWaiting = false;
    }

    private Waypoint GetNewWaypoint()
    {
        float chanceValue = Random.value;

        if (_lastWaypointIsBlocked && _isCheckingBlockedWaypoints && HasDirectConnectionsLeft)
            return HasCheckedBlockedPath();

        if (_isFindingNewPath)
            return FindViablePath();

        if (_isCheckingBlockedWaypoints && HasBlockedConnections)
            return GetRandomWaypoint(ListExtensions.GetListWithout
            (
                _currentWaypoint.BlockedConnections,
                _lastWaypoint
            ));

        ChooseNextWaypoint(chanceValue, out Waypoint nextWaypoint, out bool hasChosenWaypoint);
        if (hasChosenWaypoint)
            return nextWaypoint;
        
        nextWaypoint = CheckNextWaypointBlocked(nextWaypoint);
        
        if (nextWaypoint == null)
            nextWaypoint = NoWaypointFallback(chanceValue);

        return nextWaypoint;
    }

    private Waypoint FindViablePath()
    {
        bool hasFoundViablePath = ListExtensions.GetListWithout(_currentWaypoint.DirectConnections, _currentWaypoint.BlockedConnections, _lastWaypoint).Count > 0;

        if (hasFoundViablePath)
            return GetRandomWaypoint(ListExtensions.GetListWithout
            (
                _currentWaypoint.DirectConnections,
                _currentWaypoint.BlockedConnections
            ));
            
        return GetRandomWaypoint(_currentWaypoint.IndirectConnections);
    }

    private Waypoint HasCheckedBlockedPath()
    {
        _lastWaypointIsBlocked = false;
        _isCheckingBlockedWaypoints = false;
        _isFindingNewPath = false;

        bool hasReachedOtherSide = _lastWaypoint.IsTargetWaypointCloser(_currentWaypoint);
        if (hasReachedOtherSide)
        {
            _lastWaypoint.BlockConnection(_currentWaypoint, false);
            _currentWaypoint.BlockConnection(_lastWaypoint, false);
        }

        return GetRandomWaypoint(ListExtensions.GetListWithout
        (
            _currentWaypoint.DirectConnections,
            _currentWaypoint.BlockedConnections,
            _lastWaypoint
        ));
    }

    private Waypoint NoWaypointFallback(float chanceValue)
    {
        if (chanceValue <= _currentWaypoint.DirectConnectionsChance && HasDirectConnectionsLeft)
            return GetRandomWaypoint(ListExtensions.GetListWithout
            (
                _currentWaypoint.DirectConnections,
                _currentWaypoint.BlockedConnections,
                _lastWaypoint
            ));
        else if (HasInDirectConnectionsLeft)
            return GetRandomWaypoint(ListExtensions.GetListWithout
            (
                _currentWaypoint.IndirectConnections,
                _currentWaypoint.BlockedConnections,
                _lastWaypoint
            ));

        return _lastWaypoint;
    }

    private Waypoint CheckNextWaypointBlocked(Waypoint nextWaypoint)
    {
        bool isNextWaypointBlocked = _currentWaypoint.BlockedConnections.Contains(nextWaypoint);
        bool isStraightConnection = _currentWaypoint.DirectConnections.Count <= 1;

        if (isNextWaypointBlocked)
        {
            if (isStraightConnection)
                _currentWaypoint.BlockConnection(_lastWaypoint, true);

            _lastWaypointIsBlocked = true;
            _isFindingNewPath = true;

            nextWaypoint = _lastWaypoint;
        }

        return nextWaypoint;
    }

    private Waypoint ChooseNextWaypoint(float chanceValue, out Waypoint nextWaypoint, out bool hasChosenWaypoint)
    {
        nextWaypoint = null;
        
        if (chanceValue <= _currentWaypoint.BlockedConnectionsChance && HasBlockedConnections)
        {
            _isCheckingBlockedWaypoints = true;
            _lastWaypointIsBlocked = true;
            hasChosenWaypoint = true;
            nextWaypoint = GetRandomWaypoint(ListExtensions.GetListWithout
            (
                _currentWaypoint.BlockedConnections, 
                _lastWaypoint
            ));
            return nextWaypoint;
        }

        if (chanceValue <= _currentWaypoint.IndirectConnectionChance && HasIndirectConnections)
        {
            hasChosenWaypoint = true;
            nextWaypoint = GetRandomWaypoint(ListExtensions.GetListWithout
            (
                _currentWaypoint.IndirectConnections, 
                _lastWaypoint
            ));
            return nextWaypoint;
        }

        if (chanceValue <= _currentWaypoint.DirectConnectionsChance && HasDirectConnections)
            nextWaypoint = GetRandomWaypoint(ListExtensions.GetListWithout
            (
                _currentWaypoint.DirectConnections, 
                _lastWaypoint
            ));
        
        hasChosenWaypoint = false;

        return nextWaypoint;
    }

    private Waypoint GetRandomWaypoint(List<Waypoint> waypoints) 
        => waypoints[Random.Range(0, waypoints.Count)];

    private void MoveToWaypoint()
    {
        float distance = Vector3.Distance(transform.position, _targetPosition);
        if (distance <= 0.3f)
        {
            if (_currentWaypoint.IsDeadEnd)
            {
                var lookAtTarget = _currentWaypoint.Target.transform.position;
                lookAtTarget.y = transform.position.y;
                transform.LookAt(lookAtTarget);
                return;
            }    

            SetNewWaypoint();
            return;
        }

        transform.position +=  Speed * Time.deltaTime * transform.forward;

        Quaternion targetRotation = Quaternion.Lerp
        (
            transform.rotation,
            Quaternion.LookRotation(_targetPosition - transform.position),
            RotationSpeed * Time.deltaTime
        );

        transform.rotation = targetRotation;
        Debug.DrawLine(transform.position, _targetPosition);
    }

    private void SetNewWaypoint(Waypoint nextWaypoint = null)
    {
        if (nextWaypoint == null)
            nextWaypoint = GetNewWaypoint();

        if (nextWaypoint == null)
            return;
        
        nextWaypoint.Subscribe(this);
        _targetPosition = nextWaypoint.GetRandomPositionInWaypoint();
        _currentWaypoint.OnReachedWaypoint?.Invoke(this, nextWaypoint);

        _currentWaypoint.UnSubscribe(this);
        _lastWaypoint = _currentWaypoint;
        _currentWaypoint = nextWaypoint;
    }

    /// <summary>
    /// Sets the the movement speed back to its original state.
    /// </summary>
    public void RevertMovementSpeed() => Speed = _baseMovementSpeed;
}
