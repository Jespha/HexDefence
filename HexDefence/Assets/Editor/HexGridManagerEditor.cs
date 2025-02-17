using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HexGridManager))]
public class HexGridManagerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        HexGridManager _hexGridManager = (HexGridManager)target;

        if (GUILayout.Button("Test"))
        {
            _hexGridManager.Test();
        }
        if (GUILayout.Button("Generate"))
        {
            _hexGridManager.MakeHexGrid();
        }
        if (GUILayout.Button("Clear Grid"))
        {
            _hexGridManager.ClearHexGrid();
        }
        if (GUILayout.Button("Clear TEMP"))
        {
            _hexGridManager.ClearTempHexGrid();
        }
        if (GUILayout.Button("AnimateIn"))
        {
            _hexGridManager.AnimateIn();
        }
    }

    void OnSceneGUI()
    {
        HexGridManager _hexGridManager = (HexGridManager)target;

        var style = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 10};
        style.normal.textColor = Color.black;
        GUI.contentColor = Color.white;

        if (_hexGridManager.showCoordinates == true && _hexGridManager.HexCells.Count > 0){
            for (int z = 0; z < _hexGridManager.HexCells.Count; z++)
            {

                Vector3 _pos =_hexGridManager.HexCells[z].Position;
                Vector3 _centerPosition =  _pos + _hexGridManager.transform.position;

                Handles.Label(_centerPosition, $"[{Mathf.Round(_centerPosition.x)}, {Mathf.Round(_centerPosition.z)}]\n", style);
            }
        }

    }
}
