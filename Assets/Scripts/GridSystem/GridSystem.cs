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

    [SerializeField] private List<GridSystem> connectedGrids;

    private void Start() 
    {
        CreateGrid();
        ConnectGrids();
    }

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
            elevation = hit.point.y + 0.2f;
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
                
                // perform linecast between nodes to check for obstacles
                Vector3 direction = otherNode.position - currentNode.position;
                float distance = Vector3.Distance(currentNode.position, otherNode.position);
                Ray ray = new Ray(currentNode.position, direction);

                if (Physics.Linecast(ray.origin, ray.origin + ray.direction * distance))
                    continue;

                currentNode.AddConnectedNode(otherNode);
            }
        }
    }

    public void ClearGrid()
    {
        nodes = null;
    }

    public void ConnectGrids()
    {
        if (nodes == null)
            return; 

        if (connectedGrids == null)
            return;

        foreach (GridSystem otherGrid in connectedGrids)
        {
            if (otherGrid.nodes == null)
                continue;

            ConnectNodesBetweenGrids(otherGrid);
        }
    }

    public void DisconnectGrids()
    {
        foreach (GridSystem otherGrid in connectedGrids)
        {
            DisconnectNodesBetweenGrids(otherGrid);
        }
    }

    private void DisconnectNodesBetweenGrids(GridSystem otherGrid)
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Node currentNode = nodes[x, y];

                foreach (Node otherNode in otherGrid.nodes)
                {
                    if (otherNode == null)
                        continue;

                    currentNode.connectedNodes.Remove(otherNode);
                }
            }
        }
    }

    private void ConnectNodesBetweenGrids(GridSystem otherGrid)
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Node currentNode = nodes[x, y];

                CheckAndConnectNeighboringNodes(otherGrid, currentNode, x, y);
            }
        }
    }

    private void CheckAndConnectNeighboringNodes(GridSystem otherGrid, Node currentNode, int x, int y)
    {
        foreach (Node otherNode in otherGrid.nodes)
        {
            if (otherNode == null)
                continue;

            if (otherNode.isWalkable)
            {
                currentNode.AddConnectedNode(otherNode);
            }
        }
    }

    private void OnDrawGizmos()
    {
        // draw a bounding box around the grid based on grid size
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + new Vector3(gridSizeX * nodeDistance / 2f, 0, gridSizeY * nodeDistance / 2f), new Vector3(gridSizeX * nodeDistance, 0, gridSizeY * nodeDistance));

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
