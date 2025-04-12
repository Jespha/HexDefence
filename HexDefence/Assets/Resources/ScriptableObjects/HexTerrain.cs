using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HexTerrain", menuName = "ScriptableObjects/HexTerrain", order = 1)]
public class HexTerrain : ScriptableObject
{
    public string Name;
    public Material Material;
    public Sprite Icon;
    public Sprite Sprite;
}
