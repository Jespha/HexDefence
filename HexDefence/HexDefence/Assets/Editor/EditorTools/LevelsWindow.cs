using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;


/// <summary>
/// SceneViewWindow class.
/// </summary>
public class LevelsWindow : EditorWindow
{
    /// <summary>
    /// Tracks scroll position.
    /// </summary>
    private Vector2 scrollPos;

    /// <summary>
    /// Initialize window state.
    /// </summary>
    [MenuItem("Window/LevelsWindow")]
    internal static void Init()
    {

        var window = (LevelsWindow)GetWindow(typeof(LevelsWindow), false, "LevelsWindow");
        window.position = new Rect(window.position.xMin + 100f, window.position.yMin + 100f, 200f, 400f);
    }

    internal void OnGUI()
    {  
        Levels[] levelsArray = Resources.LoadAll<Levels>("ScriptableObjects/Levels");
        Levels levels = levelsArray.Length > 0 ? levelsArray[0] : null;
        EditorGUILayout.BeginHorizontal();
        var Levels = GUILayout.Button("> LevelsLayout <", EditorStyles.miniButtonLeft);
        if (Levels)
        {
            Selection.activeObject = levels;
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        this.scrollPos = EditorGUILayout.BeginScrollView(this.scrollPos, false, false);
        Level[] levelAsset = Resources.LoadAll<Level>("ScriptableObjects/Levels");

        for (var i = 0; i < levelAsset.Length; i++)
        {
            var levelName = levelAsset[i].levelName;
            var pressed = GUILayout.Button(levelName, EditorStyles.miniButtonLeft);

            if (pressed)
            {
                Selection.activeObject = levelAsset[i];
            }
            
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }
}   
