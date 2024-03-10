using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

[CreateAssetMenu(fileName = "Projectile", menuName = "ScriptableObjects/Projectile", order = 7)]
public class Projectile : ScriptableObject
{
    public HexBuildingType HexBuildingType;
    public string Name;
    public GameObject ProjectilePrefab;
    public GameObject LaunchVFXPrefab;
    public GameObject ImpactVFXPrefab;
    public float AttackSpeed = 1.0f;
    public float AttackDamage = 1.0f;
}