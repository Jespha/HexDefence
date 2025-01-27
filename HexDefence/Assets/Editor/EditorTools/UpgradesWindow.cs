using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using Codice.CM.Client.Gui;


/// <summary>
/// SceneViewWindow class.
/// </summary>
public class UpgradesWindow : EditorWindow
{
    /// <summary>
    /// Tracks scroll position.
    /// </summary>
    private Vector2 scrollPos;

    /// <summary>
    /// Initialize window state.
    /// </summary>
    [MenuItem("HexTower/Upgrades")]
    internal static void Init()
    {

        var window = (UpgradesWindow)GetWindow(typeof(UpgradesWindow), false, "Upgrades");
        window.position = new Rect(window.position.xMin + 100f, window.position.yMin + 100f, 200f, 400f);
    }

    internal void OnGUI()
    {  
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        this.scrollPos = EditorGUILayout.BeginScrollView(this.scrollPos, false, false);
        Upgrade[] upgradeAsset = Resources.LoadAll<Upgrade>("ScriptableObjects/Upgrade");
        UpgradeRarity upgradeRarity = Resources.Load<UpgradeRarity>("ScriptableObjects/Upgrade/UpgradeRarity");
        upgradeRarity.InitializeRarityToColor();
        for (var i = 0; i < upgradeAsset.Length; i++)
        {
            var upgradeName = upgradeAsset[i].upgradeName;
            upgradeName = upgradeName.Replace("<br>", "-");
            GUI.backgroundColor = upgradeRarity.rarityToColor[upgradeAsset[i].rarity];
            var pressed = GUILayout.Button(upgradeName, EditorStyles.miniButtonLeft);
            
            if (pressed)
            {
                Selection.activeObject = upgradeAsset[i];
            }
            
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }
}   
