using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;


/// <summary>
/// SceneViewWindow class.
/// </summary>
public class SceneViewWindow : EditorWindow
{
    /// <summary>
    /// Tracks scroll position.
    /// </summary>
    private Vector2 scrollPos;

    /// <summary>
    /// Initialize window state.
    /// </summary>
    [MenuItem("HexTower/Scenes In Build")]
    internal static void Init()
    {

        var window = (SceneViewWindow)GetWindow(typeof(SceneViewWindow), false, "Scenes In Build");
        window.position = new Rect(window.position.xMin + 100f, window.position.yMin + 100f, 200f, 400f);
    }

    internal void OnGUI()
    {  
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        this.scrollPos = EditorGUILayout.BeginScrollView(this.scrollPos, false, false);

        for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            var scene = EditorBuildSettings.scenes[i];
            var sceneName = Path.GetFileNameWithoutExtension(scene.path);
            var pressed = GUILayout.Button(sceneName, EditorStyles.miniButtonLeft);

            if (pressed)
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Single);
                }
            }
            
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();

        for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            var scene = EditorBuildSettings.scenes[i];
            var sceneName = Path.GetFileNameWithoutExtension(scene.path);


            var pressed = GUILayout.Button("+", EditorStyles.miniButtonRight);
            if (pressed)
            {
                EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Additive);
            }
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();


    }
}   
