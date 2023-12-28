using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node
{
    public Vector3 position;
    public List<Node> connectedNodes;
    public float elevation;
    public float maxElevationDifference;
    public float maxConnectionDistance;
    public bool isWalkable; // Represents if the node is walkable or not

    public Node(Vector3 pos)
    {
        position = pos;
        connectedNodes = new List<Node>();
        elevation = 0f;
        maxElevationDifference = 0.6f;
        maxConnectionDistance = 1.5f;
        isWalkable = true; // Set to true by default
    }

    public void AddConnectedNode(Node node)
    {
        float elevationDifference = CalculateElevation(node);
        float distance = CalculateDistance(node);
        float angle = CalculateAngle(node);

        if (elevationDifference <= maxElevationDifference && distance <= maxConnectionDistance)
        {
            connectedNodes.Add(node);
        }
    }

    private float CalculateElevation(Node otherNode)
    {
        return Mathf.Abs(elevation - otherNode.elevation);
    }

    private float CalculateDistance(Node otherNode)
    {
        return Vector3.Distance(position, otherNode.position);
    }

    private float CalculateAngle(Node otherNode)
    {
        return Vector3.Angle(position, otherNode.position);
    }
}
