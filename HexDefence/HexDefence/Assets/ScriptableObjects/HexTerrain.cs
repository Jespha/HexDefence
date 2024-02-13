using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    // Start is called before the first frame update
[CreateAssetMenu(fileName = "HexTerrain", menuName = "ScriptableObjects/HexTerrain", order = 1)]
public class HexTerrain : ScriptableObject
{
    public HexTerrainType HexTerrainType;
    public string Name;
    public Material Material;
    public Sprite Icon;
}
