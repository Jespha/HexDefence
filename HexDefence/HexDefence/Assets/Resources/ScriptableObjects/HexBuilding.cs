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
    // public Dictionary<AttackType, string> AttackTypeToSprite = new Dictionary<AttackType, string>();

    // public void Awake()
    // {
    //     // initalize dictionary AttackTypeToSprite
    //     AttackTypeToSprite.Add(AttackType.Projectile , "<sprite name=\"Projectile\">");
    //     AttackTypeToSprite.Add(AttackType.Splash, "<sprite name=\"Splash\">");
    //     AttackTypeToSprite.Add(AttackType.Area, "<sprite name=\"Area\">");
    //     AttackTypeToSprite.Add(AttackType.Beam, "<sprite name=\"Beam\">");
    //     AttackTypeToSprite.Add(AttackType.Buff, "<sprite name=\"Buff\">");
    //     AttackTypeToSprite.Add(AttackType.Debuff, "<sprite name=\"Debuff\">");
    //     AttackTypeToSprite.Add(AttackType.Summon, "<sprite name=\"Summon\">");
    //     AttackTypeToSprite.Add(AttackType.Trap, "<sprite name=\"Trap\">");
    //     AttackTypeToSprite.Add(AttackType.Turret, "<sprite name=\"Turret\">");
    // }

    public static string AttackTypeToSprite(AttackType attackType)
    {
        return "<sprite name=\"" + attackType + "\">";
    }

}


public enum AttackType
{
    Projectile,
    Splash,
    Area,
    Economy,
    Beam,
    Buff,
    Debuff,
    Summon,
    Trap,
    Turret,
    None,
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
    Tower,
}
