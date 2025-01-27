using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;


/// <summary>
/// SceneViewWindow class.
/// </summary>
public class BuildingsWindow : EditorWindow
{
    /// <summary>
    /// Tracks scroll position.
    /// </summary>
    private Vector2 scrollPos;

    /// <summary>
    /// Initialize window state.
    /// </summary>
    [MenuItem("HexTower/Buildings")]
    internal static void Init()
    {

        var window = (BuildingsWindow)GetWindow(typeof(BuildingsWindow), false, "Building");
        window.position = new Rect(window.position.xMin + 100f, window.position.yMin + 100f, 200f, 400f);
    }

    internal void OnGUI()
    {  
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        this.scrollPos = EditorGUILayout.BeginScrollView(this.scrollPos, false, false);
        HexBuilding[] hexBuilding = Resources.LoadAll<HexBuilding>("ScriptableObjects/HexBuilding");

        for (var i = 0; i < hexBuilding.Length; i++)
        {
            var buildingName = hexBuilding[i].Name;
            var pressed = GUILayout.Button(buildingName, EditorStyles.miniButtonLeft);

            if (pressed)
            {
                Selection.activeObject = hexBuilding[i];
            }
            
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        for (var i = 0; i < hexBuilding.Length; i++)
        {
            var buildingName = hexBuilding[i].Name;
            var pressed = GUILayout.Button("P", EditorStyles.miniButtonLeft);

            if (pressed)
            {
                Selection.activeObject = hexBuilding[i].Prefab;
            }
            
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }
}   
