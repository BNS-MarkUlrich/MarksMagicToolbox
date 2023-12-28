using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [SerializeField] private float maxElevationDetection = 10f;
    public int gridSizeX, gridSizeY;
    public float nodeDistance = 1f;
    public Node[,] nodes;
    [SerializeField] private LayerMask walkableLayerMask;
    [SerializeField] private LayerMask obstacleLayerMask;

    public void CreateGrid()
    {
        Vector3 startPosition = transform.position;

        nodes = new Node[gridSizeX, gridSizeY];

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 nodePosition = startPosition + new Vector3(x * nodeDistance, 0, y * nodeDistance);
                Node newNode = new Node(nodePosition);

                if (IsWalkableArea(nodePosition, out float elevation))
                {
                    newNode.elevation = elevation;
                    newNode.position = new Vector3(nodePosition.x, elevation, nodePosition.z);
                    newNode.isWalkable = true;

                    // Check if there is a walkable area under the current node
                    // If there is, create a new node with the same position but with the elevation of the walkable area
                    // Concatenate the new node to the nodes array
                    // if (IsWalkableArea(newNode.position, out float elevation2))
                    // {
                    //     Node extraNode = new Node(newNode.position)
                    //     {
                    //         elevation = elevation2,
                    //         position = new Vector3(newNode.position.x, elevation2, newNode.position.z),
                    //         isWalkable = true
                    //     };

                    //     // Concatenate the extraNode to the nodes array
                    //     Node[,] newNodes = new Node[gridSizeX + 1, gridSizeY + 1];
                    //     for (int i = 0; i < gridSizeX; i++)
                    //     {
                    //         for (int j = 0; j < gridSizeY; j++)
                    //         {
                    //             newNodes[i, j] = nodes[i, j];
                    //         }
                    //     }
                    //     newNodes[gridSizeX, gridSizeY] = extraNode;
                    //     nodes = newNodes;
                    // }
                }
                else
                {
                    newNode.isWalkable = false;
                }

                nodes[x, y] = newNode;
            }
        }

        ConnectNodes();
    }

    private bool IsWalkableArea(Vector3 position, out float elevation)
    {
        // Create a ray from above the position downward
        Ray ray = new Ray(position + Vector3.up * maxElevationDetection, Vector3.down);
        RaycastHit hit;

        // Set the maximum distance for the raycast
        float maxRaycastDistance = 200f;

        if (Physics.Raycast(ray, out hit, maxRaycastDistance, obstacleLayerMask))
        {
            elevation = 0f;
            return false; // If an obstacle is hit
        }

        // Perform the raycast
        if (Physics.Raycast(ray, out hit, maxRaycastDistance, walkableLayerMask))
        {
            elevation = hit.point.y;
            return true; // If walkable terrain is hit
        }

        elevation = 0f;
        return false; // If no walkable terrain is found
    }

    private void ConnectNodes()
    {
        foreach (int x in Enumerable.Range(0, gridSizeX))
        {
            foreach (int y in Enumerable.Range(0, gridSizeY))
            {
                CheckNodeConnections(x, y);
            }
        }
    }

    private void CheckNodeConnections(int x, int y)
    {
        Node currentNode = nodes[x, y];

        foreach (int i in Enumerable.Range(x - 1, 3))
        {
            foreach (int j in Enumerable.Range(y - 1, 3))
            {
                if (i == x && j == y || i < 0 || i >= gridSizeX || j < 0 || j >= gridSizeY)
                    continue;

                Node otherNode = nodes[i, j];

                if (otherNode == null)
                    continue;

                currentNode.AddConnectedNode(otherNode);
            }
        }
    }

    public void ClearGrid()
    {
        nodes = null;
    }

    private void OnDrawGizmos()
    {
        if (nodes != null)
        {
            foreach (Node node in nodes)
            {
                if (!node.isWalkable)
                    continue;
                
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(node.position, 0.1f);
            
                Gizmos.color = Color.green;
                foreach (Node connectedNode in node.connectedNodes)
                {
                    if (!connectedNode.isWalkable)
                        continue;
                    
                    Gizmos.DrawLine(node.position, connectedNode.position);
                }
            }
        }
    }
}
