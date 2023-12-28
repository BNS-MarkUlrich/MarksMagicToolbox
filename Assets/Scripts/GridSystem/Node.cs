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
    public Vector2Int gridPosition; // Position of the node in the grid

    // Variables for A* algorithm
    public Node parent; // Parent node for A* algorithm

    // Cost variables for A* algorithm
    public int gCost; // Cost from starting node to this node
    public int hCost; // Heuristic cost from this node to the target node
    public int FCost { get { return gCost + hCost; } } // Total cost (fCost)

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
