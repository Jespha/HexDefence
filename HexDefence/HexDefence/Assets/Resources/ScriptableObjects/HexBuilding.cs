using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

[CreateAssetMenu(fileName = "HexBuilding", menuName = "ScriptableObjects/HexBuilding", order = 1)]
public class HexBuilding : ScriptableObject
{
    public HexBuildingType HexBuildingType;
    public string Name;
    public PooledObject Prefab;
    public Projectile ProjectilePrefab;
    public Sprite Icon;
    public int Cost;
    public TypeCollider TypeCollider;
    public Vector3 ColliderSize = new Vector3(1, 1, 1);
    public Vector3 ColliderPosition = new Vector3(0, 0, 0);
    public AttackType AttackType;
    public float AttackSpeed = 1.0f;
    public float AttackRange = 1.0f;
    public float AttackDamage = 1.0f;
    public AttackPriority TargetPriority;
    public int Level = 0;
}

public enum AttackType
{
    Projectile,
    Area,
    HitScan,
    GenerateCurrency,
    None
}

public enum TypeCollider
{
    SphereCollider,
    BoxCollider,
}

public enum AttackPriority
{
    First,
    Last,
    Closest,
    Strongest,
    Weakest,
    Random
}

public enum HexBuildingType
{
    None,
    Base,
    BasicTower,
}
