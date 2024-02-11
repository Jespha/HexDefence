using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HexGrid))]
public class HexGridEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        HexGrid _hexGrid = (HexGrid)target;

        if (GUILayout.Button("Render Grid"))
        {
            _hexGrid.RenderGrid();
        }
        if (GUILayout.Button("Generate"))
        {
            _hexGrid.MakeHexGrid();
        }
        if (GUILayout.Button("Clear Grid"))
        {
            _hexGrid.ClearHexGrid();
        }

    }

}
