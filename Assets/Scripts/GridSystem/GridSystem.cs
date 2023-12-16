using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(GridSystem))]
public class GridSystemEditor : Editor
{
    private GridSystem gridSystem;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        gridSystem = (GridSystem)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Editor Actions", EditorStyles.boldLabel);

        if (GUILayout.Button("Create Grid"))
        {
            gridSystem.CreateGrid();
        }
    }
}
#endif

public class GridSystem : MonoBehaviour
{
    [SerializeField] private Transform gridOrigin;
    [SerializeField] private int width = 50;
    [SerializeField] private int height = 50;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private float elevationThreshold = 5f;
    [SerializeField] private Node[,] grid;
    public LayerMask walkableSurfaceLayer; // Layer mask to identify walkable surfaces

    public Node[,] Grid { get { return grid; } }

    public GridSystem(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        grid = new Node[width, height];
        CreateGrid();
    }

    [ContextMenu("Create Grid")]
    public void CreateGrid()
    {
        grid = new Node[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 worldPoint = new Vector3(x * cellSize, 0, y * cellSize);
                float elevation = GetHeightAtPosition(x, y);
                bool walkable = elevation != 0;

                worldPoint.y = elevation; // Adjust the Y position based on elevation

                grid[x, y] = new Node(walkable, worldPoint, x, y, elevation);
            }
        }

        Setgrid(grid);
    }

    public void Setgrid(Node[,] grid)
    {
        this.grid = grid;
    }

    private float GetHeightAtPosition(int x, int y)
    {
        Vector3 gridPos = new Vector3(x * cellSize, 0, y * cellSize);

        RaycastHit hit;
        if (Physics.Raycast(gridPos + Vector3.up * 100f, Vector3.down, out hit, Mathf.Infinity, walkableSurfaceLayer))
        {
            return hit.point.y;
        }
        return 0f; // Default elevation if no collision found (adjust as needed)
    }

    public class Node
    {
        public bool walkable;
        public Vector3 worldPosition;
        public int gridX;
        public int gridY;
        public float elevation;

        public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY, float elevation)
        {
            this.walkable = walkable;
            this.worldPosition = worldPosition;
            this.gridX = gridX;
            this.gridY = gridY;
            this.elevation = elevation;
        }
    }

    public void DrawGrid()
    {
        if (Grid != null) 
        {
            for (int x = 0; x < width; x++) 
            {
                for (int y = 0; y < height; y++) 
                {
                    if (!Grid[x, y].walkable)
                        continue;
                        
                    Gizmos.color = grid[x, y].walkable ? Color.blue : Color.red;

                    Vector3 nodePos = grid[x, y].worldPosition;
                    Vector3 rightPos = nodePos + new Vector3(cellSize, 0, 0);
                    Vector3 upPos = nodePos + new Vector3(0, 0, cellSize);

                    if (x < width - 1)
                    {
                        Gizmos.DrawLine(nodePos, grid[x + 1, y].worldPosition);
                    }
                    if (y < height - 1)
                    {
                        Gizmos.DrawLine(nodePos, grid[x, y + 1].worldPosition);
                    }

                    Gizmos.DrawLine(nodePos, rightPos);
                    Gizmos.DrawLine(nodePos, upPos);
                }
            }
        }
    }

    private void OnDrawGizmos() 
    {
        DrawGrid();
    }
}
