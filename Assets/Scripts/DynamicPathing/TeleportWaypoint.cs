using System.Collections;
using UnityEngine;

public class TeleportWaypoint : Waypoint
{
    [SerializeField]
    private float _teleportTime = 1f;

    protected void Awake() => OnReachedWaypoint += TeleportFollower;
    
    private void TeleportFollower(DynamicPathFollower dynamicPathFollower, Waypoint targetWaypoint)
    {
        if (targetWaypoint.GetType() != typeof(TeleportWaypoint))
            return;

        StartCoroutine(WaitUntilTeleport(dynamicPathFollower, targetWaypoint, _teleportTime));
    }

    private IEnumerator WaitUntilTeleport(DynamicPathFollower dynamicPathFollower, Waypoint targetWaypoint, float seconds)
    {
        dynamicPathFollower.CanMove = false;
        
        yield return new WaitForSeconds(seconds / dynamicPathFollower.Speed);
        
        dynamicPathFollower.transform.position = targetWaypoint.GetRandomPositionInWaypoint();
        dynamicPathFollower.CanMove = true;
    }

    protected override void OnDestroy()
    {
        StopAllCoroutines();
        base.OnDestroy();
        OnReachedWaypoint -= TeleportFollower;
    }
}
