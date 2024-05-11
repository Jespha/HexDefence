using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

[CreateAssetMenu(fileName = "Projectile", menuName = "ScriptableObjects/Projectile", order = 7)]
public class Projectile : ScriptableObject
{
    public HexBuildingType HexBuildingType;
    public string Name;
    public PooledObject ProjectilePrefab;
    public PooledObject LaunchVFXPrefab;
    public PooledObject ImpactVFXPrefab;
    public float AttackSpeed = 1.0f;
    public float AttackDamage = 1.0f;
}