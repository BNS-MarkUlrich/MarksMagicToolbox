using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridSystem))]
public class GridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridSystem grid = (GridSystem)target;

        if (GUILayout.Button("Create Grid"))
        {
            grid.CreateGrid();
        }

        if (GUILayout.Button("Connect Grids"))
        {
            grid.ConnectGrids();
        }

        if (GUILayout.Button("Disconnect Grids"))
        {
            grid.DisconnectGrids();
        }

        if (GUILayout.Button("Clear Grid"))
        {
            grid.ClearGrid();
        }
    }
}
