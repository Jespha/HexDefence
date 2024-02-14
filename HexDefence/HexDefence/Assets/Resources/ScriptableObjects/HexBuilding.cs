using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HexBuilding", menuName = "ScriptableObjects/HexBuilding", order = 1)]
public class HexBuilding : ScriptableObject
{
    public HexBuildingType HexBuildingType;
    public string Name;
    public Building Prefab;
    public Sprite Icon;
}
