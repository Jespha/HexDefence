using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;


/// <summary>
/// SceneViewWindow class.
/// </summary>
public class EnemiesWindow : EditorWindow
{
    /// <summary>
    /// Tracks scroll position.
    /// </summary>
    private Vector2 scrollPos;

    /// <summary>
    /// Initialize window state.
    /// </summary>
    [MenuItem("HexTower/Enemies")]
    internal static void Init()
    {

        var window = (EnemiesWindow)GetWindow(typeof(EnemiesWindow), false, "Enemies");
        window.position = new Rect(window.position.xMin + 100f, window.position.yMin + 100f, 200f, 400f);
    }

    internal void OnGUI()
    {  
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        this.scrollPos = EditorGUILayout.BeginScrollView(this.scrollPos, false, false);
        Enemy[] enemiesAsset = Resources.LoadAll<Enemy>("ScriptableObjects/Enemy");

        for (var i = 0; i < enemiesAsset.Length; i++)
        {
            var enemieName = enemiesAsset[i].enemyName;
            var pressed = GUILayout.Button(enemieName, EditorStyles.miniButtonLeft);

            if (pressed)
            {
                Selection.activeObject = enemiesAsset[i];
            }
            
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        for (var i = 0; i < enemiesAsset.Length; i++)
        {
            var enemieName = enemiesAsset[i].enemyName;
            var pressed = GUILayout.Button("P", EditorStyles.miniButtonLeft);

            if (pressed)
            {
                Selection.activeObject = enemiesAsset[i].prefab;
            }
            
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }
}   
