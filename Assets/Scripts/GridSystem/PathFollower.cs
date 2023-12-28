using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PathFollower : MonoBehaviour
{
    [SerializeField] private GridSystem grid;
    [SerializeField] private Transform target;
    public float speed = 5f;
    public Color pathColor = Color.red;
    public bool canMove;

    private List<Node> path;
    private int currentPathIndex = 0;

    public void MoveToTarget()
    {
        if (!canMove)
        {
            return;
        }

        Vector3 startPosition = transform.position;
        path = grid.FindPath(startPosition, target.position);
        currentPathIndex = 0;

        if (path != null && path.Count > 0)
        {
            StartCoroutine(FollowPath());
        }
    }

    private IEnumerator FollowPath()
    {
        while (currentPathIndex < path.Count)
        {
            Vector3 currentWaypoint = path[currentPathIndex].position;

            while (Vector3.Distance(transform.position, currentWaypoint) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
                yield return null;
            }

            currentPathIndex++;
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        if (path != null)
        {
            Gizmos.color = pathColor;

            for (int i = currentPathIndex; i < path.Count - 1; i++)
            {
                Gizmos.DrawLine(path[i].position, path[i + 1].position);
            }
        }
    }
}


[CustomEditor(typeof(PathFollower))]
public class PathFollowerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PathFollower pathFollower = (PathFollower)target;

        GUILayout.Space(10);
        
        if (GUILayout.Button("Move To Target"))
        {
            pathFollower.canMove = true;
            pathFollower.MoveToTarget();
        }

        if (GUILayout.Button("Stop Moving"))
        {
            pathFollower.canMove = false;
            pathFollower.StopAllCoroutines();
        }
    }
}
